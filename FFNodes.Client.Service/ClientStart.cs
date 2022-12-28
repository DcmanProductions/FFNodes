// LFInteractive LLC. - All Rights Reserved
using Chase.FFmpeg.Downloader;
using FFNodes.Core.Utilities;
using Serilog;

namespace FFNodes.Client.Service;

public class ClientStart
{
    public static void Main(string[] args)
    {
        ILogger log = Instance.log;
        if (string.IsNullOrEmpty(Instance.NodeName))
        {
            Console.Write("Enter Node Name: ");
            string? line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                log.Error("Name can NOT be null or empty");
                Main(args);
                return;
            }
            Instance.NodeName = line;
            Instance.Save();
        }
        if (string.IsNullOrEmpty(Instance.ServerHost))
        {
            Console.Write("Enter Server Host (http://127.0.0.1): ");
            string? line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                log.Error("Host can NOT be null or empty");
                Main(args);
                return;
            }
            Instance.ServerHost = $"{line}:{ApplicationData.ServerAPIPort}";
            Instance.Save();
        }
        if (string.IsNullOrEmpty(Instance.VideoCodec))
        {
            Console.Write("Enter Video Codec: ");
            string? line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                log.Error("Codec can NOT be null or empty");
                Main(args);
                return;
            }
            Instance.VideoCodec = line;
            Instance.Save();
        }
        if (string.IsNullOrEmpty(Instance.AudioCodec))
        {
            Console.Write("Enter Audio Codec: ");
            string? line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                log.Error("Codec can NOT be null or empty");
                Main(args);
                return;
            }
            Instance.AudioCodec = line;
            Instance.Save();
        }
        if (string.IsNullOrEmpty(Instance.PixelFormat))
        {
            Console.Write("Enter Pixel Format: ");
            string? line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                log.Error("Pixel Format can NOT be null or empty");
                Main(args);
                return;
            }
            Instance.PixelFormat = line;
            Instance.Save();
        }
        if (Instance.ConcurrentProcess == -1)
        {
            Console.Write("Enter Concurrent Processes: ");
            string? line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line) || !int.TryParse(line, out int concurrent))
            {
                log.Error("Concurrent Processes can NOT be null or empty");
                Main(args);
                return;
            }
            Instance.ConcurrentProcess = concurrent;
            Instance.Save();
        }

        try
        {
            using HttpClient c = new();
            using HttpResponseMessage r = c.GetAsync($"{Instance.ServerHost}/api").Result;
            if (!r.IsSuccessStatusCode)
            {
                log.Error($"Server Host did not respond: {Instance.ServerHost}/api");
                return;
            }
        }
        catch
        {
            log.Error($"Server Host did not respond: {Instance.ServerHost}/api");
            return;
        }

        using HttpClient client = new();
        HttpRequestMessage request = new(HttpMethod.Put, $"{Instance.ServerHost}/api/nodes?name={Instance.NodeName}");
        using HttpResponseMessage response = client.Send(request);
        if (response.IsSuccessStatusCode)
        {
            log.Information("Registered Node: {name}", Instance.NodeName);
        }

        log.Information("Checking FFmpeg...");
        FFmpegDownloader.Instance.GetLatest(Directory.CreateDirectory(Path.Combine(Locations.Root, "ffmpeg")).FullName).Wait();

        log.Information("Service is Starting!\nNode Name: {name}\nServer Host: {host}\nVideo Codec: {video}\nAudio Codec: {audio}\nPixel Format: {pixfmt}\nConcurrent Processes: {concurrent}", Instance.NodeName, Instance.ServerHost, Instance.VideoCodec, Instance.AudioCodec, Instance.PixelFormat, Instance.ConcurrentProcess);
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<Worker>();
            })
            .UseSerilog()
            .UseWindowsService()
            .Build();

        host.Run();
    }
}