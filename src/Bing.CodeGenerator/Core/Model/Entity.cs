using System;
using System.Linq;

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

        /// <summary>
        /// 获取租户接口
        /// </summary>
        private string GetITenant() => AnyColumn(Const.TenantId) ? ",ITenant" : string.Empty;

        /// <summary>
        /// 是否存在指定列
        /// </summary>
        /// <param name="columnName">列名</param>
        private bool AnyColumn(string columnName) => Properties.Any(x => string.Equals(x.ColumnName, columnName, StringComparison.CurrentCultureIgnoreCase));

        /// <summary>
        /// 获取逻辑删除接口
        /// </summary>
        private string GetISoftDelete() => AnyColumn(Const.IsDeleted) ? ",IDelete" : string.Empty;
        
        /// <summary>
        /// 获取审计接口
        /// </summary>
        private string GetIAudited()
        {
            if (IsCreatorAudited() && IsModifierAudited())
                return ",IAuditor";
            if (IsCreationAudited() && IsModificationAudited())
                return ",IAudited";
            if (IsCreatorAudited())
                return ",ICreatorAudited";
            if (IsCreationAudited())
                return ",ICreationAudited";
            if (IsModifierAudited())
                return ",IModifierAudited";
            if (IsModificationAudited())
                return ",IModificationAudited";
            return string.Empty;
        }

        /// <summary>
        /// 是否创建审计
        /// </summary>
        private bool IsCreationAudited() => AnyColumn(Const.CreationTime) && AnyColumn(Const.CreatorId);

        /// <summary>
        /// 是否创建人审计
        /// </summary>
        /// <returns></returns>
        private bool IsCreatorAudited() => IsCreationAudited() && AnyColumn(Const.Creator);

        /// <summary>
        /// 是否修改审计
        /// </summary>
        private bool IsModificationAudited() => AnyColumn(Const.LastModificationTime) && AnyColumn(Const.LastModifierId);

        /// <summary>
        /// 是否修改人审计
        /// </summary>
        /// <returns></returns>
        private bool IsModifierAudited() => IsCreationAudited() && AnyColumn(Const.LastModifier);


    }
}
