// LFInteractive LLC. - All Rights Reserved
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Text.Json;

namespace FFNodes.Core.Utilities;

public static class FSUtilities
{
    public static bool IsDirectory(string path) => new FileInfo(path).Attributes.HasFlag(FileAttributes.Directory);

    public static bool IsFile(string path) => !IsDirectory(path);

    public static T ReadFromFile<T>(string path)
    {
        if (File.Exists(path))
        {
            string json = "";
            using (FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using GZipStream compressed = new(fs, CompressionMode.Decompress);
                using StreamReader reader = new(compressed);
                json = reader.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(json))
            {
                T? item = JObject.Parse(json).ToObject<T>();
                if (item == null)
                    throw new InvalidCastException($"Unable to cast file to {typeof(T).Name}");
                return item;
            }
        }
        return default;
    }

    public static string WriteToFile(string directory, string name, object content)
    {
        string path = Path.Combine(directory, name);
        using FileStream fs = new(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        using GZipStream compressed = new(fs, CompressionLevel.Fastest);
        using StreamWriter writer = new(compressed);
        writer.Write(JsonSerializer.Serialize(content));
        return path;
    }
}