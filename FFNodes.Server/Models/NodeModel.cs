// LFInteractive LLC. - All Rights Reserved
using FFNodes.Core.Utilities;
using System.IO.Compression;
using System.Text.Json;

namespace FFNodes.Server.Models;

/// <summary>
/// A node is a remote client working on ffmpeg process.
/// </summary>
public struct NodeModel
{
    public List<NodeActiveProcessModel> ActiveProcesses { get; set; }

    /// <summary>
    /// The number of bytes per second the node can recieve from the server.
    /// </summary>
    public double AverageDownloadSpeed { get; set; }

    /// <summary>
    /// The average video duration that the node has processed.
    /// </summary>
    public double AverageDuration { get; set; }

    /// <summary>
    /// The average number of bytes the node has saved!
    /// </summary>
    public double AverageSaved { get; set; }

    /// <summary>
    /// The average speed that the node can process each item.
    /// </summary>
    public double AverageSpeed { get; set; }

    /// <summary>
    /// The number of bytes per second that the node can send to the server.
    /// </summary>
    public double AverageUploadSpeed { get; set; }

    /// <summary>
    /// The average amount of time the node is running for.
    /// </summary>
    public double AverageUptime { get; set; }

    /// <summary>
    /// The average time that the node speeds on a single task.
    /// </summary>
    public double AverageWorkTime { get; set; }

    /// <summary>
    /// A list of completed <seealso cref="ProcessModel">Processes</seealso>
    /// </summary>
    public List<ProcessModel> CompletedProcesses { get; set; }

    public Guid ID { get; set; }

    /// <summary>
    /// If the node is currently connected and working on a file.
    /// </summary>
    public bool IsCurrentlyActive { get; set; }

    /// <summary>
    /// The name of the node.
    /// </summary>
    public string Name { get; init; }

    public void Ping(NodeActiveProcessModel[] processes)
    {
    }

    /// <summary>
    /// Saves the node to file as json.
    /// </summary>
    public void Save()
    {
        using FileStream fs = new(Path.Combine(Locations.NodeData, ID.ToString()), FileMode.Create, FileAccess.Write, FileShare.Read);
        using GZipStream compressed = new(fs, CompressionLevel.Fastest);
        using StreamWriter writer = new(compressed);
        writer.Write(JsonSerializer.Serialize(this));
    }
}