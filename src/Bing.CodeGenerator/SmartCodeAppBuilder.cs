using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartCode.App;

namespace Bing.CodeGenerator;

/// <summary>
/// SmartCode应用程序构建器
/// </summary>
public class SmartCodeAppBuilder
{
    /// <summary>
    /// 应用设置路径
    /// </summary>
    private const string AppSettingsPath = "appsettings.json";

    /// <summary>
    /// SmartCode键名
    /// </summary>
    private const string SmartCodeKey = "SmartCode";

    /// <summary>
    /// 应用程序目录
    /// </summary>
    public string AppDirectory => AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// 构建
    /// </summary>
    /// <param name="configPath">配置路径</param>
    public SmartCodeApp Build(string configPath)
    {
        var appSettingsBuilder = new ConfigurationBuilder()
            .SetBasePath(AppDirectory)
            .AddJsonFile(AppSettingsPath, false, true);
        var configuration = appSettingsBuilder.Build();
        var smartCodeOptions = configuration.GetSection(SmartCodeKey).Get<SmartCodeOptions>();
        smartCodeOptions.ConfigPath = configPath;
        smartCodeOptions.Services.AddLogging((loggerBuilder) =>
        {
            var loggingConfig = configuration.GetSection("Logging");
            loggerBuilder.AddConfiguration(loggingConfig).AddConsole();
        });
        return new SmartCodeApp(smartCodeOptions);
    }
}