namespace Bing.CodeGenerator.Configuration;

/// <summary>
/// 架构过滤
/// </summary>
public class SchemaFilter
{
    /// <summary>
    /// 包含架构
    /// </summary>
    public IEnumerable<string> IncludeSchemas { get; set; }

    /// <summary>
    /// 忽略架构
    /// </summary>
    public IEnumerable<string> IgnoreSchemas { get; set; }
}