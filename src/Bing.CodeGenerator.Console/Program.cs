using Bing.CodeGenerator.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharprompt;
using SmartCode.App;
using SmartCode.Utilities;

namespace Bing.CodeGenerator.Console;

/// <summary>
/// 进程
/// </summary>
public class Program
{
    /// <summary>
    /// 代码设置路径
    /// </summary>
    private const string CodeSettingsPath = "codesettings.json";

    /// <summary>
    /// Code生成键名
    /// </summary>
    private const string CodeGenKey = "CodeGenDict";

    /// <summary>
    /// 模板字典
    /// </summary>
    private static readonly Dictionary<int, string> _templateDict = new Dictionary<int, string>();

    /// <summary>
    /// 代码生成字典
    /// </summary>
    /// <remarks>索引值 - (名称键, 名称值)</remarks>
    private static readonly IDictionary<int, KeyValuePair<string, CodeGenItem>> _codeGenDict = new Dictionary<int, KeyValuePair<string, CodeGenItem>>();

    /// <summary>
    /// 代码生成模式
    /// </summary>
    private static readonly IDictionary<int, string> _codeGenModeDict = new Dictionary<int, string>
    {
        { 1, "解决方案模式" },
        { 2, "代码生成模式" }
    };

    /// <summary>
    /// 主函数
    /// </summary>
    public static async Task Main(string[] args)
    {
        System.Console.CancelKeyPress += (sender, arg) => arg.Cancel = true;
        
        ConsoleHelper.PrintHeader();

        // 初始化配置信息
        InitTemplateDict();
        var options = GetCodeGenOptions();
        InitCodeGenDict(options);

        // 用户选择代码模板
        var target = Prompt.Select("请选择生成代码模板?", _templateDict.Values);
        System.Console.WriteLine($"你选中的代码模板为：{target}");
        ConsoleHelper.PrintSectionHeader($"欢迎使用{target}代码生成功能器");
        
        // 用户选择解决方案
        var slnName = Prompt.Select("请选择需要生成的解决方案?", _codeGenDict.Values.Select(x => x.Key));
        System.Console.WriteLine($"解决方案：{slnName}");
        System.Console.WriteLine(new string('=', 58));

        // 用户选择代码生成方式
        var slnTypeKv = Prompt.Select("请选择生成代码方式?", _codeGenModeDict, textSelector: x => x.Value);
        System.Console.WriteLine($"生成代码方式：{slnTypeKv.Value}");

        // 用户输入输出路径
        var outputPath = ConsoleHelper.GetValidOutputPath();
        if (outputPath == null)
            return;
        System.Console.WriteLine(new string('=', 58));

        // 获取代码生成配置
        var codeGenItem = options[slnName];
        var configFileName = slnTypeKv.Key switch
        {
            1 => $"{target}SlnGenerateConfig.yml",
            2 => $"{target}CodeGenerateConfig.yml",
            _ => ""
        };

        // 显示配置信息并确认
        ConsoleHelper.DisplayConfigurationInfo(target, slnName, slnTypeKv.Value, codeGenItem, configFileName, target, outputPath);
        if (!Prompt.Confirm("确认以上配置并继续生成代码?", true))
        {
            System.Console.WriteLine("已取消代码生成操作！");
            System.Console.ReadLine();
            return;
        }

        // 生成代码
        var app = GetSmartCodeApp(slnTypeKv.Key, slnName, options[slnName], target, outputPath);
        if (app == null)
        {
            System.Console.WriteLine("生成代码失败，路径不存在！！！");
            System.Console.ReadLine();
            return;
        }
        app.Logger.LogInformation("-----------------------------开始生成代码-----------------------------");
        await app.Run();
        app.Logger.LogInformation("-----------------------------结束生成代码-----------------------------");
        System.Console.ReadLine();
    }

    /// <summary>
    /// 获取代码配置
    /// </summary>
    private static CodeGenOptions GetCodeGenOptions()
    {
        var basePath = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(basePath, CodeSettingsPath);
        if (!File.Exists(configPath)) 
            basePath = AppDomain.CurrentDomain.BaseDirectory;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile(CodeSettingsPath, false, true)
            .Build();
        return configuration.GetSection(CodeGenKey).Get<CodeGenOptions>();
    }

    /// <summary>
    /// 初始化代码生成字典
    /// </summary>
    /// <param name="dict">字典</param>
    private static void InitCodeGenDict(IDictionary<string, CodeGenItem> dict)
    {
        var i = 1;
        foreach (var item in dict)
        {
            // 避免重复添加
            if (_codeGenDict.Values.Any(x => x.Key == item.Key))
                continue;
            _codeGenDict.Add(i++, item);
        }
    }

    /// <summary>
    /// 初始化模板字典
    /// </summary>
    private static void InitTemplateDict()
    {
        var appPath = Path.Combine(AppContext.BaseDirectory, "RazorTemplates");
        LoadTemplatesFromDirectory(appPath, _templateDict);

        // 如果当前目录与应用程序目录相同，则无需加载自定义模板
        if (PathsAreEqual(AppContext.BaseDirectory, Directory.GetCurrentDirectory()))
            return;

        // 加载自定义模板
        var customPath = Path.Combine(Directory.GetCurrentDirectory(), "RazorTemplates");
        if (!Directory.Exists(customPath))
            return;

        // 复制自定义模板到应用目录
        CopyDir(customPath, appPath);

        // 加载未重复的自定义模板
        LoadTemplatesFromDirectory(customPath, _templateDict, true);
    }

    /// <summary>
    /// 从目录加载模板
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    /// <param name="templateDict">模板字典</param>
    /// <param name="skipExisting">是否跳过已存在模板</param>
    private static void LoadTemplatesFromDirectory(string directoryPath, Dictionary<int, string> templateDict, bool skipExisting = false)
    {
        if(!Directory.Exists(directoryPath))
            return;
        var dirInfo = new DirectoryInfo(directoryPath);
        var nextIndex = templateDict.Count > 0 ? templateDict.Keys.Max() + 1 : 1;
        foreach (var directory in dirInfo.GetDirectories().Where(x => !x.Name.StartsWith('.')))
        {
            if (skipExisting && templateDict.Values.Contains(directory.Name))
                continue;
            templateDict[nextIndex++] = directory.Name;
        }
    }

    /// <summary>
    /// 判断两个路径是否相等
    /// </summary>
    /// <param name="path1">路径1</param>
    /// <param name="path2">路径2</param>
    private static bool PathsAreEqual(string path1, string path2) => path1.TrimEnd('\\').Equals(path2.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// 复制目录
    /// </summary>
    /// <param name="sourceDir">源目录</param>
    /// <param name="destDir">目标目录</param>
    /// <param name="backupsDir">备份文件夹全名</param>
    private static void CopyDir(string sourceDir, string destDir, string backupsDir = null)
    {
        if (!Directory.Exists(sourceDir) || !Directory.Exists(destDir))
            return;

        var sourceDirInfo = new DirectoryInfo(sourceDir);

        // 复制文件
        foreach (var fileInfo in sourceDirInfo.GetFiles())
        {
            var sourceFile = fileInfo.FullName;
            var destFile = sourceFile.Replace(sourceDir, destDir);

            // 备份现有文件
            if (backupsDir != null && File.Exists(destFile))
            {
                Directory.CreateDirectory(backupsDir);
                var backFile = destFile.Replace(destDir, backupsDir);
                File.Copy(destDir, backFile, true);
            }

            File.Copy(sourceFile, destFile, true);
        }

        // 递归复制子目录
        foreach (var dirInfo in sourceDirInfo.GetDirectories())
        {
            var sourceSubDir = dirInfo.FullName;
            var destSubDir = sourceSubDir.Replace(sourceDir, destDir);
            var backupsSubDir = backupsDir != null ? sourceSubDir.Replace(sourceDir, backupsDir) : null;

            Directory.CreateDirectory(destSubDir);
            CopyDir(sourceSubDir, destSubDir, backupsSubDir);
        }
    }

    /// <summary>
    /// 获取SmartCodeApp
    /// </summary>
    /// <param name="slnType">解决方案类型</param>
    /// <param name="slnName">解决方案名称</param>
    /// <param name="item">代码生成项</param>
    /// <param name="targetFramework">目标框架</param>
    /// <param name="outputPath">输出路径</param>
    private static SmartCodeApp GetSmartCodeApp(int slnType, string slnName, CodeGenItem item, string targetFramework, string outputPath)
    {
        var configFileName = slnType switch
        {
            1 => $"{targetFramework}SlnGenerateConfig.yml",
            2 => $"{targetFramework}CodeGenerateConfig.yml",
            _ => ""
        };
        var buildSettings = Path.Combine("Configs", configFileName);

        // 尝试找到配置文件
        var resultFilePath = File.Exists(AppPath.Relative(buildSettings))
            ? AppPath.Relative(buildSettings)
            : Path.Combine(Directory.GetCurrentDirectory(), buildSettings);

        if (!File.Exists(resultFilePath))
        {
            System.Console.WriteLine($"路径不存在：{buildSettings}");
            return null;
        }

        // 构建SmartCode应用程序
        var app = new SmartCodeAppBuilder().Build(resultFilePath);

        // 设置项目参数
        app.Project.Module = item.SlnName;
        app.Project.DataSource.Parameters["DbName"] = item.DbName;
        app.Project.DataSource.Parameters["DbProvider"] = item.DbProvider;
        app.Project.DataSource.Parameters["ConnectionString"] = item.DbConnectionString;

        // 设置输出路径
        app.Project.Output.Path = string.IsNullOrWhiteSpace(outputPath)
            ? Path.Combine(item.OutputPath, targetFramework)
            : Path.Combine(outputPath, $"generate_{slnName}");

        app.Project.Parameters["UnitOfWork"] = item.UnitOfWorkName;

        // 设置过滤器
        if (item.TableFilter != null)
            app.Project.TableFilter = item.TableFilter;
        if (item.SchemaFilter != null)
            app.Project.Parameters[nameof(SchemaFilter)] = item.SchemaFilter;

        return app;
    }

    /// <summary>
    /// 是否退出
    /// </summary>
    private static bool IsExit()
    {
        System.Console.WriteLine("是否退出代码生成( 0 | exit | yes): ");
        var result = "";
        result = System.Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(result))
            return false;
        if (result == "exit" || result == "0" || result == "yes" || result == "y")
            return true;
        return false;
    }
}