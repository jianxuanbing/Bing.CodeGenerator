using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Bing.CodeGenerator.Extensions;

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

        #region GetPk(获取主键)

        public Property GetPk()
        {
            var key = Properties.FirstOrDefault(x => x.IsPrimaryKey.SafeValue());
            if (key == null)
            {
                Console.WriteLine($"TableName: {TableName}, 没有设置主键!");
                throw new Exception("没有设置主键");
            }
            return key;
        }

        #endregion

        #region GetPkType(获取主键类型)

        /// <summary>
        /// 获取主键类型
        /// </summary>
        public Type GetPkType() => GetPk().SystemType;

        /// <summary>
        /// 获取主键类型
        /// </summary>
        public string GetPkTypeString() => GetPkType().ToNullableType();

        #endregion

        #region GetPkDefault(获取主键默认值)

        /// <summary>
        /// 获取主键默认值
        /// </summary>
        public string GetPkDefault()
        {
            if (GetPkType() == typeof(string))
                return "string.Empty";
            if (GetPkType() == typeof(int))
                return "0";
            if (GetPkType() == typeof(long))
                return "0";
            return "Guid.Empty";
        }

        #endregion

        #region GetOtherDescription(获取导航属性描述)

        /// <summary>
        /// 获取导航属性描述
        /// </summary>
        /// <param name="name">导航属性类名</param>
        public string GetOtherDescription(string name) => Context.Entities.ByClass(name).Description;

        #endregion

        #region GetProperties(获取属性集合)

        /// <summary>
        /// 获取属性集合，不包含Version属性
        /// </summary>
        /// <param name="isExcludeVersion">是否排除版本号</param>
        public List<Property> GetProperties(bool isExcludeVersion = true)
        {
            if (isExcludeVersion)
                return Properties.Where(x =>
                    (string.Equals(x.ColumnName, Const.Version, StringComparison.CurrentCultureIgnoreCase) &&
                     x.DataType == DbType.Binary) == false).ToList();
            return Properties.ToList();
        }

        #endregion

        #region GetFirstProperty(获取第一个不是Id的属性)

        /// <summary>
        /// 获取第一个不是Id的属性
        /// </summary>
        public Property GetFirstProperty() => GetProperties().FirstOrDefault(x => x.IsPrimaryKey != true) ?? new Property();

        #endregion

        #region IsLastProperty(是否最后一个属性)

        /// <summary>
        /// 是否最后一个属性
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="isExcludeVersion">是否排除版本号</param>
        /// <returns></returns>
        public bool IsLastProperty(Property property, bool isExcludeVersion = true) => GetProperties(isExcludeVersion).Last() == property;

        #endregion

        #region GetComma(获取逗号)

        /// <summary>
        /// 获取逗号
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="isExcludeVersion">是否排除版本号</param>
        public string GetComma(Property property, bool isExcludeVersion = true) => IsLastProperty(property, isExcludeVersion) ? "" : ",";

        #endregion

        #region GetAggregateRootAndInterfaces(获取聚合根和需要实现的接口)

        /// <summary>
        /// 获取聚合根和需要实现的接口
        /// </summary>
        public string GetAggregateRootAndInterfaces()
        {
            var result = new StringBuilder();
            result.Append(GetAggregateRoot());
            result.Append(GetInterfaces());
            return result.ToString();
        }

        #region GetAggregateRoot(获取聚合根)

        /// <summary>
        /// 获取聚合根
        /// </summary>
        public string GetAggregateRoot()
        {
            if (GetPkType() == typeof(string))
                return $"AggregateRoot<{ClassName}, string>";
            if (GetPkType() == typeof(int))
                return $"AggregateRoot<{ClassName}, int>";
            if (GetPkType() == typeof(long))
                return $"AggregateRoot<{ClassName}, long>";
            return $"AggregateRoot<{ClassName}>";
        }

        #endregion

        #endregion

        #region GetTreeAggregateRootAndInterfaces(获取树型聚合跟和需要实现的接口)

        /// <summary>
        /// 获取树型聚合跟和需要实现的接口
        /// </summary>
        public string GetTreeAggregateRootAndInterfaces()
        {
            var result = new StringBuilder();
            result.Append(GetTreeAggregateRoot());
            result.Append(GetInterfaces());
            return result.ToString();
        }

        #region GetTreeAggregateRoot(获取树型聚合根)

        /// <summary>
        /// 获取树型聚合根
        /// </summary>
        public string GetTreeAggregateRoot()
        {
            if (GetPkType() == typeof(string))
                return $"TreeEntityBase<{ClassName}, string, string>";
            if (GetPkType() == typeof(int))
                return $"TreeEntityBase<{ClassName}, int, int?>";
            if (GetPkType() == typeof(long))
                return $"TreeEntityBase<{ClassName}, long>";
            return $"TreeEntityBase<{ClassName}>";
        }

        #endregion

        #endregion

        #region GetInterfaces(获取需要实现的接口)

        /// <summary>
        /// 获取需要实现的接口
        /// </summary>
        public string GetInterfaces()
        {
            var result = new StringBuilder();
            //result.Append(GetITenant()); // TODO: 注释租户，目前暂时未使用
            result.Append(GetISoftDelete());
            result.Append(GetIAudited());
            return result.ToString();
        }

        /// <summary>
        /// 获取租户接口
        /// </summary>
        private string GetITenant() => AnyColumn(Const.TenantId) ? ", ITenant" : string.Empty;

        /// <summary>
        /// 是否存在指定列
        /// </summary>
        /// <param name="columnName">列名</param>
        private bool AnyColumn(string columnName) => Properties.Any(x => string.Equals(x.ColumnName, columnName, StringComparison.CurrentCultureIgnoreCase));

        /// <summary>
        /// 是否存在指定列
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="func">函数操作</param>
        private bool AnyColumn(string columnName, Func<Property, bool> func) => Properties.Any(x =>  string.Equals(x.ColumnName, columnName, StringComparison.CurrentCultureIgnoreCase) && func(x));

        /// <summary>
        /// 获取逻辑删除接口
        /// </summary>
        public string GetISoftDelete() => AnyColumn(Const.IsDeleted) ? ", ISoftDelete" : string.Empty;

        /// <summary>
        /// 获取审计接口
        /// </summary>
        private string GetIAudited()
        {
            if (IsCreatorAudited() && IsModifierAudited())
                return ", IAuditedObjectWithName";
            if (IsCreationAudited() && IsModificationAudited())
                return ", IAuditedObject";
            if (IsCreatorAudited())
                return ", ICreationAuditedObjectWithName";
            if (IsCreationAudited())
                return ", ICreationAuditedObject";
            if (IsModifierAudited())
                return ", IModificationAuditedObjectWithName";
            if (IsModificationAudited())
                return ", IModificationAuditedObject";
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

        #endregion

        #region GetKeyTypeNoContainsGuid(获取键类型)

        /// <summary>
        /// 获取键类型，但不包括Guid
        /// </summary>
        /// <param name="isAddCommon">是否在前面添加逗号</param>
        public string GetKeyTypeNoContainsGuid(bool isAddCommon = true)
        {
            if (GetPkType() == typeof(Guid))
                return "";
            var comma = isAddCommon ? "," : "";
            return $"{comma}{GetPkTypeString()}";
        }

        #endregion

        #region GetToEntityConvert(获取转换为实体)

        /// <summary>
        /// 获取转换为实体
        /// </summary>
        public string GetToEntityConvert()
        {
            if (GetPkType() == typeof(Guid))
                return ".ToGuid()";
            if (GetPkType() == typeof(int))
                return ".ToInt()";
            return "";
        }

        #endregion

        #region GetToDtoConvert(获取转换为数据传输对象)

        /// <summary>
        /// 获取转换为数据传输对象
        /// </summary>
        public string GetToDtoConvert()
        {
            if (GetPkType() == typeof(Guid))
                return ".ToString()";
            return "";
        }

        #endregion

        #region GetNewKey(获取新的主键值)

        /// <summary>
        /// 获取新的主键值
        /// </summary>
        public string GetNewKey()
        {
            if (GetPkType() == typeof(Guid))
                return "Guid.NewGuid()";
            if(GetPkType()==typeof(string))
                return "Guid.NewGuid().ToString()";
            return "";
        }

        #endregion

        #region GetOrderBy(获取排序字段)

        /// <summary>
        /// 获取排序字段
        /// </summary>
        public string GetOrderBy() => AnyColumn(Const.CreationTime, x => x.SystemType == typeof(DateTime)) ? Const.CreationTime : Const.Id;

        #endregion

        #region GetNamespace(获取命名空间)

        /// <summary>
        /// 获取命名空间
        /// </summary>
        /// <param name="baseNamespace">基命名空间</param>
        /// <param name="layer">层</param>
        /// <param name="category">分类</param>
        public string GetNamespace(string baseNamespace, string layer, string category = "") =>
            string.IsNullOrWhiteSpace(TableName) || TableName.ToLower().Trim() == "dbo"
                ? $"{baseNamespace}.{layer}{GetCategory(category)}"
                : $"{baseNamespace}{TableSchema}{layer}{GetCategory(category)}";

        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="category">分类</param>
        private string GetCategory(string category) => string.IsNullOrWhiteSpace(category) ? string.Empty : $".{category}";

        #endregion

        #region GetGridProperties(获取Grid显示的属性集合)

        /// <summary>
        /// 获取Grid显示的属性集合
        /// </summary>
        public List<Property> GetGridProperties() =>
            Properties.Where(t =>
                    (t.ColumnName == "Version" && t.DataType == DbType.Binary) == false &&
                    (t.ColumnName == "IsDeleted" && t.DataType == DbType.Boolean) == false &&
                    (t.ColumnName == "CreatorId" && t.DataType == DbType.Guid) == false &&
                    (t.ColumnName == "LastModificationTime" && t.DataType == DbType.DateTime) ==
                    false &&
                    (t.ColumnName == "LastModifierId" && t.DataType == DbType.Guid) == false &&
                    (t.ColumnName == "TenantId" && t.DataType == DbType.String) == false)
                .ToList();

        #endregion

        #region GetGridComma(获取Grid项的逗号)

        /// <summary>
        /// 获取Grid项的逗号
        /// </summary>
        /// <param name="property">属性</param>
        public string GetGridComma(Property property) => GetGridProperties().Last() == property ? "" : ",";

        #endregion

        #region IsTreeEntity(是否树型实体)

        /// <summary>
        /// 是否树型实体
        /// </summary>
        public bool IsTreeEntity() => AnyColumn(Const.ParentId) && AnyColumn(Const.Path) && AnyColumn(Const.Level);

        #endregion
    }
}
