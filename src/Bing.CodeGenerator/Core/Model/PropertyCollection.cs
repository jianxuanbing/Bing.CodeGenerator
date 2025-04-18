using System.Collections.ObjectModel;

namespace Bing.CodeGenerator.Core;

/// <summary>
/// 属性集合
/// </summary>
public class PropertyCollection : ObservableCollection<Property>
{
    /// <summary>
    /// 是否已处理
    /// </summary>
    public bool IsProcessed { get; set; }

    /// <summary>
    /// 主键属性集合
    /// </summary>
    public IEnumerable<Property> PrimaryKeys => this.Where(x => x.IsPrimaryKey == true);

    /// <summary>
    /// 外键属性集合
    /// </summary>
    public IEnumerable<Property> ForeignKeys => this.Where(x => x.IsForeignKey == true);

    /// <summary>
    /// 通过列名获取属性
    /// </summary>
    /// <param name="columnName">列名</param>
    public Property ByColumn(string columnName) => this.FirstOrDefault(x => x.ColumnName == columnName);

    /// <summary>
    /// 通过属性名获取属性
    /// </summary>
    /// <param name="propertyName">属性名</param>
    public Property ByProperty(string propertyName) => this.FirstOrDefault(x => x.PropertyName == propertyName);
}