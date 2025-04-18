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

    /// <summary>
    /// 显示配置信息
    /// </summary>
    /// <param name="target">目标模板</param>
    /// <param name="slnName">解决方案名称</param>
    /// <param name="slnTypeValue">解决方案类型值</param>
    /// <param name="item">代码生成项</param>
    /// <param name="configFile">配置文件名</param>
    /// <param name="targetFramework">目标框架</param>
    /// <param name="outputPath">输出路径</param>
    public static void DisplayConfigurationInfo(string target, string slnName, string slnTypeValue, CodeGenItem item, string configFile, string targetFramework, string outputPath)
    {
        PrintSectionHeader("代码生成配置信息");
        System.Console.WriteLine($"模板名称: {target}");
        System.Console.WriteLine($"解决方案: {slnName}");
        System.Console.WriteLine($"生成方式: {slnTypeValue}");
        System.Console.WriteLine($"配置文件: {configFile}");

        // 数据库信息
        System.Console.WriteLine("\n【数据库信息】");
        System.Console.WriteLine($"数据库名称: {item.DbName}");
        System.Console.WriteLine($"数据库提供程序: {item.DbProvider}");
        System.Console.WriteLine($"连接字符串: {item.DbConnectionString}");

        // 输出信息
        System.Console.WriteLine("\n【输出信息】");
        var finalOutputPath = string.IsNullOrWhiteSpace(outputPath)
            ? Path.Combine(item.OutputPath, targetFramework)
            : Path.Combine(outputPath, $"generate_{slnName}");
        System.Console.WriteLine($"输出路径: {finalOutputPath}");
        System.Console.WriteLine($"工作单元: {item.UnitOfWorkName}");

        // 过滤器信息
        System.Console.WriteLine("\n【过滤器信息】");
        if (item.TableFilter != null)
        {
            System.Console.WriteLine("表过滤器: 已配置");
            if (item.TableFilter.IncludeTables?.Count() > 0)
                System.Console.WriteLine($"  - 包含表: {string.Join(", ", item.TableFilter.IncludeTables)}");
            if (item.TableFilter.IgnoreTables?.Count() > 0)
                System.Console.WriteLine($"  - 排除表: {string.Join(", ", item.TableFilter.IgnoreTables)}");
        }
        else
        {
            System.Console.WriteLine("表过滤器: 未配置");
        }

        if (item.SchemaFilter != null)
        {
            System.Console.WriteLine("架构过滤器: 已配置");
            if (item.SchemaFilter.IncludeSchemas?.Count() > 0)
                System.Console.WriteLine($"  - 包含架构: {string.Join(", ", item.SchemaFilter.IncludeSchemas)}");
            if (item.SchemaFilter.IgnoreSchemas?.Count() > 0)
                System.Console.WriteLine($"  - 排除架构: {string.Join(", ", item.SchemaFilter.IgnoreSchemas)}");
        }
        else
        {
            System.Console.WriteLine("架构过滤器: 未配置");
        }

        System.Console.WriteLine(new string('=', 58));
    }
}