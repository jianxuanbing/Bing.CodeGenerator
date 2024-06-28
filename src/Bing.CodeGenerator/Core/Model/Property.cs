﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml.Serialization;
using Bing.CodeGenerator.Extensions;

namespace Bing.CodeGenerator.Core
{
    /// <summary>
    /// 属性
    /// </summary>
    public class Property : EntityBase
    {
        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public DbType DataType { get; set; }

        /// <summary>
        /// 原生类型
        /// </summary>
        public string NativeType { get; set; }

        /// <summary>
        /// 系统类型
        /// </summary>
        [XmlIgnore]
        public Type SystemType { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Order { get; set; }

        /// <summary>
        /// 是否指定排序
        /// </summary>
        public bool OrderSpecified => Order.HasValue;

        /// <summary>
        /// 是否可空值
        /// </summary>
        public bool? IsNullable { get; set; }

        /// <summary>
        /// 是否指定可空值
        /// </summary>
        public bool IsNullableSpecified => IsNullable.HasValue;

        /// <summary>
        /// 是否必填项
        /// </summary>
        public bool IsRequired
        {
            get => IsNullable == false;
            set => IsNullable = !value;
        }

        /// <summary>
        /// 是否可选项
        /// </summary>
        public bool IsOptional
        {
            get => IsNullable == true;
            set => IsNullable = value;
        }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool? IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否指定主键
        /// </summary>
        public bool IsPrimaryKeySpecified => IsPrimaryKey.HasValue;

        /// <summary>
        /// 是否外键
        /// </summary>
        public bool? IsForeignKey { get; set; }

        /// <summary>
        /// 是否指定外键
        /// </summary>
        public bool IsForeignKeySpecified => IsForeignKey.HasValue;

        /// <summary>
        /// 是否自动生成
        /// </summary>
        public bool? IsAutoGenerated { get; set; }

        /// <summary>
        /// 是否指定自动生成
        /// </summary>
        public bool IsAutoGeneratedSpecified => IsAutoGenerated.HasValue;

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool? IsReadOnly { get; set; }

        /// <summary>
        /// 是否指定只读
        /// </summary>
        public bool IsReadOnlySpecified => IsReadOnly.HasValue;

        /// <summary>
        /// 是否行版本
        /// </summary>
        public bool? IsRowVersion { get; set; }

        /// <summary>
        /// 是否指定行版本
        /// </summary>
        public bool IsRowVersionSpecified => IsRowVersion.HasValue;

        /// <summary>
        /// 是否自增列
        /// </summary>
        public bool? IsIdentity { get; set; }

        /// <summary>
        /// 是否指定自增列
        /// </summary>
        public bool IsIdentitySpecified => IsIdentity.HasValue;

        /// <summary>
        /// 是否唯一索引
        /// </summary>
        public bool? IsUnique { get; set; }

        /// <summary>
        /// 是否指定唯一索引
        /// </summary>
        public bool IsUniqueSpecified => IsUnique.HasValue;

        /// <summary>
        /// 是否Unicode编码
        /// </summary>
        public bool? IsUnicode { get; set; }

        /// <summary>
        /// 是否指定Unicode编码
        /// </summary>
        public bool IsUnicodeSpecified => IsUnicode.HasValue;

        /// <summary>
        /// 是否固定长度
        /// </summary>
        public bool? IsFixedLength { get; set; }

        /// <summary>
        /// 是否指定固定长度
        /// </summary>
        public bool IsFixedLengthSpecified => IsFixedLength.HasValue;

        /// <summary>
        /// 最大长度
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// 是否指定最大长度
        /// </summary>
        public bool MaxLengthSpecified => MaxLength.HasValue;

        /// <summary>
        /// 精度
        /// </summary>
        public byte? Precision { get; set; }

        /// <summary>
        /// 是否指定精度
        /// </summary>
        public bool PrecisionSpecified => Precision.HasValue;

        /// <summary>
        /// 小数位数
        /// </summary>
        public int? Scale { get; set; }

        /// <summary>
        /// 是否指定小数位数
        /// </summary>
        public bool ScaleSpecified => Scale.HasValue;

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName => SystemType.ToNullableType(IsNullable == true);

        /// <summary>
        /// 是否标识
        /// </summary>
        public bool IsKey => IsPrimaryKey.SafeValue();

        #region Filter(是否过滤)
        /// <summary>
        /// 是否过滤
        /// </summary>
        /// <param name="columnName">列名</param>
        public bool Filter(string columnName) =>
            string.Equals(ColumnName, columnName, StringComparison.CurrentCultureIgnoreCase);

        /// <summary>
        /// 是否过滤
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="func">函数操作</param>
        public bool Filter(string columnName, Func<Property, bool> func) =>
            string.Equals(ColumnName, columnName, StringComparison.CurrentCultureIgnoreCase) &&
            func(this);

        #endregion

        #region IsHidden(是否隐藏)

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsHidden()
        {
            if (Filter(Const.Version, x => x.DataType == DbType.Binary))
                return true;
            if (Filter(Const.IsDeleted, x => x.DataType == DbType.Boolean))
                return true;
            if (Filter(Const.CreationTime, x => x.DataType == DbType.DateTime))
                return true;
            if (Filter(Const.CreatorId, x => x.DataType == DbType.Guid))
                return true;
            if (Filter(Const.LastModificationTime, x => x.DataType == DbType.DateTime))
                return true;
            if (Filter(Const.LastModifierId, x => x.DataType == DbType.Guid))
                return true;
            return false;
        }

        #endregion

        #region IsFilterByQueryObject(是否过滤查询实体属性)

        /// <summary>
        /// 是否过滤查询实体属性
        /// </summary>
        public bool IsFilterByQueryObject()
        {
            if (Filter(Const.Version, x => x.DataType == DbType.Binary))
                return true;
            if (Filter(Const.IsDeleted, x => x.DataType == DbType.Boolean))
                return true;
            return false;
        }

        #endregion

        #region GetFormat(获取格式化)

        /// <summary>
        /// 获取格式化
        /// </summary>
        public string GetFormat()
        {
            if (DataType == DbType.Boolean)
                return ".Description()";
            if (DataType == DbType.DateTime)
                return ".ToDateTimeString( true )";
            return string.Empty;
        }

        #endregion

        #region IgnoreTreeEntityProperty(忽略树型实体属性)

        /// <summary>
        /// 忽略树型实体属性
        /// </summary>
        public bool IgnoreTreeEntityProperty()
        {
            if (Filter(Const.ParentId))
                return true;
            if (Filter(Const.Path, x => x.DataType == DbType.String))
                return true;
            if (Filter(Const.Level, x => x.DataType == DbType.Int32))
                return true;
            if (Filter(Const.Enabled, x => x.DataType == DbType.Boolean))
                return true;
            if (Filter(Const.SortId, x => x.DataType == DbType.Int32))
                return true;
            return false;
        }

        #endregion

        #region HasValidate(是否需要验证)

        /// <summary>
        /// 是否需要验证
        /// </summary>
        public bool HasValidate() => GetValidations().Count > 0;

        /// <summary>
        /// 获取验证列表
        /// </summary>
        private List<string> GetValidations()
        {
            var result = new List<string>();
            ValidateRequired(result);
            ValidateStringLength(result);
            return result;
        }

        /// <summary>
        /// 验证是否必填项
        /// </summary>
        /// <param name="result">结果</param>
        private void ValidateRequired(List<string> result)
        {
            if (IsRequired == false)
                return;
            if (DataType == DbType.Boolean)
                return;
            result.Add($"[Required(ErrorMessage = \"{Description?.Replace("\r\n", "")?.Replace(" ", "")}不能为空\")]");
        }

        /// <summary>
        /// 验证字符串长度
        /// </summary>
        /// <param name="result">结果</param>
        private void ValidateStringLength(List<string> result)
        {
            if (SystemType != typeof(string))
                return;
            if (MaxLength == -1)
                return;
            if(!MaxLength.HasValue)
                return;
            result.Add($"[StringLength( {GetSafeMaxLength()}, ErrorMessage = \"{Description?.Replace("\r\n", "")?.Replace(" ", "")}输入过长，不能超过{GetSafeMaxLength()}位\" )]");
        }

        /// <summary>
        /// 获取安全最大长度
        /// </summary>
        private int GetSafeMaxLength()
        {
            if (NativeType == "nvarchar")
                return MaxLength.SafeValue() / 2;
            return MaxLength.SafeValue();
        }

        #endregion

        #region Validate(验证)

        /// <summary>
        /// 验证
        /// </summary>
        public string Validate()
        {
            var result = new StringBuilder();
            List<string> validations = GetValidations();
            if (validations.Count == 1)
                result.Append(validations[0]);
            else
                AddValidations(result, validations);
            return result.ToString();
        }

        /// <summary>
        /// 添加验证列表
        /// </summary>
        /// <param name="result">结果</param>
        /// <param name="validations">验证列表</param>
        private void AddValidations(StringBuilder result, List<string> validations)
        {
            for (var i = 0; i < validations.Count; i++)
            {
                if (i == 0)
                {
                    result.Append($"{validations[i]}\r\n");
                    continue;
                }
                result.Append(
                    i == validations.Count - 1 ? $"        {validations[i]}" : $"        {validations[i]}\r\n");
            }
        }

        #endregion

        #region ShowDescription(设置描述方法)

        /// <summary>
        /// 设置描述方法
        /// </summary>
        public string ShowDescription() => DataType == DbType.Boolean ? ".Description()" : string.Empty;

        #endregion

        #region FormatGridColumn(格式化表格列)

        /// <summary>
        /// 格式化表格列
        /// </summary>
        public string FormatGridColumn()
        {
            if (SystemType == typeof(DateTime))
                return ".FormatDate()";
            if (SystemType == typeof(bool))
                return ".FormatBool()";
            return string.Empty;
        }

        #endregion

        #region GetGridColumnWidth(获取表格列宽)

        /// <summary>
        /// 获取表格列宽
        /// </summary>
        public string GetGridColumnWidth()
        {
            if (SystemType == typeof(DateTime))
                return "120";
            if (SystemType == typeof(bool))
                return "60";
            return "100";
        }

        #endregion

        #region GetDisplayName(获取显示名称)

        /// <summary>
        /// 获取显示名称
        /// </summary>
        public string GetDisplayName()
        {
            if (!string.IsNullOrWhiteSpace(Description))
                return Description;
            return PropertyName;
        }

        #endregion
    }
}
