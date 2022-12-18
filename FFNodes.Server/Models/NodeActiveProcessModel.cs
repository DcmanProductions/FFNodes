// LFInteractive LLC. - All Rights Reserved
namespace FFNodes.Server.Models;

[Serializable]
public struct NodeActiveProcessModel
{
    /// <summary>
    /// The current ffmpeg arguments
    /// </summary>
    public string FFmpegArgs { get; set; }

    /// <summary>
    /// The current ffmpeg completion percentage
    /// </summary>
    public double Percent { get; set; }

    /// <summary>
    /// The current size in bytes
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// The current speed.
    /// </summary>
    public double Speed { get; set; }
}