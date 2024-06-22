using System.Linq;
using System.Threading.Tasks;
using Bing.CodeGenerator.Extensions;
using Microsoft.Extensions.Logging;
using SmartCode;
using SmartCode.Generator.BuildTasks;
using SmartCode.TemplateEngine;

namespace Bing.CodeGenerator.BuildTasks
{
    /// <summary>
    /// 架构表构建任务
    /// </summary>
    public class SchemaTableBuildTask : AbstractDbBuildTask
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<SchemaTableBuildTask> _logger;

        /// <summary>
        /// 插件管理器
        /// </summary>
        private readonly IPluginManager _pluginManager;

        /// <summary>
        /// 初始化一个<see cref="SchemaTableBuildTask"/>类型的实例
        /// </summary>
        /// <param name="pluginManager">插件管理器</param>
        /// <param name="logger">日志</param>
        public SchemaTableBuildTask(IPluginManager pluginManager, ILogger<SchemaTableBuildTask> logger) : base("SchemaTable", logger)
        {
            _pluginManager = pluginManager;
            _logger = logger;
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="context">构建上下文</param>
        public override async Task Build(BuildContext context)
        {
            var schemas = context.GetCurrentAllSchema();
            foreach (var schema in schemas)
            {
                //if (schema.Name == "dbo")
                //    continue;
                _logger.LogInformation($"BuildSchema:{schema.Name} Start!");
                context.SetCurrentSchema(schema);
                var filterTables = FilterTable(schema.Tables, context.BuildKey, context.Build).ToList();
                foreach (var table in filterTables)
                {
                    _logger.LogInformation($"BuildTable:{table.Name} Start!");
                    context.SetCurrentTable(table);
                    _pluginManager.Resolve<INamingConverter>().Convert(context);
                    context.Result = await _pluginManager.Resolve<ITemplateEngine>(context.Build.TemplateEngine.Name).Render(context);
                    await _pluginManager.Resolve<IOutput>(context.Build.Output.Type).Output(context);
                    _logger.LogInformation($"BuildTable:{table.Name} End!");
                }
                _logger.LogInformation($"BuildSchema:{schema.Name} End!");
            }
        }
    }
}
