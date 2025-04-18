namespace Bing.CodeGenerator.Core;

/// <summary>
/// 对应关系
/// </summary>
public enum Cardinality
{
    /// <summary>
    /// 0:1
    /// </summary>
    ZoroOrOne,
    /// <summary>
    /// 1:1
    /// </summary>
    One,
    /// <summary>
    /// 1:n
    /// </summary>
    Many
}