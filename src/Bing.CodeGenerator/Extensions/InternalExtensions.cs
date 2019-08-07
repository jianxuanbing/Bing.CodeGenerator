using System;
using System.Collections.Generic;

namespace Bing.CodeGenerator.Extensions
{
    /// <summary>
    /// 内部扩展
    /// </summary>
    internal static class InternalExtensions
    {
        /// <summary>
        /// C#类型别名
        /// </summary>
        private static readonly Dictionary<string, string> CsharpTypeAlias;

        /// <summary>
        /// 初始化一个<see cref="InternalExtensions"/>类型的静态实例
        /// </summary>
        static InternalExtensions()
        {
            CsharpTypeAlias = new Dictionary<string, string>(16)
            {
                {"System.Int16", "short"},
                {"System.Int32", "int"},
                {"System.Int64", "long"},
                {"System.String", "string"},
                {"System.Object", "object"},
                {"System.Boolean", "bool"},
                {"System.Void", "void"},
                {"System.Char", "char"},
                {"System.Byte", "byte"},
                {"System.UInt16", "ushort"},
                {"System.UInt32", "uint"},
                {"System.UInt64", "ulong"},
                {"System.SByte", "sbyte"},
                {"System.Single", "float"},
                {"System.Double", "double"},
                {"System.Decimal", "decimal"}
            };
        }

        /// <summary>
        /// 安全返回值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">可空值</param>
        public static T SafeValue<T>(this T? value) where T : struct => value ?? default(T);

        /// <summary>
        /// 转换为可空类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="isNullable">是否为空</param>
        public static string ToNullableType(this Type type, bool isNullable = false) => ToNullableType(type.FullName, isNullable);

        /// <summary>
        /// 转换为可空类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="isNullable">是否为空</param>
        public static string ToNullableType(this string type, bool isNullable = false)
        {
            bool isValueType = type.IsValueType();
            type = type.ToType();
            type = type.Replace("System.", "");

            if (!isValueType || !isNullable)
                return type;
            return type + "?";
        }

        /// <summary>
        /// 是否值类型
        /// </summary>
        /// <param name="type">类型</param>
        public static bool IsValueType(this string type)
        {
            if (!type.StartsWith("System."))
                return false;
            var t = Type.GetType(type, false);
            return t != null && t.IsValueType;
        }

        /// <summary>
        /// 转换为类型
        /// </summary>
        /// <param name="type">类型</param>
        public static string ToType(this Type type) => ToType(type.FullName);

        /// <summary>
        /// 转换为类型
        /// </summary>
        /// <param name="type">类型</param>
        public static string ToType(this string type)
        {
            if (type == "System.Xml.XmlDocument")
                type = "System.String";
            if (CsharpTypeAlias.TryGetValue(type, out var t))
                return t;
            return type;
        }
    }
}