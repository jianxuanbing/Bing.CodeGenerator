using System.Collections.ObjectModel;

namespace Bing.CodeGenerator.Core;

/// <summary>
/// 关系集合
/// </summary>
public class RelationshipCollection : ObservableCollection<Relationship>
{
    /// <summary>
    /// 是否已处理
    /// </summary>
    public bool IsProcessed { get; set; }

    /// <summary>
    /// 通过关系名获取关系
    /// </summary>
    /// <param name="name">关系名</param>
    public Relationship ByName(string name) => this.FirstOrDefault(x => x.RelationshipName == name);

    /// <summary>
    /// 通过属性名获取关系
    /// </summary>
    /// <param name="propertyName">属性名</param>
    public Relationship ByProperty(string propertyName) =>
        this.FirstOrDefault(x => x.ThisPropertyName == propertyName);

    /// <summary>
    /// 通过其他实体名称获取关系
    /// </summary>
    /// <param name="name">其他实体名称</param>
    public Relationship ByOther(string name) => this.FirstOrDefault(x => x.OtherEntity == name);
}