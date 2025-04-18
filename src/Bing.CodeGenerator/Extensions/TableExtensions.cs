using SmartCode.Generator.Entity;

namespace Bing.CodeGenerator.Extensions;

/// <summary>
/// 表扩展
/// </summary>
public static class TableExtensions
{
    /// <summary>
    /// 获取描述
    /// </summary>
    /// <param name="table">表</param>
    public static string GetDescription(this Table table)
    {
        if (string.IsNullOrWhiteSpace(table.Description))
            return string.IsNullOrWhiteSpace(table.ConvertedName) ? table.ConvertedName : table.Name;
        return table.Description;
    }
}