// LFInteractive LLC. - All Rights Reserved

namespace FFNodes.Server.Utilities;

public sealed class ServerManager
{
    public static ServerManager Instance = Instance ??= new();
    private readonly long _start;

    private ServerManager()
    {
        _start = DateTime.Now.Ticks;
    }

    public TimeSpan GetUpTime() => new(DateTime.Now.Ticks - _start);
}