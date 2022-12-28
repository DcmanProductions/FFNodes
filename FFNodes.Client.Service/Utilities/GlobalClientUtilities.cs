// LFInteractive LLC. - All Rights Reserved
global using static FFNodes.Client.Service.Utilities.GlobalClientUtilities;
global using ILogger = Serilog.ILogger;
using FFNodes.Core.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace FFNodes.Client.Service.Utilities;

internal class GlobalClientUtilities
{
    public static GlobalClientUtilities Instance = Instance ??= new();
    public readonly ILogger log;
    public string AudioCodec = "";
    public int ConcurrentProcess = -1;
    public string NodeName = "";
    public string PixelFormat = "";
    public string ServerHost = "";
    public string VideoCodec = "";
    private readonly string _config_file;
    private readonly Logger _logger;

    private GlobalClientUtilities()
    {
        _config_file = Path.Combine(Locations.Data, "client.json");
        log = _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .CreateLogger();

        if (!File.Exists(_config_file))
        {
            Save();
        }
        using (FileStream fs = new(_config_file, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using StreamReader reader = new(fs);
            JObject? json = JObject.Parse(reader.ReadToEnd());
            if (json != null)
            {
                ServerHost = json["ServerHost"]?.ToObject<string>() ?? "";
                NodeName = json["Name"]?.ToObject<string>() ?? "";
                VideoCodec = json["VideoCodec"]?.ToObject<string>() ?? "";
                AudioCodec = json["AudioCodec"]?.ToObject<string>() ?? "";
                PixelFormat = json["PixelFormat"]?.ToObject<string>() ?? "";
                ConcurrentProcess = json["ConcurrentProcess"]?.ToObject<int>() ?? -1;
            }
        }
    }

    ~GlobalClientUtilities()
    {
        _logger?.Dispose();
    }

    public void Save()
    {
        using FileStream fs = new(_config_file, FileMode.Create, FileAccess.Write, FileShare.None);
        using StreamWriter writer = new(fs);
        writer.Write(JsonConvert.SerializeObject(new
        {
            ServerHost,
            Name = NodeName,
            VideoCodec,
            AudioCodec,
            ConcurrentProcess,
            PixelFormat,
        }));
    }
}