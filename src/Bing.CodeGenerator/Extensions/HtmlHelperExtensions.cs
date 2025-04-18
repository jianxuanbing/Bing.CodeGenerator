using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bing.CodeGenerator.Extensions;

/// <summary>
/// HtmlHelper扩展
/// </summary>
public static class HtmlHelperExtensions
{
    /// <summary>
    /// 获取CSharp摘要
    /// </summary>
    /// <param name="helper">Html帮助类</param>
    /// <param name="summary">摘要内容</param>
    public static string GetCSharpSummary(this IHtmlHelper helper, string summary)
    {
        var sb = new StringBuilder();
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// {summary}");
        sb.AppendLine("/// </summary>");
        return sb.ToString();
    }

    /// <summary>
    /// 获取CSharp参数
    /// </summary>
    /// <param name="helper">Html帮助类</param>
    /// <param name="paramName">参数名</param>
    /// <param name="paramValue">参数值</param>
    public static string GetCSharpParam(this IHtmlHelper helper, string paramName, string paramValue) => $"/// <param name=\"{paramName}\">{paramValue}</param>";
}