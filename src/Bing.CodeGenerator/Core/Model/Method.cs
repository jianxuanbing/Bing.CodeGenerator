namespace Bing.CodeGenerator.Core;

/// <summary>
/// 方法
/// </summary>
public class Method : EntityBase
{
    /// <summary>
    /// 名称后缀
    /// </summary>
    public string NameSuffix { get; set; }

    /// <summary>
    /// 原始名称
    /// </summary>
    public string SourceName { get; set; }

    /// <summary>
    /// 是否主键
    /// </summary>
    public bool IsKey { get; set; }

    /// <summary>
    /// 是否唯一索引
    /// </summary>
    public bool IsUnique { get; set; }

    /// <summary>
    /// 是否索引
    /// </summary>
    public bool IsIndex { get; set; }

    /// <summary>
    /// 属性列表
    /// </summary>
    public List<Property> Properties { get; set; } = new List<Property>();
}