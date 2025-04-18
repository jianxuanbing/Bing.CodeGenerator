using Bing.CodeGenerator.Extensions;
using Microsoft.Extensions.Logging;
using SmartCode;
using SmartCode.Configuration;
using SmartCode.Generator.BuildTasks;
using SmartCode.Generator.Entity;
using SmartCode.TemplateEngine;

namespace Bing.CodeGenerator.BuildTasks;

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
            context.Build.IgnoreNoPKTable = true;
            var filterTables = FilterTable(schema.Tables, context.BuildKey, context.Build);
            filterTables = FilterTable(filterTables, context.BuildKey, context.Project);
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

    /// <summary>
    /// 过滤表
    /// </summary>
    /// <param name="tables">表集合</param>
    /// <param name="buildKey">构建KEY</param>
    /// <param name="project">项目</param>
    protected IList<Table> FilterTable(IEnumerable<Table> tables, string buildKey, Project project)
    {
        _logger.LogInformation($"Project FilterTable Build:{buildKey} Start!");
        IEnumerable<Table> buildTables = CopyTables(tables);
        var tableFilter = project.TableFilter;
        if (tableFilter != null)
        {
            if (tableFilter.IgnoreNoPKTable.HasValue && tableFilter.IgnoreNoPKTable.Value)
            {
                _logger.LogInformation($"Project FilterTable Build:{buildKey} IgnoreNoPKTable!");
                buildTables = buildTables.Where(m => m.PKColumn != null);
            }

            if (tableFilter.IgnoreView.HasValue && tableFilter.IgnoreView.Value)
            {
                _logger.LogInformation($"Project FilterTable Build:{buildKey} IgnoreView!");
                buildTables = buildTables.Where(m => m.Type != Table.TableType.View);
            }

            if (tableFilter.IgnoreTables != null)
            {
                _logger.LogInformation(
                    $"Project FilterTable Build:{buildKey} IgnoreTables: [{String.Join(",", tableFilter.IgnoreTables)}]!");
                buildTables = buildTables.Where(m => !tableFilter.IgnoreTables.Contains(m.Name));
            }

            if (tableFilter.IncludeTables != null)
            {
                _logger.LogInformation(
                    $"Project FilterTable Build:{buildKey} IncludeTables: [{String.Join(",", tableFilter.IncludeTables)}]!");
                buildTables = buildTables.Where(m => tableFilter.IncludeTables.Contains(m.Name));
            }
        }

        _logger.LogInformation($"Project FilterTable Build:{buildKey} End!");
        return buildTables.ToList();
    }
}