using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Plaintext
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int? threadCount = null;
            if (args.Length >= 1)
            {
                threadCount = int.Parse(args[0]);
            }          

            var netCoreVersion = GetNetCoreVersion();

            Console.WriteLine($".NET Core:\t{netCoreVersion}");
            Console.WriteLine($"ThreadCount:\t{threadCount?.ToString() ?? "null"}\t");
            Console.WriteLine();

            new WebHostBuilder()
#if NETCOREAPP2_0
                .UseKestrel()
                .UseLibuv(options =>
                {
                    if (threadCount.HasValue)
                    {
                        options.ThreadCount = threadCount.Value;
                    }
                })
#elif NETCOREAPP1_1
                .UseKestrel(options =>
                {
                    if (threadCount.HasValue)
                    {
                        options.ThreadCount = threadCount.Value;
                    }
                })
#else
#error Target framework needs to be updated
#endif
                .Configure(app => app.Run(async (context) =>
                {
                    await context.Response.WriteAsync($"Hello from .NET Core {netCoreVersion}");
                }))
                .UseUrls("http://*:5000")
                .Build()
                .Run();
        }

        public static string GetNetCoreVersion()
        {
            var appName = PlatformServices.Default.Application.ApplicationName;
            var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"{appName}.runtimeconfig.json"));
            dynamic runtimeconfig = JObject.Parse(json);
            return runtimeconfig.runtimeOptions.framework.version;
        }
    }
}
