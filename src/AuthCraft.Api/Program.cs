using System;
using System.IO;
using System.Net;
using Easy.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AuthCraft.Api;

/// <summary>
/// Program entry point. Trigger change
/// </summary>
public class Program
{
    private static readonly string HostPort = System.Environment.GetEnvironmentVariable("HOST_PORT");
    private static readonly string Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    private static IConfigurationRoot Configuration { get; } =
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.json", false, true)
            .AddJsonFile($"appsettings.{Environment}.json", true, true)
            .Build();

    public static void Main()
    {
        var nlogLoggingConfiguration = new NLogLoggingConfiguration(Configuration.GetSection("nlog"));
        var logger = NLogBuilder.ConfigureNLog(nlogLoggingConfiguration).GetCurrentClassLogger();

        try
        {
            var host = BuildWebHost(logger);

            var report = DiagnosticReport.Generate();
            logger.Info(report);

            host.Run();
        }
        catch (Exception ex)
        {
            logger.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            logger.Factory.Flush();
        }
    }

    private static IWebHost BuildWebHost(Logger logger) =>
        new WebHostBuilder()
            .UseKestrel(options =>
            {
                var port = int.Parse(HostPort);
                options.Listen(IPAddress.Any, port);
            })
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddEnvironmentVariables();
                builder.AddConfiguration(Configuration);
            })
            .ConfigureLogging((context, builder) =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                LogManager.Configuration = new NLogLoggingConfiguration(Configuration.GetSection("nlog"));
            })
            .UseStartup<Startup>()
            .UseNLog()
            .Build();
}
