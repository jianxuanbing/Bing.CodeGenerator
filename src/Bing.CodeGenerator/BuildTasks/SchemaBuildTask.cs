using Bing.CodeGenerator.Extensions;
using Microsoft.Extensions.Logging;
using SmartCode;
using SmartCode.Generator.BuildTasks;
using SmartCode.TemplateEngine;

namespace Bing.CodeGenerator.BuildTasks;

/// <summary>
/// 架构构建任务
/// </summary>
public class SchemaBuildTask : AbstractDbBuildTask
{
    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger<SchemaBuildTask> _logger;

    /// <summary>
    /// 插件管理器
    /// </summary>
    private readonly IPluginManager _pluginManager;

    /// <summary>
    /// 初始化一个<see cref="SchemaBuildTask"/>类型的实例
    /// </summary>
    /// <param name="pluginManager">插件管理器</param>
    /// <param name="logger">日志</param>
    public SchemaBuildTask(IPluginManager pluginManager, ILogger<SchemaBuildTask> logger) : base("Schema", logger)
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
            context.Result = await _pluginManager.Resolve<ITemplateEngine>(context.Build.TemplateEngine.Name).Render(context);
            await _pluginManager.Resolve<IOutput>(context.Build.Output.Type).Output(context);
            _logger.LogInformation($"BuildSchema:{schema.Name} End!");
        }
    }
}