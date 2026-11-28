using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrangSach.Data;
using TrangSach.Helpers;
using TrangSach.Models;
using TrangSach.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Thiếu ConnectionStrings:DefaultConnection trong appsettings.json");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        // Giao diện đăng ký chỉ yêu cầu mật khẩu từ 6 ký tự
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole>()
    .AddErrorDescriber<ViIdentityErrorDescriber>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Quản trị khóa tài khoản thì phiên đăng nhập hiện tại bị thu hồi sau tối đa 1 phút
builder.Services.Configure<SecurityStampValidatorOptions>(o => o.ValidationInterval = TimeSpan.FromMinutes(1));

// Giao diện yêu cầu đăng nhập mới xem được mọi trang (các trang Identity có [AllowAnonymous])
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

builder.Services.AddControllersWithViews(options =>
{
    // Mọi form POST đều phải kèm token chống giả mạo
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddRazorPages();

// Giữ nguyên chữ có dấu trong HTML do Razor sinh ra
builder.Services.AddWebEncoders(o => o.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

builder.Services.AddScoped<DonhangService>();
builder.Services.AddScoped<ThongkeService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Status", "?code={0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
