// LFInteractive LLC. - All Rights Reserved
global using static FFNodes.Client.Service.Utilities.GlobalClientUtilities;
global using ILogger = Serilog.ILogger;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace FFNodes.Client.Service.Utilities;

internal class GlobalClientUtilities
{
    public static GlobalClientUtilities Instance = Instance ??= new();
    public readonly ILogger log;
    private readonly Logger _logger;

    private GlobalClientUtilities()
    {
        log = _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .CreateLogger();
    }

    ~GlobalClientUtilities()
    {
        _logger?.Dispose();
    }
}