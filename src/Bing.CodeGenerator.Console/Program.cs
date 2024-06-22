using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bing.CodeGenerator.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharprompt;
using SmartCode.App;
using SmartCode.Utilities;

namespace Bing.CodeGenerator.Console
{
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
        private static readonly IDictionary<int, string> _templateDict = new Dictionary<int, string>();

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
            System.Console.CancelKeyPress += (sender, arg) =>
            {
                arg.Cancel = true;
            };
            System.Console.WriteLine($"============================================================================================");
            System.Console.WriteLine(Figgle.FiggleFonts.Standard.Render("Bing CodeGenerator"));
            System.Console.WriteLine($"============================================================================================");
            System.Console.WriteLine($"============================================================================================");
            System.Console.WriteLine($"===== 环境路径：{AppContext.BaseDirectory}");
            System.Console.WriteLine($"===== 配置路径：{Directory.GetCurrentDirectory()}");
            System.Console.WriteLine($"============================================================================================");
            // 初始化配置信息
            InitTemplateDict();
            var options = GetCodeGenOptions();
            InitCodeGenDict(options);

            var target = Prompt.Select("请选择生成代码模板?", _templateDict.Values);
            System.Console.WriteLine($"你选中的代码模板为：{target}");
            System.Console.WriteLine($"==========================================================");
            System.Console.WriteLine($"===========    欢迎使用{target}代码生成功能器    ===========");
            System.Console.WriteLine($"==========================================================");
            var slnName = Prompt.Select("请选择需要生成的解决方案?", _codeGenDict.Values.Select(x => x.Key));
            System.Console.WriteLine($"解决方案：{slnName}");
            System.Console.WriteLine($"==========================================================");
            var slnTypeKv = Prompt.Select("请选择生成代码方式?", _codeGenModeDict, textSelector: x => x.Value);
            System.Console.WriteLine($"生成代码方式：{slnTypeKv.Value}");
            var outputPath = Prompt.Input<string>("请输入代码生成目录");
            if (string.IsNullOrWhiteSpace(outputPath) || !Directory.Exists(outputPath))
            {
                var result = Prompt.Confirm("输入的目录路径无效，是否使用默认配置的输出代码路径？", true);
                if (!result)
                {
                    System.Console.WriteLine("终止代码生成！！！");
                    System.Console.ReadLine();
                    return;
                }
            }
            System.Console.WriteLine($"==========================================================");

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
            if (!File.Exists(Path.Combine(basePath, CodeSettingsPath)))
                basePath = AppDomain.CurrentDomain.BaseDirectory;
            var codeSettingsBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(CodeSettingsPath, false, true);
            var configuration = codeSettingsBuilder.Build();
            var codeGenOptions = configuration.GetSection(CodeGenKey).Get<CodeGenOptions>();
            return codeGenOptions;
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
                // 如果代码生成字典中的值，已存在Key，则无需再次加入字典中
                if (_codeGenDict.Values.Any(x => x.Key == item.Key))
                    continue;
                _codeGenDict.Add(i, item);
                i++;
            }
        }

        /// <summary>
        /// 初始化模板字典
        /// </summary>
        private static void InitTemplateDict()
        {
            var appPath = $"{AppContext.BaseDirectory}/RazorTemplates";
            // 内置模板库
            var dir = new DirectoryInfo(appPath);
            var i = 1;
            foreach (var dictionary in dir.GetDirectories().Where(x => !x.Name.StartsWith('.')))
            {
                _templateDict[i] = dictionary.Name;
                i++;
            }
            if (AppContext.BaseDirectory.TrimEnd('\\').Equals(Directory.GetCurrentDirectory().TrimEnd('\\')))
                return;

            // 检查自定义模板路径，如果路径等同应用路径，则无需下一步操作
            var customPath = $"{Directory.GetCurrentDirectory()}/RazorTemplates";
            if (!Directory.Exists(customPath))
                return;
            // 自定义模板库
            var customDir = new DirectoryInfo(customPath);
            CopyDir(customDir.FullName, dir.FullName);
            foreach (var dictionary in customDir.GetDirectories().Where(x => !x.Name.StartsWith('.')))
            {
                if (_templateDict.Values.Contains(dictionary.Name))
                    continue;
                _templateDict[i] = dictionary.Name;
                i++;
            }
        }

        /// <summary>
        /// 复制目录
        /// </summary>
        /// <param name="sourceDir">源目录</param>
        /// <param name="destDir">目标目录</param>
        /// <param name="backupsDir">备份文件夹全名</param>
        private static void CopyDir(string sourceDir, string destDir, string backupsDir = null)
        {
            if (Directory.Exists(sourceDir) && Directory.Exists(destDir))
            {
                var sourceDirInfo = new DirectoryInfo(sourceDir);
                var fileInfos = sourceDirInfo.GetFiles();
                foreach (var fileInfo in fileInfos)
                {
                    var sourceFile = fileInfo.FullName;
                    var destFile = sourceFile.Replace(sourceDir, destDir);
                    if (backupsDir != null && File.Exists(destFile))
                    {
                        Directory.CreateDirectory(backupsDir);
                        var backFile = destFile.Replace(destDir, backupsDir);
                        File.Copy(destDir, backFile, true);
                    }

                    File.Copy(sourceFile, destFile, true);
                }

                var dirInfos = sourceDirInfo.GetDirectories();
                foreach (var dirInfo in dirInfos)
                {
                    var sourceDir2 = dirInfo.FullName;
                    var destDir2 = sourceDir2.Replace(sourceDir, destDir);
                    string backupsDir2 = null;
                    if (backupsDir != null)
                        backupsDir2 = sourceDir2.Replace(sourceDir, backupsDir);
                    Directory.CreateDirectory(destDir2);
                    CopyDir(sourceDir2, destDir2, backupsDir2);
                }
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
            var buildSettings = "";
            switch (slnType)
            {
                case 1:
                    buildSettings = $"{targetFramework}SlnGenerateConfig.yml";
                    break;
                case 2:
                    buildSettings = $"{targetFramework}CodeGenerateConfig.yml";
                    break;
            }
            // 拼接前置路径
            buildSettings = Path.Combine("Configs", buildSettings);
            // 构建结果路径
            var resultFilePath = File.Exists(AppPath.Relative(buildSettings))
                ? AppPath.Relative(buildSettings)
                : Path.Combine(Directory.GetCurrentDirectory(), buildSettings);

            if (!File.Exists(resultFilePath))
            {
                System.Console.WriteLine($"路径不存在：{buildSettings}");
                return null;
            }

            var app = new SmartCodeAppBuilder().Build(resultFilePath);
            app.Project.Module = item.SlnName;
            app.Project.DataSource.Parameters["DbName"] = item.DbName;
            app.Project.DataSource.Parameters["DbProvider"] = item.DbProvider;
            app.Project.DataSource.Parameters["ConnectionString"] = item.DbConnectionString;
            app.Project.Output.Path = string.IsNullOrWhiteSpace(outputPath) ? item.OutputPath : Path.Combine(outputPath, $"generate_{slnName}");
            app.Project.Parameters["UnitOfWork"] = item.UnitOfWorkName;
            // 设置表过滤
            if (item.TableFilter != null)
                app.Project.TableFilter = item.TableFilter;
            // 设计架构过滤
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
}
