using System;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Linq;

namespace Plaintext
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(".NET Core " + GetNetCoreVersion());
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
