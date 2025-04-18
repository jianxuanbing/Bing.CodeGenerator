using Sharprompt;

namespace Bing.CodeGenerator.Console;

/// <summary>
/// 控制台 帮助类
/// </summary>
public static class ConsoleHelper
{
    /// <summary>
    /// 打印头部信息
    /// </summary>
    public static void PrintHeader()
    {
        var headerLine = new string('=', 92);
        System.Console.WriteLine(headerLine);
        System.Console.WriteLine(Figgle.FiggleFonts.Standard.Render("Bing CodeGenerator"));
        System.Console.WriteLine(headerLine);
        System.Console.WriteLine(headerLine);
        System.Console.WriteLine($"===== 环境路径：{AppContext.BaseDirectory}");
        System.Console.WriteLine($"===== 配置路径：{Directory.GetCurrentDirectory()}");
        System.Console.WriteLine(headerLine);
    }

    /// <summary>
    /// 打印区块标题
    /// </summary>
    /// <param name="title">标题</param>
    public static void PrintSectionHeader(string title)
    {
        var separatorLine = new string('=', 58);
        System.Console.WriteLine(separatorLine);
        System.Console.WriteLine($"===========    {title}    ===========");
        System.Console.WriteLine(separatorLine);
    }

    /// <summary>
    /// 获取有效的输出路径
    /// </summary>
    public static string GetValidOutputPath()
    {
        var outputPath = Prompt.Input<string>("请输入代码生成目录");
        if (!string.IsNullOrWhiteSpace(outputPath) && Directory.Exists(outputPath))
            return outputPath;

        var result = Prompt.Confirm("输入的目录路径无效，是否使用默认配置的输出代码路径？", true);
        if (!result)
        {
            System.Console.WriteLine("终止代码生成！！！");
            System.Console.ReadLine();
            return null;
        }
        return string.Empty; // 返回空字符串表示使用默认配置
    }
}