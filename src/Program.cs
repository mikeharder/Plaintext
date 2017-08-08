using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Plaintext
{
    public class Program
    {
        private static readonly byte[] _helloWorldPayload = Encoding.UTF8.GetBytes("Hello, World!");

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
                .Configure(app => app.Run(WriteResponse))
                .UseUrls("http://*:5000")
                .Build()
                .Run();
        }


        private static Task WriteResponse(HttpContext context)
        {
            var response = context.Response;

            var payloadLength = _helloWorldPayload.Length;
            response.StatusCode = 200;
            response.ContentType = "text/plain";
            response.ContentLength = payloadLength;
            return response.Body.WriteAsync(_helloWorldPayload, 0, payloadLength);
        }

        private static string GetNetCoreVersion()
        {
            var appName = PlatformServices.Default.Application.ApplicationName;
            var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"{appName}.runtimeconfig.json"));
            dynamic runtimeconfig = JObject.Parse(json);
            return runtimeconfig.runtimeOptions.framework.version;
        }
    }
}
