// LFInteractive LLC. - All Rights Reserved

using Chase.FFmpeg.Converters;
using Chase.FFmpeg.Info;
using FFNodes.Core.Utilities;
using FFNodes.Server.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace FFNodes.Client.Service;

public class Worker : BackgroundService
{
    private readonly ILogger log;
    private Dictionary<string, NodeActiveProcessModel> activeProcesses;
    private List<Process> processes;
    private int working = 0;

    public Worker()
    {
        log = Instance.log;
        activeProcesses = new();
        processes = new();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        AdvancedTimer timer = new(TimeSpan.FromSeconds(5))
        {
            AutoReset = true,
        };
        timer.Elapsed += (s, e) => PingServer(activeProcesses.Values.ToArray());
        while (!stoppingToken.IsCancellationRequested)
        {
            if (working < Instance.ConcurrentProcess)
            {
                Task.Run(() =>
                {
                    working++;
                    GetNextFile();
                    working--;
                }, stoppingToken);
            }
            await Task.Delay((int)TimeSpan.FromSeconds(1).TotalMilliseconds, stoppingToken);
        }
        foreach (Process process in processes)
        {
            if (process != null && !process.HasExited)
            {
                process?.Kill();
            }
        }
    }

    private void GetNextFile()
    {
        string path = "";
        string og_file = "";
        using (HttpClient client = new())
        {
            log.Information("Getting Next File");
            string url = $"{Instance.ServerHost}/api/files/next?node={Uri.EscapeDataString(Instance.NodeName)}";

            string name = "";
            long size = 0;
            using (HttpRequestMessage requestMessage = new(HttpMethod.Head, $"{Instance.ServerHost}/api/files/next"))
            {
                using HttpResponseMessage message = client.Send(requestMessage);
                if (message.IsSuccessStatusCode)
                {
                    name = message.Headers.GetValues("Name").First();
                    og_file = message.Headers.GetValues("Path").First();
                    size = Convert.ToInt64(message.Headers.GetValues("Size").First());
                }
            }
            path = Path.Combine(Locations.Temp, name);
            if (File.Exists(path)) File.Delete(path);
            HttpWebRequest webRequest = WebRequest.CreateHttp(url);
            using WebResponse response = webRequest.GetResponse();
            using FileStream fs = new(path, FileMode.OpenOrCreate, FileAccess.Write);
            using AdvancedTimer update = new(TimeSpan.FromSeconds(1))
            {
                AutoReset = true
            };
            update.Elapsed += (s, e) =>
            {
                long newSize = new FileInfo(path).Length;
                log.Information("{percentage} - {name}", (newSize / (double)size).ToString("p2"), name);
            };
            update.Start();
            response.GetResponseStream().CopyToAsync(fs).Wait();
            fs.Flush();
            fs.Dispose();
            fs.Close();
            update.Stop();
        }
        if (File.Exists(path))
        {
            WorkOnFile(path, og_file);
        }
    }

    private void PingServer(NodeActiveProcessModel[] model)
    {
        try
        {
            using HttpClient client = new();
            HttpRequestMessage request = new(HttpMethod.Post, $"{Instance.ServerHost}/api/nodes?node={Instance.NodeName}");
            List<KeyValuePair<string?, string?>> items = new();
            foreach (NodeActiveProcessModel n in model)
            {
                items.Add(new("NodeActiveProcessModel[]", JsonConvert.SerializeObject(n)));
            }
            request.Content = new FormUrlEncodedContent(items);
            client.Send(request);
        }
        catch (Exception e)
        {
            log.Error("Unable to ping server", e);
        }
    }

    private void UploadFile(string file, string og_file, int attempts = 0)
    {
        log.Information("Uploading file...");
        using HttpClient client = new();
        using HttpRequestMessage message = new(HttpMethod.Post, $"{Instance.ServerHost}/api/files/next?path={og_file}");
        using FileStream fs = new(file, FileMode.Open, FileAccess.Read, FileShare.None);
        message.Content = new StreamContent(fs);
        using HttpResponseMessage response = client.Send(message);
        if (!response.IsSuccessStatusCode)
        {
            log.Error("Failed to upload file");
            if (attempts < 5)
            {
                log.Error("Trying again...");
                Thread.Sleep(5000);
                UploadFile(file, og_file, attempts + 1);
            }
        }
        log.Information("Done uploading...");
    }

    private void WorkOnFile(string file, string og_file)
    {
        log.Information("Working on File: {file}", file);
        FFMediaInfo info = new(file);
        string tmp_file = Path.Combine(Locations.Working, info.Filename);
        Process process = new();
        activeProcesses.Add(file, new()
        {
            FFmpegArgs = "",
            Percent = 0,
            Size = 0,
            Speed = 0
        });
        processes.Add(process);
        process = FFMuxedConverter.SetMedia(info)
            .ChangeHardwareAccelerationMethod()
            .ChangeVideoCodec(Instance.VideoCodec)
            .ChangeAudioCodec(Instance.AudioCodec)
            .ChangePixelFormat(Instance.PixelFormat)
            .OverwriteOriginal()
            .Convert(tmp_file, null, (s, e) =>
            {
                log.Information("{percentage} ({speed}x) -> {name}", e.Percentage.ToString("p2"), e.Speed, info.Filename);
                activeProcesses[file] = new()
                {
                    FFmpegArgs = "",
                    Percent = e.Percentage,
                    Size = new FileInfo(tmp_file).Length,
                    Speed = e.Speed
                };
            });
        try
        {
            processes.Remove(process);
            activeProcesses.Remove(file);
        }
        catch (Exception e)
        {
            log.Error("Unable to remove process: {file}", file, e);
        }
        UploadFile(tmp_file, og_file);
    }
}