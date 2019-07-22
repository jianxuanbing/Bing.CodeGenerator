namespace Bing.CodeGenerator.Core
{
    /// <summary>
    /// 实体
    /// </summary>
    public class Entity : EntityBase
    {
        /// <summary>
        /// 实体上下文
        /// </summary>
        public EntityContext Context { get; set; }

        /// <summary>
        /// 上下文名称
        /// </summary>
        public string ContextName { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 映射名称
        /// </summary>
        public string MappingName { get; set; }

        /// <summary>
        /// 表架构
        /// </summary>
        public string TableSchema { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 全名
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 属性集合
        /// </summary>
        public PropertyCollection Properties { get; set; } = new PropertyCollection();

        /// <summary>
        /// 关系集合
        /// </summary>
        public RelationshipCollection Relationships { get; set; } = new RelationshipCollection();

        /// <summary>
        /// 方法集合
        /// </summary>
        public MethodCollection Methods { get; set; } = new MethodCollection();


    }
}
