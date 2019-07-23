using System.Threading.Tasks;
using SmartCode.Utilities;

namespace Bing.CodeGenerator.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var buildSettings = "BingGenerateConfig.yml";
            var app = new SmartCodeAppBuilder().Build(AppPath.Relative(buildSettings));
            System.Console.WriteLine("开始执行构建程序");
            await app.Run();
            System.Console.WriteLine("来源隔壁老萌的新手大礼包");
            System.Console.ReadLine();
        }
    }
}
