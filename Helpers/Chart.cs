using System.Globalization;
using Microsoft.AspNetCore.Html;

namespace TrangSach.Helpers;

public static class Chart
{
    static string F(double d) => d.ToString("0.##", CultureInfo.InvariantCulture);

    public static IHtmlContent Area(IReadOnlyList<double> v)
    {
        const int W = 600, H = 180;
        if (v.Count < 2) v = new[] { 0d, 0d };
        double mx = Math.Max(v.Max(), 1) * 1.12;
        var p = v.Select((y, i) => (X: i * W / (double)(v.Count - 1), Y: H - y / mx * H)).ToList();
        string l = string.Join(" ", p.Select(a => F(a.X) + "," + F(a.Y)));
        var e = p[p.Count - 1];
        string grid = string.Concat(Enumerable.Range(0, 4).Select(i =>
            $"<line x1=\"0\" x2=\"{W}\" y1=\"{F(i * H / 3.0)}\" y2=\"{F(i * H / 3.0)}\" style=\"stroke:var(--line)\" stroke-dasharray=\"3 5\"/>"));
        return new HtmlString(
            $"<svg viewBox=\"0 0 {W} {H + 6}\" width=\"100%\" style=\"display:block;overflow:visible\"><defs><linearGradient id=\"g1\" x1=\"0\" x2=\"0\" y1=\"0\" y2=\"1\"><stop offset=\"0\" style=\"stop-color:var(--p);stop-opacity:.28\"/><stop offset=\"1\" style=\"stop-color:var(--p);stop-opacity:0\"/></linearGradient></defs>" +
            grid +
            $"<polygon points=\"0,{H} {l} {W},{H}\" fill=\"url(#g1)\"/><polyline points=\"{l}\" fill=\"none\" style=\"stroke:var(--p)\" stroke-width=\"2.2\" stroke-linejoin=\"round\"/><circle cx=\"{F(e.X)}\" cy=\"{F(e.Y)}\" r=\"4.5\" style=\"fill:var(--bg);stroke:var(--p)\" stroke-width=\"2.2\"/></svg>");
    }

    public static IHtmlContent Spark(IReadOnlyList<double> v)
    {
        if (v.Count < 2) v = new[] { 0d, 0d };
        double mx = Math.Max(v.Max(), 1);
        string p = string.Join(" ", v.Select((y, i) => F(i * 100.0 / (v.Count - 1)) + "," + F(30 - y / mx * 26)));
        return new HtmlString(
            $"<svg viewBox=\"0 0 100 32\" preserveAspectRatio=\"none\" width=\"100%\" height=\"30\"><polyline points=\"{p}\" fill=\"none\" style=\"stroke:var(--p)\" stroke-width=\"1.6\" vector-effect=\"non-scaling-stroke\"/></svg>");
    }
}
