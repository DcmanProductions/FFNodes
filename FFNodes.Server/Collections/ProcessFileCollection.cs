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

    private readonly AdvancedTimer _list_sorter;
    private readonly string _locationFile;
    private readonly FileSystemWatcher[] _watchers;
    private string[] _directories;
    private List<string> _files;
    private long _totalSize;
    private bool _update_list;
    private int currentIndex = 0;
    private Serilog.Core.Logger log;

    private ProcessFileCollection()
    {
        _update_list = false;
        _locationFile = Path.Combine(Locations.Data, "locations.json");
        _directories = Array.Empty<string>();
        _files = new();
        _totalSize = 0;
        log = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        if (!File.Exists(_locationFile))
        {
            SetDirectories();
        }
        _watchers = new FileSystemWatcher[_directories.Length];
        for (int i = 0; i < _watchers.Length; i++)
        {
            _watchers[i] = new()
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName,
                Path = _directories[i]
            };
            _watchers[i].Created += (s, e) =>
            {
                string file = e.FullPath;
                if (FSUtilities.IsFile(file))
                {
                    _files.Add(file);
                    _update_list = true;
                }
            };
            _watchers[i].Deleted += (s, e) =>
            {
                string file = e.FullPath;
                if (FSUtilities.IsFile(file))
                {
                    _files.Remove(file);
                    _update_list = true;
                }
            };
        }

        _list_sorter = new(TimeSpan.FromMinutes(5))
        {
            AutoReset = false,
        };
        _list_sorter.Elapsed += (s, e) =>
        {
            if (_update_list)
            {
                Sort();
                _update_list = false;
            }
            _list_sorter.Start();
        };
    }

    ~ProcessFileCollection()
    {
        log.Warning("Disposing of {0}", typeof(ProcessFileCollection).Name);
        _list_sorter.Stop();
        _list_sorter.Close();
        foreach (FileSystemWatcher watcher in _watchers)
        {
            watcher?.Dispose();
        }
        log.Dispose();
    }

    public long Size => _totalSize;

    public string[] GetDirectories() => _directories;

    public string[] GetFiles() => _files.ToArray();

    public string GetNextFile(bool increment = true)
    {
        string file = _files[currentIndex];
        if (increment)
            currentIndex++;
        return file;
    }

    public void Refresh()
    {
        log.Information("Refreshing {0}", typeof(ProcessFileCollection).Name);
        try
        {
            using (FileStream fs = new(_locationFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            {
                using StreamReader reader = new(fs);
                try
                {
                    string[]? arr = JArray.Parse(reader.ReadToEnd()).ToObject<string[]>();
                    if (arr != null)
                    {
                        _directories = arr.Where(i => Directory.Exists(i)).ToArray();
                    }
                }
                catch
                {
                    _directories = Array.Empty<string>();
                }
            }
        }
        catch (Exception e)
        {
            log.Error("Unable to read directories from file!", e);
        }
        if (_directories.Any())
        {
            List<string> files = new();

            foreach (string dir in _directories)
            {
                try
                {
                    ICollection<string> items = FFVideoUtility.GetFiles(dir, true);
                    files.AddRange(items);
                }
                catch (Exception e)
                {
                    log.Error("Unable to get files from directory: {directory}", dir, e);
                }
            }
            _files = files;
            Sort();
        }
        log.Information("Done refreshing {0}: {1} files found", typeof(ProcessFileCollection).Name, _files.Count);
    }

    public void RemoveDirectories(params string[] directories)
    {
        List<string> dirs = _directories.ToList();
        foreach (string directory in directories)
        {
            dirs.Remove(directory);
        }
        SetDirectories(dirs.ToArray());
    }

    public void SetDirectories(params string[] directories)
    {
        log.Information("Setting directories to: \"{0}\"", string.Join(',', directories));
        using (FileStream fs = new(_locationFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
        {
            using StreamWriter writer = new(fs);
            writer.Write(JsonConvert.SerializeObject(directories, Formatting.Indented));
        }
        Refresh();
    }

    private void Sort()
    {
        try
        {
            _files = _files.OrderByDescending(i =>
            {
                FileInfo info = new(i);
                long size = info.Length;
                _totalSize += size;
                return size;
            }).ToList();
        }
        catch (Exception e)
        {
            log.Error("Unable to sort files array by file size", e);
        }
    }
}