using System.Data;

namespace Bing.CodeGenerator.Helpers;

/// <summary>
/// 类型转换器
/// </summary>
internal static class TypeConverter
{
    /// <summary>
    /// 解析类型
    /// </summary>
    /// <param name="systemType">系统类型</param>
    /// <param name="isNullable">是否可空</param>
    public static Type ParseType(string systemType, bool isNullable)
    {
        var currentType = systemType;
        //if (isNullable && !systemType.Contains("[]") && systemType.ToLower() != "string")
        //{
        //    currentType = $"{currentType}?";
        //}
        switch (currentType)
        {
            case "byte[]":
                return typeof(byte[]);
            case "string":
                return typeof(string);
            case "Guid":
                return typeof(Guid);
            case "Guid?":
                return typeof(Guid?);
            case "DateTime":
                return typeof(DateTime);
            case "DateTime?":
                return typeof(DateTime?);
            case "TimeSpan":
                return typeof(TimeSpan);
            case "TimeSpan?":
                return typeof(TimeSpan?);
            case "DateTimeOffset":
                return typeof(DateTimeOffset);
            case "DateTimeOffset?":
                return typeof(DateTimeOffset?);
            case "byte":
                return typeof(byte);
            case "byte?":
                return typeof(byte?);
            case "short":
                return typeof(short);
            case "short?":
                return typeof(short?);
            case "int":
                return typeof(int);
            case "int?":
                return typeof(int?);
            case "long":
                return typeof(long);
            case "long?":
                return typeof(long?);
            case "bool":
                return typeof(bool);
            case "bool?":
                return typeof(bool?);
            case "decimal":
                return typeof(decimal);
            case "decimal?":
                return typeof(decimal?);
            case "float":
                return typeof(float);
            case "float?":
                return typeof(float?);
            case "double":
                return typeof(double);
            case "double?":
                return typeof(double?);
            default:
                throw new NotImplementedException($"未知系统类型: {systemType}{(isNullable ? "?" : string.Empty)}");
        }
    }

    /// <summary>
    /// 类型映射
    /// </summary>
    private static readonly IDictionary<Type, DbType> TypeMap = new Dictionary<Type, DbType>()
    {
        {typeof(byte), DbType.Byte},
        {typeof(sbyte), DbType.SByte},
        {typeof(short), DbType.Int16},
        {typeof(ushort), DbType.UInt16},
        {typeof(int), DbType.Int32},
        {typeof(uint), DbType.UInt32},
        {typeof(long), DbType.Int64},
        {typeof(ulong), DbType.UInt64},
        {typeof(float), DbType.Single},
        {typeof(double), DbType.Double},
        {typeof(decimal), DbType.Decimal},
        {typeof(bool), DbType.Boolean},
        {typeof(string), DbType.String},
        {typeof(char), DbType.StringFixedLength},
        {typeof(Guid), DbType.Guid},
        {typeof(DateTime), DbType.DateTime},
        {typeof(DateTimeOffset), DbType.DateTimeOffset},
        {typeof(TimeSpan), DbType.Time},
        {typeof(byte[]), DbType.Binary},
        {typeof(byte?), DbType.Byte},
        {typeof(sbyte?), DbType.SByte},
        {typeof(short?), DbType.Int16},
        {typeof(ushort?), DbType.UInt16},
        {typeof(int?), DbType.Int32},
        {typeof(uint?), DbType.UInt32},
        {typeof(long?), DbType.Int64},
        {typeof(ulong?), DbType.UInt64},
        {typeof(float?), DbType.Single},
        {typeof(double?), DbType.Double},
        {typeof(decimal?), DbType.Decimal},
        {typeof(bool?), DbType.Boolean},
        {typeof(char?), DbType.StringFixedLength},
        {typeof(Guid?), DbType.Guid},
        {typeof(DateTime?), DbType.DateTime},
        {typeof(DateTimeOffset?), DbType.DateTimeOffset},
        {typeof(TimeSpan?), DbType.Time},
        {typeof(object), DbType.Object},
    };

    /// <summary>
    /// 解析数据库类型
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="isNullable">是否可空</param>
    public static DbType ParseDbType(string type, bool isNullable)
    {
        var currentType = ParseType(type, isNullable);
        if (TypeMap.ContainsKey(currentType))
            return TypeMap[currentType];
        throw new NotImplementedException($"未知类型: {type}{(isNullable ? "?" : string.Empty)}");
    }
}