using System.Collections.Generic;

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

        /// <summary>
        /// 当前对应关系
        /// </summary>
        public Cardinality ThisCardinality { get; set; }

        /// <summary>
        /// 当前属性名集合
        /// </summary>
        public List<string> ThisProperties { get; set; } = new List<string>();

        /// <summary>
        /// 其他实体
        /// </summary>
        public string OtherEntity { get; set; }

        /// <summary>
        /// 其他实体属性名
        /// </summary>
        public string OtherPropertyName { get; set; }

        /// <summary>
        /// 其他实体对应关系
        /// </summary>
        public Cardinality OtherCardinality { get; set; }

        /// <summary>
        /// 其他实体属性名集合
        /// </summary>
        public List<string> OtherProperties { get; set; } = new List<string>();

        /// <summary>
        /// 删除可空
        /// </summary>
        public bool? DeleteOnNull { get; set; }

        /// <summary>
        /// 是否外键
        /// </summary>
        public bool IsForeignKey { get; set; }

        /// <summary>
        /// 是否映射
        /// </summary>
        public bool IsMapped { get; set; }

        /// <summary>
        /// 是否多对多关系
        /// </summary>
        public bool IsManyToMany => ThisCardinality == Cardinality.Many && OtherCardinality == Cardinality.Many;

        /// <summary>
        /// 是否一对一关系
        /// </summary>
        public bool IsOneToOne => ThisCardinality == Cardinality.One && OtherCardinality == Cardinality.One;

        /// <summary>
        /// 关联表
        /// </summary>
        public string JoinTable { get; set; }

        /// <summary>
        /// 关联架构
        /// </summary>
        public string JoinSchema { get; set; }

        /// <summary>
        /// 关联当前列
        /// </summary>
        public List<string> JoinThisColumn { get; set; }

        /// <summary>
        /// 关联其他列
        /// </summary>
        public List<string> JoinOtherColumn { get; set; }
    }
}
