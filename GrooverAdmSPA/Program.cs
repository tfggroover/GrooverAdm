using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GrooverAdmSPA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (File.Exists("./appsettings.json"))
            {
                CreateWebHostBuilder(args).Build().Run();
            } else
            {
                throw new ArgumentNullException("No appsettings file found");
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            string port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            string url = String.Concat("http://0.0.0.0:", port);

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("./appsettings.json");
                })
                .UseStartup<Startup>().UseUrls(url);
        }
    }
}
