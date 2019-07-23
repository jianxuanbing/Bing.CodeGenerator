namespace Bing.CodeGenerator.Core
{
    /// <summary>
    /// 关系
    /// </summary>
    public class Relationship : EntityBase
    {
        /// <summary>
        /// 关系名称
        /// </summary>
        public string RelationshipName { get; set; }

        /// <summary>
        /// 当前实体
        /// </summary>
        public string ThisEntity { get; set; }

        /// <summary>
        /// 当前属性名
        /// </summary>
        public string ThisPropertyName { get; set; }

    }
}
