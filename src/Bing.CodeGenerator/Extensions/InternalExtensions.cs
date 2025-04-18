using Bing.CodeGenerator.Helpers;

namespace Bing.CodeGenerator.Extensions;

/// <summary>
/// 内部扩展
/// </summary>
public static class InternalExtensions
{
    /// <summary>
    /// C#类型别名
    /// </summary>
    private static readonly Dictionary<string, string> CsharpTypeAlias;

    /// <summary>
    /// C#关键字
    /// </summary>
    private static readonly HashSet<string> CsharpKeywords;

    /// <summary>
    /// 初始化一个<see cref="InternalExtensions"/>类型的静态实例
    /// </summary>
    static InternalExtensions()
    {
        CsharpKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "as", "do", "if", "in", "is",
            "for", "int", "new", "out", "ref", "try",
            "base", "bool", "byte", "case", "char", "else", "enum", "goto", "lock", "long", "null", "this", "true", "uint", "void",
            "break", "catch", "class", "const", "event", "false", "fixed", "float", "sbyte", "short", "throw", "ulong", "using", "while",
            "double", "extern", "object", "params", "public", "return", "sealed", "sizeof", "static", "string", "struct", "switch", "typeof", "unsafe", "ushort",
            "checked", "decimal", "default", "finally", "foreach", "private", "virtual",
            "abstract", "continue", "delegate", "explicit", "implicit", "internal", "operator", "override", "readonly", "volatile",
            "__arglist", "__makeref", "__reftype", "interface", "namespace", "protected", "unchecked",
            "__refvalue", "stackalloc"
        };

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

    /// <summary>
    /// 获取唯一名称
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="exists">是否存在名称</param>
    public static string MakeUnique(this string name, Func<string, bool> exists)
    {
        string uniqueName = name;
        var count = 1;
        while (exists(uniqueName))
            uniqueName = string.Concat(name, count++);
        return uniqueName;
    }

    /// <summary>
    /// 转换为安全名称
    /// </summary>
    /// <param name="name">名称</param>
    public static string ToSafeName(this string name)
    {
        if (!name.IsKeyword())
            return name;
        return $"@{name}";
    }

    /// <summary>
    /// 是否关键词
    /// </summary>
    /// <param name="text">文本</param>
    public static bool IsKeyword(this string text) => CsharpKeywords.Contains(text);

    /// <summary>
    /// 转换为CamelCase(小驼峰式)字符串
    /// </summary>
    /// <example>
    ///     <code>Str.ToCamelCase("FOOBAR")  // "foobar"</code>
    ///     <code>Str.ToCamelCase("FOO_BAR") // "fooBar"</code>
    ///     <code>Str.ToCamelCase("FooBar")  // "fooBar"</code>
    ///     <code>Str.ToCamelCase("foo bar") // "fooBar"</code>
    /// </example>
    /// <param name="value">值</param>
    public static string ToCamelCase(this string value) => Str.ToCamelCase(value);

    /// <summary>
    /// 转换为字段名
    /// </summary>
    /// <param name="value">值</param>
    public static string ToFieldName(this string value) => $"_{Str.ToCamelCase(value)}";
}