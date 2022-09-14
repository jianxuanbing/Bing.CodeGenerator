using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
        private static IDictionary<int, string> TemplateDict = new Dictionary<int, string>();

        /// <summary>
        /// 代码生成字典
        /// </summary>
        /// <remarks>索引值 - (名称键, 名称值)</remarks>
        private static IDictionary<int, KeyValuePair<string, CodeGenItem>> _codeGenDict = new Dictionary<int, KeyValuePair<string, CodeGenItem>>();

        /// <summary>
        /// 主函数
        /// </summary>
        public static async Task Main(string[] args)
        {
            System.Console.CancelKeyPress += (sender, arg) =>
            {
                arg.Cancel = true;
            };

            var templateIndex = Convert.ToInt32(InputTemplate());
            var target = TemplateDict[templateIndex];
            var options = GetCodeGenOptions();
            InitCodeGenDict(options);

            System.Console.WriteLine($"欢迎使用{target}代码生成功能器");
            var slnName = InputSlnName(options);
            System.Console.WriteLine($"解决方案: {slnName}");
            var slnType = InputSlnType(options);
            System.Console.WriteLine($"生成代码方式: {slnType}");
            var app = GetSmartCodeApp(slnType, options[slnName], target);
            System.Console.WriteLine("-----------------------------开始生成代码-----------------------------");
            await app.Run();
            System.Console.WriteLine("-----------------------------结束生成代码-----------------------------");
            System.Console.ReadLine();
        }

        /// <summary>
        /// 获取代码配置
        /// </summary>
        private static CodeGenOptions GetCodeGenOptions()
        {
            var basePath = Directory.GetCurrentDirectory();
            System.Console.WriteLine($"读取配置文件路径: {basePath}");
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
        /// 输入模板
        /// </summary>
        private static string InputTemplate()
        {
            OutputTemplateList();
            var result= System.Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(result))
            {
                System.Console.WriteLine("不能输入空模板，请重新输入模板索引!");
                return InputTemplate();
            }

            if (!TemplateDict.ContainsKey(Convert.ToInt32(result)))
            {
                System.Console.WriteLine($"不存在该【{result}】方案，请重新输入代码方案!");
                return InputTemplate();
            }

            return result;
        }

        /// <summary>
        /// 输出模板列表
        /// </summary>
        private static void OutputTemplateList()
        {
            System.Console.WriteLine("请选择生成代码模板(索引): ");
            var dir = new DirectoryInfo($"{AppContext.BaseDirectory}/RazorTemplates");

            var i = 1;
            foreach (var dictionary in dir.GetDirectories().Where(x => !x.Name.StartsWith('.')))
            {
                System.Console.WriteLine($"{i}. {dictionary.Name}");
                TemplateDict[i] = dictionary.Name;
                i++;
            }
        }

        /// <summary>
        /// 输入方案名称
        /// </summary>
        /// <param name="dict">字典</param>
        private static string InputSlnName(IDictionary<string, CodeGenItem> dict)
        {
            OutputSlnList();
            var result = "";
            result = System.Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(result))
            {
                System.Console.WriteLine("不能输入空方案，请重新输入代码方案!");
                return InputSlnName(dict);
            }

            if (int.TryParse(result, out var intResult))
            {
                if (!_codeGenDict.ContainsKey(intResult))
                {
                    System.Console.WriteLine($"不存在该【{result}】方案，请重新输入代码方案!");
                    return InputSlnName(dict);
                }
            }

            return _codeGenDict[intResult].Key;
        }

        /// <summary>
        /// 输出方案列表
        /// </summary>
        /// <param name="dict">字典</param>
        private static void OutputSlnList(IDictionary<string, CodeGenItem> dict)
        {
            System.Console.WriteLine("请选择需要生成的解决方案(名称): ");
            var i = 1;
            foreach (var item in dict)
            {
                System.Console.WriteLine($"{i}. {item.Key}");
                i++;
            }
        }

        /// <summary>
        /// 输出方案列表
        /// </summary>
        private static void OutputSlnList()
        {
            System.Console.WriteLine("请选择需要生成的解决方案(索引): ");
            foreach (var item in _codeGenDict) 
                System.Console.WriteLine($"{item.Key}. {item.Value.Key}");
        }

        /// <summary>
        /// 输入解决方案类型
        /// </summary>
        /// <param name="dict">字典</param>
        private static int InputSlnType(IDictionary<string, CodeGenItem> dict)
        {
            OutputSlnType();
            var result = "";
            result = System.Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(result))
            {
                System.Console.WriteLine("不能输入空字符串，请重新输入生成代码方式!");
                return InputSlnType(dict);
            }

            if (int.TryParse(result, out var intResult))
            {
                switch (intResult)
                {
                    case 1:
                    case 2:
                        return intResult;
                    default:
                        System.Console.WriteLine($"不存在该【{result}】生成代码方式，请重新输入生成代码方式!");
                        return InputSlnType(dict);
                }
            }
            System.Console.WriteLine($"无法识别该【{result}】生成代码方式，请重新输入生成代码方式!");
            return InputSlnType(dict);
        }

        /// <summary>
        /// 输出方案类型
        /// </summary>
        private static void OutputSlnType()
        {
            System.Console.WriteLine("请选择生成代码方式(索引): ");
            System.Console.WriteLine("1. 解决方案模式");
            System.Console.WriteLine("2. 代码生成模式");
        }

        /// <summary>
        /// 获取SmartCodeApp
        /// </summary>
        /// <param name="slnType">解决方案类型</param>
        /// <param name="item">代码生成项</param>
        /// <param name="targetFramework">目标框架</param>
        private static SmartCodeApp GetSmartCodeApp(int slnType, CodeGenItem item, string targetFramework)
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

            var app = new SmartCodeAppBuilder().Build(AppPath.Relative(buildSettings));
            app.Project.Module = item.SlnName;
            app.Project.DataSource.Parameters["DbName"] = item.DbName;
            app.Project.DataSource.Parameters["DbProvider"] = item.DbProvider;
            app.Project.DataSource.Parameters["ConnectionString"] = item.DbConnectionString;
            app.Project.Output.Path = item.OutputPath;
            app.Project.Parameters["UnitOfWork"] = item.UnitOfWorkName;
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
