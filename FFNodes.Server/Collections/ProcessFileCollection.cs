// LFInteractive LLC. - All Rights Reserved
using Chase.FFmpeg.Extra;
using FFNodes.Core.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace FFNodes.Server.Collections;

public sealed class ProcessFileCollection
{
    public static ProcessFileCollection Instance = Instance ??= new();

    private readonly string _locationFile;
    private string[] _directories;
    private string[] _files;
    private long _totalSize;
    private int currentIndex = 0;
    private Serilog.Core.Logger log;

    private ProcessFileCollection()
    {
        _locationFile = Path.Combine(Locations.Data, "locations.json");
        _directories = Array.Empty<string>();
        _files = Array.Empty<string>();
        _totalSize = 0;
        log = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        if (!File.Exists(_locationFile))
        {
            SetDirectories();
        }
    }

    ~ProcessFileCollection()
    {
        log.Warning("Disposing of {0}", typeof(ProcessFileCollection).Name);
        log.Dispose();
    }

    public long Size => _totalSize;

    public string GetNextFile()
    {
        string file = _files[currentIndex];
        currentIndex++;
        return file;
    }

    public void Refresh()
    {
        log.Information("Refreshing {0}", typeof(ProcessFileCollection).Name);
        using (FileStream fs = new(_locationFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
        {
            using StreamReader reader = new(fs);
            try
            {
                string[]? arr = JObject.Parse(reader.ReadToEnd()).ToObject<string[]>();
                if (arr != null)
                {
                    _directories = arr;
                }
            }
            catch
            {
                _directories = Array.Empty<string>();
            }
        }
        if (_directories.Any())
        {
            List<string> files = new();

            foreach (string dir in _directories)
            {
                ICollection<string> items = FFVideoUtility.GetFiles(dir, true);
                files.AddRange(items);
            }

            _files = files.OrderBy(i =>
            {
                FileInfo info = new(i);
                long size = info.Length;
                _totalSize += size;
                return size;
            }).ToArray();
        }
        log.Information("Done refreshing {0}: {1} files found", typeof(ProcessFileCollection).Name, _files.Length);
    }

    public void SetDirectories(params string[] directories)
    {
        log.Information("Setting directories to: \"{0}\"", string.Join(',', directories));
        using FileStream fs = new(_locationFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        using StreamWriter writer = new(fs);
        writer.Write(JsonConvert.SerializeObject(directories, Formatting.Indented));
        Refresh();
    }
}