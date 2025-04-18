using System.Text;
using System.Text.RegularExpressions;

namespace Bing.CodeGenerator.Helpers;

/// <summary>
/// 字符串操作
/// </summary>
internal static class Str
{
    /// <summary>
    /// 单词正则表达式
    /// </summary>
    private static readonly Regex WordsRegex = new Regex(@"[A-Z\xc0-\xd6\xd8-\xde]?[a-z\xdf-\xf6\xf8-\xff]+(?:['’](?:d|ll|m|re|s|t|ve))?(?=[\xac\xb1\xd7\xf7\x00-\x2f\x3a-\x40\x5b-\x60\x7b-\xbf\u2000-\u206f \t\x0b\f\xa0\ufeff\n\r\u2028\u2029\u1680\u180e\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u202f\u205f\u3000]|[A-Z\xc0-\xd6\xd8-\xde]|$)|(?:[A-Z\xc0-\xd6\xd8-\xde]|[^\ud800-\udfff\xac\xb1\xd7\xf7\x00-\x2f\x3a-\x40\x5b-\x60\x7b-\xbf\u2000-\u206f \t\x0b\f\xa0\ufeff\n\r\u2028\u2029\u1680\u180e\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u202f\u205f\u3000\d+\u2700-\u27bfa-z\xdf-\xf6\xf8-\xffA-Z\xc0-\xd6\xd8-\xde])+(?:['’](?:D|LL|M|RE|S|T|VE))?(?=[\xac\xb1\xd7\xf7\x00-\x2f\x3a-\x40\x5b-\x60\x7b-\xbf\u2000-\u206f \t\x0b\f\xa0\ufeff\n\r\u2028\u2029\u1680\u180e\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u202f\u205f\u3000]|[A-Z\xc0-\xd6\xd8-\xde](?:[a-z\xdf-\xf6\xf8-\xff]|[^\ud800-\udfff\xac\xb1\xd7\xf7\x00-\x2f\x3a-\x40\x5b-\x60\x7b-\xbf\u2000-\u206f \t\x0b\f\xa0\ufeff\n\r\u2028\u2029\u1680\u180e\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u202f\u205f\u3000\d+\u2700-\u27bfa-z\xdf-\xf6\xf8-\xffA-Z\xc0-\xd6\xd8-\xde])|$)|[A-Z\xc0-\xd6\xd8-\xde]?(?:[a-z\xdf-\xf6\xf8-\xff]|[^\ud800-\udfff\xac\xb1\xd7\xf7\x00-\x2f\x3a-\x40\x5b-\x60\x7b-\xbf\u2000-\u206f \t\x0b\f\xa0\ufeff\n\r\u2028\u2029\u1680\u180e\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u202f\u205f\u3000\d+\u2700-\u27bfa-z\xdf-\xf6\xf8-\xffA-Z\xc0-\xd6\xd8-\xde])+(?:['’](?:d|ll|m|re|s|t|ve))?|[A-Z\xc0-\xd6\xd8-\xde]+(?:['’](?:D|LL|M|RE|S|T|VE))?|\d+|(?:[\u2700-\u27bf]|(?:\ud83c[\udde6-\uddff]){2}|[\ud800-\udbff][\udc00-\udfff])[\ufe0e\ufe0f]?(?:[\u0300-\u036f\ufe20-\ufe23\u20d0-\u20f0]|\ud83c[\udffb-\udfff])?(?:\u200d(?:[^\ud800-\udfff]|(?:\ud83c[\udde6-\uddff]){2}|[\ud800-\udbff][\udc00-\udfff])[\ufe0e\ufe0f]?(?:[\u0300-\u036f\ufe20-\ufe23\u20d0-\u20f0]|\ud83c[\udffb-\udfff])?)*");

    /// <summary>
    /// 缩进正则表达式
    /// </summary>
    private static readonly Regex IndentRegex = new Regex(@"^[ \t]*(?=\S)", RegexOptions.Multiline);

    /// <summary>
    /// 移除缩进
    /// </summary>
    /// <param name="value">值</param>
    public static string StripIndent(string value)
    {
        var indent = IndentRegex.Matches(value).Cast<Match>().Select(m => m.Value.Length).Min();
        return new Regex($@"^[ \t]{{{indent}}}", RegexOptions.Multiline).Replace(value, "");
    }

    /// <summary>
    /// 转换为单词集合
    /// </summary>
    /// <param name="value">值</param>
    public static IEnumerable<string> ToWords(string value)
    {
        foreach (Match match in WordsRegex.Matches(value))
            yield return match.Value;
    }

    /// <summary>
    /// 转换为首字母大写字符串
    /// </summary>
    /// <param name="value">值</param>
    public static string ToUpperFirst(string value) => ChangeCaseFirst(value, c => c.ToUpperInvariant());

    /// <summary>
    /// 转换为首字母小写字符串
    /// </summary>
    /// <param name="value">值</param>
    public static string ToLowerFirst(string value) => ChangeCaseFirst(value, c => c.ToLowerInvariant());

    /// <summary>
    /// 首字母大写，其余小写字符串
    /// </summary>
    /// <param name="value">值</param>
    public static string Capitalize(string value) => ToUpperFirst(value.ToLowerInvariant());

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
    public static string ToCamelCase(string value) => ChangeCase(value,
        (word, index) => (index == 0 ? word.ToLowerInvariant() : Capitalize(word)));

    /// <summary>
    /// 转换为常量字符串
    /// </summary>
    /// <example>
    ///     <code>Str.ToConstantCase("fOo BaR") // "FOO_BAR"</code>
    ///     <code>Str.ToConstantCase("FooBar")  // "FOO_BAR"</code>
    ///     <code>Str.ToConstantCase("Foo Bar") // "FOO_BAR"</code>
    /// </example>
    /// <param name="value">值</param>
    public static string ToConstantCase(string value) => ChangeCase(value, "_", w => w.ToUpperInvariant());

    /// <summary>
    /// 转换为大写字符串
    /// </summary>
    /// <example>
    ///     <code>Str.ToUpperCase("foobar")  // "FOOBAR"</code>
    ///     <code>Str.ToUpperCase("FOO_BAR") // "FOO BAR"</code>
    ///     <code>Str.ToUpperCase("FooBar")  // "FOO BAR"</code>
    ///     <code>Str.ToUpperCase("Foo Bar") // "FOO BAR"</code>
    /// </example>
    /// <param name="value">值</param>
    public static string ToUpperCase(string value) => ChangeCase(value, " ", (word) => word.ToUpperInvariant());

    /// <summary>
    /// 转换为小写字符串
    /// </summary>
    /// <example>
    ///     <code>Str.ToLowerCase("FOOBAR")  // "foobar"</code>
    ///     <code>Str.ToLowerCase("FOO_BAR") // "foo bar"</code>
    ///     <code>Str.ToLowerCase("FooBar")  // "foo bar"</code>
    ///     <code>Str.ToLowerCase("Foo Bar") // "foo bar"</code>
    /// </example>
    /// <param name="value">值</param>
    public static string ToLowerCase(string value) => ChangeCase(value, " ", word => word.ToLowerInvariant());

    /// <summary>
    /// 转换为PascalCase(大驼峰式)字符串
    /// </summary>
    /// <example>
    ///     <code>Str.ToPascalCase("FOOBAR")  // "FooBar"</code>
    ///     <code>Str.ToPascalCase("FOO_BAR") // "FooBar"</code>
    ///     <code>Str.ToPascalCase("fooBar")  // "FooBar"</code>
    ///     <code>Str.ToPascalCase("Foo Bar") // "FooBar"</code>
    /// </example>
    /// <param name="value">值</param>
    public static string ToPascalCase(string value) => ChangeCase(value, Capitalize);

    /// <summary>
    /// 转换为KebabCase(烤肉串式)字符串
    /// </summary>
    /// <example>
    ///     <code>Str.ToKebabCase("FOOBAR")  // "foo-bar"</code>
    ///     <code>Str.ToKebabCase("FOO_BAR") // "foo-bar"</code>
    ///     <code>Str.ToKebabCase("fooBar")  // "foo-bar"</code>
    ///     <code>Str.ToKebabCase("Foo Bar") // "foo-bar"</code>
    /// </example>
    /// <param name="value">值</param>
    public static string ToKebabCase(string value) => ChangeCase(value, "-", word => word.ToLowerInvariant());

    /// <summary>
    /// 转换为SnakeCase(蛇式)字符串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToSnakeCase(string value) => ChangeCase(value, "_", word => word.ToLowerInvariant());

    /// <summary>
    /// 变更大小写
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="composer">比较器</param>
    public static string ChangeCase(string value, Func<string, string> composer) => ChangeCase(value, "", composer);

    /// <summary>
    /// 变更大小写
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="separator">分隔符</param>
    /// <param name="composer">比较器</param>
    public static string ChangeCase(string value, string separator, Func<string, string> composer) => ChangeCase(value, separator, (w, i) => composer(w));

    /// <summary>
    /// 变更大小写
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="composer">比较器</param>
    public static string ChangeCase(string value, Func<string, int, string> composer) => ChangeCase(value, "", composer);

    /// <summary>
    /// 变更大小写
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="separator">分隔符</param>
    /// <param name="composer">比较器</param>
    public static string ChangeCase(string value, string separator, Func<string, int, string> composer)
    {
        var sb = new StringBuilder();
        var index = 0;
        foreach (var word in ToWords(value))
        {
            sb.Append((index == 0 ? "" : separator));
            sb.Append(composer(word, index++));
        }

        return sb.ToString();
    }

    /// <summary>
    /// 变更首字母大小写
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="change">变更</param>
    private static string ChangeCaseFirst(string value, Func<string, string> change) => change(value.Substring(0, 1)) + value.Substring(1);
}