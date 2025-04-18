using System.Collections;
using Bing.CodeGenerator.Extensions;
using Microsoft.Extensions.Logging;
using SmartCode;
using SmartCode.Configuration;
using SmartCode.TemplateEngine;

namespace Bing.CodeGenerator.BuildTasks;

/// <summary>
/// 多架构模板->多单文件构建任务
/// </summary>
public class MultiSchemaTemplateBuildTask : IBuildTask
{
    /// <summary>
    /// 多模板键名
    /// </summary>
    private const string TEMPLATES_KEY = "Templates";

    /// <summary>
    /// 模板键
    /// </summary>
    private const string TEMPLATE_KEY = "Key";

    /// <summary>
    /// 输出键
    /// </summary>
    private const string TEMPLATE_OUTPUT_KEY = "Output";

    /// <summary>
    /// 替换键
    /// </summary>
    private const string TEMPLATE_REPLACE_KEY = "Replace";

    /// <summary>
    /// 插件管理器
    /// </summary>
    private readonly IPluginManager _pluginManager;

    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger<MultiSchemaTemplateBuildTask> _logger;

    /// <summary>
    /// 是否已初始化
    /// </summary>
    public bool Initialized => true;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name => "MultiSchemaTemplate";

    public MultiSchemaTemplateBuildTask(IPluginManager pluginManager
        , ILogger<MultiSchemaTemplateBuildTask> logger)
    {
        _pluginManager = pluginManager;
        _logger = logger;
    }

    public void Initialize(IDictionary<string, object> parameters)
    {
    }

    /// <summary>
    /// 构建
    /// </summary>
    /// <param name="context">构建上下文</param>
    public async Task Build(BuildContext context)
    {
        var schemas = context.GetCurrentAllSchema();
        if (context.Build.Parameters.Value(TEMPLATES_KEY, out IEnumerable templates))
        {
            foreach (var templateKVs in templates)
            {
                var _templateKVs = (Dictionary<object, object>)templateKVs;
                if (!_templateKVs.Value(TEMPLATE_KEY, out string templateKey))
                    throw new SmartCodeException($"Build:{context.BuildKey},Can not find TemplateKey!");
                context.Build.TemplateEngine.Path = templateKey;
                var templateEngine = _pluginManager.Resolve<ITemplateEngine>(context.Build.TemplateEngine.Name);
                context.Result = await templateEngine.Render(context);
                if (!_templateKVs.Value(TEMPLATE_OUTPUT_KEY, out Dictionary<object, object> outputKVs))
                    throw new SmartCodeException($"Build:{context.BuildKey},Can not find Output!");
                if (context.Output == null)
                    throw new SmartCodeException($"Build:{context.BuildKey},Output can not be null!");
                Output output = new Output
                {
                    Path = context.Output.Path,
                    Mode = context.Output.Mode,
                    Name = context.Output.Name,
                    Extension = context.Output.Extension
                };
                if (outputKVs.Value(nameof(Output.Path), out string outputPath))
                    output.Path = outputPath;
                if (outputKVs.Value(nameof(Output.Mode), out CreateMode outputMode))
                    output.Mode = outputMode;
                if (string.IsNullOrEmpty(output.Path))
                    throw new SmartCodeException($"Build:{context.BuildKey},Template:{templateKey},can not find Output.Path!");
                if (!outputKVs.Value(nameof(Output.Name), out string outputName))
                    throw new SmartCodeException($"Build:{context.BuildKey},Template:{templateKey},can not find Output.Name!");
                output.Name = outputName;

                if (outputKVs.Value(nameof(Output.Extension), out string extension))
                    output.Extension = extension;

                if (_templateKVs.Value(TEMPLATE_REPLACE_KEY, out string schemaKey))
                {
                    var path = output.Path;
                    var name = output.Name;
                    foreach (var schema in schemas)
                    {
                        output.Path = path.Replace($"#{schemaKey}#", $".{schema.Name}");
                        output.Name = name.Replace($"#{schemaKey}#", $".{schema.Name}");
                        await _pluginManager.Resolve<IOutput>(context.Output.Type).Output(context, output);
                    }
                    return;
                }
                output.Path = output.Path.Replace("#Schema#", "");
                output.Name = output.Name.Replace("#Schema#", "");
                await _pluginManager.Resolve<IOutput>(context.Output.Type).Output(context, output);
            }
        }
    }
}