using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrangSach.Models;

namespace TrangSach.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _env = env;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public bool HienGoiY => _env.IsDevelopment();

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập email")]
            [RegularExpression(@"^[^ @]+@[^ @]+[.][^ @]+$", ErrorMessage = "Email chưa hợp lệ")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";
        }

        public IActionResult OnGet(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return LocalRedirect(User.IsInRole("Admin") ? "~/Admin" : "~/");

            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                var email = Input.Email.Trim();
                var result = await _signInManager.PasswordSignInAsync(email, Input.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    TempData["toast"] = "Đăng nhập thành công";
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                        return LocalRedirect("~/Admin");

                    return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                        ? LocalRedirect(returnUrl)
                        : LocalRedirect("~/");
                }

                ModelState.AddModelError(string.Empty, result.IsLockedOut
                    ? "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên."
                    : "Email hoặc mật khẩu không đúng.");
            }
            return Page();
        }
    }
}
