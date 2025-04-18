namespace Bing.CodeGenerator.Core;

/// <summary>
/// 方案类型
/// </summary>
public enum SlnType
{
    /// <summary>
    /// 根目录
    /// </summary>
    [Obsolete("弃用枚举类型")]
    Root = 0,
    /// <summary>
    /// 解决方案文件夹
    /// </summary>
    Dir = 1,
    /// <summary>
    /// 项目
    /// </summary>
    Project = 2
}