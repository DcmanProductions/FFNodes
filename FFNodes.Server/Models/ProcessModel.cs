// LFInteractive LLC. - All Rights Reserved
using Chase.FFmpeg.Info;

namespace FFNodes.Server.Models;

[Serializable]
public struct ProcessModel
{
    /// <summary>
    /// The average speed that ffmpeg ran.
    /// </summary>
    public double AverageSpeed { get; set; }

    /// <summary>
    /// The name of the file. <br/>
    /// Example: <i>video_file.mkv</i>
    /// </summary>
    public string File { get; init; }

    /// <summary>
    /// The duration of the video
    /// </summary>
    public FFMediaInfo MediaInfo { get; set; }

    /// <summary>
    /// The updated size of the file in bytes.
    /// </summary>
    public long NewSize { get; set; }

    /// <summary>
    /// The original size of the file in bytes.
    /// </summary>
    public long OriginalSize { get; set; }

    /// <summary>
    /// The full path to the file. <br/>
    /// Example: <i>/path/to/video_file.mkv</i>
    /// </summary>
    public string Path { get; init; }

    /// <summary>
    /// If the video was successfully reduced.
    /// </summary>
    public bool Successful { get; set; }

    /// <summary>
    /// The amount of time it took to process file.
    /// </summary>
    public long TimeToProcess { get; set; }
}