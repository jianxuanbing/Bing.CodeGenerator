using Bing.CodeGenerator.Core;
using Bing.CodeGenerator.Entity;
using SmartCode;

namespace Bing.CodeGenerator.Extensions;

/// <summary>
/// 构建上下文扩展
/// </summary>
public static class BuildContextExtensions
{
    /// <summary>
    /// 当前全部架构
    /// </summary>
    public const string CurrentAllSchema = "CurrentAllSchema";

    /// <summary>
    /// 当前架构
    /// </summary>
    public const string CurrentSchema = "CurrentSchema";

    /// <summary>
    /// 实体上下文
    /// </summary>
    public const string EntityContext = "EntityContext";

    /// <summary>
    /// 获取当前所有架构
    /// </summary>
    /// <param name="context">构建上下文</param>
    public static IEnumerable<Schema> GetCurrentAllSchema(this BuildContext context)
    {
        if (!context.Items.ContainsKey(CurrentAllSchema))
        {
            var filter = context.Project.GetSchemaFilter();
            var schemas = context.GetDataSource<DbTableWithSchemaSource>().Schemas;
            if (filter != null)
            {
                if (filter.IgnoreSchemas.Any())
                    schemas = schemas.Where(x => !filter.IgnoreSchemas.Contains(x.Name)).ToList();
                if (filter.IncludeSchemas.Any())
                    schemas = schemas.Where(x => filter.IncludeSchemas.Contains(x.Name)).ToList();
            }
            context.SetItem(CurrentAllSchema, schemas);
            return schemas;
        }

        return context.GetItem<IEnumerable<Schema>>(CurrentAllSchema);
    }

    /// <summary>
    /// 设置当前架构
    /// </summary>
    /// <param name="context">构建上下文</param>
    /// <param name="schema">架构</param>
    public static void SetCurrentSchema(this BuildContext context, Schema schema) => context.SetItem(CurrentSchema, schema);

    /// <summary>
    /// 获取当前架构
    /// </summary>
    /// <param name="context">构建上下文</param>
    public static Schema GetCurrentSchema(this BuildContext context) => context.GetItem<Schema>(CurrentSchema);

    /// <summary>
    /// 获取领域名称
    /// </summary>
    /// <param name="context">构建上下文</param>
    public static string GetDomainName(this BuildContext context)
    {
        var schema = context.GetCurrentSchema();
        return schema.IsDefault ? $"{context.Project.Module}.Domain" : $"{context.Project.Module}.{schema.Name}.Domain";
    }

    /// <summary>
    /// 获取领域名称
    /// </summary>
    /// <param name="context">构建上下文</param>
    /// <param name="module">模块</param>
    public static string GetDomainName(this BuildContext context, string module) => $"{context.GetDomainName()}.{module}";

    /// <summary>
    /// 获取领域名称
    /// </summary>
    /// <param name="context">构建上下文</param>
    /// <param name="module">模块</param>
    /// <param name="suffix">后缀</param>
    public static string GetDomainName(this BuildContext context, string module, string suffix) => $"{context.GetDomainName(module)}.{suffix}";

    /// <summary>
    /// 获取数据名称
    /// </summary>
    /// <param name="context">构建上下文</param>
    public static string GetDataName(this BuildContext context) => $"{context.Project.Module}.Data";

    /// <summary>
    /// 获取数据名称
    /// </summary>
    /// <param name="context">构建上下文</param>
    /// <param name="module">模块</param>
    public static string GetDataName(this BuildContext context, string module)
    {
        var schema = context.GetCurrentSchema();
        return schema.IsDefault ? $"{context.GetDataName()}.{module}" : $"{context.GetDataName()}.{module}.{schema.Name}";
    }

    /// <summary>
    /// 获取数据名称
    /// </summary>
    /// <param name="context">构建上下文</param>
    /// <param name="module">模块</param>
    /// <param name="suffix">后缀</param>
    public static string GetDataName(this BuildContext context, string module, string suffix) => $"{context.GetDataName(module)}.{suffix}";

    /// <summary>
    /// 获取服务名称
    /// </summary>
    /// <param name="context">构建上下文</param>
    public static string GetServiceName(this BuildContext context) => $"{context.Project.Module}.Service";

    /// <summary>
    /// 获取服务名称
    /// </summary>
    /// <param name="context">构建上下文</param>
    /// <param name="module">模块</param>
    public static string GetServiceName(this BuildContext context, string module)
    {
        var schema = context.GetCurrentSchema();
        return schema.IsDefault ? $"{context.GetServiceName()}.{module}" : $"{context.GetServiceName()}.{module}.{schema.Name}";
    }

    /// <summary>
    /// 获取服务名称
    /// </summary>
    /// <param name="context">构建上下文</param>
    /// <param name="module">模块</param>
    /// <param name="suffix">后缀</param>
    public static string GetServiceName(this BuildContext context, string module, string suffix) => $"{context.GetServiceName(module)}.{suffix}";

    /// <summary>
    /// 获取API名称
    /// </summary>
    /// <param name="context">构建上下文</param>
    /// <param name="module">模块</param>
    public static string GetApiName(this BuildContext context, string module)
    {
        var schema = context.GetCurrentSchema();
        return schema.IsDefault ? $"{module}.Apis" : $"{module}.Apis.{schema.Name}";
    }

    /// <summary>
    /// 获取实体上下文
    /// </summary>
    /// <param name="context">构建上下文</param>
    public static EntityContext GetEntityContext(this BuildContext context)
    {
        if (!context.Items.ContainsKey(EntityContext))
        {
            var entityContextBuilder = new EntityContextBuilder();
            var entityContext = entityContextBuilder.Build(context);
            context.SetItem(EntityContext, entityContext);
            return entityContext;
        }
        return context.GetItem<EntityContext>(EntityContext);
    }
}