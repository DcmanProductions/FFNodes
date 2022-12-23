// LFInteractive LLC. - All Rights Reserved
using FFNodes.Core.Utilities;

namespace FFNodes.Server.Models;

/// <summary>
/// A node is a remote client working on ffmpeg process.
/// </summary>
public struct NodeModel
{
    private AdvancedTimer _inactive_timeout;
    public NodeActiveProcessModel[] ActiveProcesses { get; set; }

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
    public bool IsCurrentlyActive { get; private set; }

    /// <summary>
    /// The name of the node.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// </summary>
    /// <param name="processes"></param>
    public void Ping(NodeActiveProcessModel[] processes)
    {
        if (_inactive_timeout == null)
        {
            _inactive_timeout = new(TimeSpan.FromSeconds(10))
            {
                Interuptable = true,
            };
            NodeModel current = this;
            _inactive_timeout.Elapsed += (s, e) =>
            {
                current.IsCurrentlyActive = false;
                current.ActiveProcesses = Array.Empty<NodeActiveProcessModel>();
            };
        }
        IsCurrentlyActive = true;
        _inactive_timeout.Start();
        ActiveProcesses = processes;
    }

    /// <summary>
    /// Saves the node to file as json.
    /// </summary>
    public void Save()
    {
        FSUtilities.WriteToFile(Locations.NodeData, ID.ToString(), this);
    }
}