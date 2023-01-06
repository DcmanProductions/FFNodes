// LFInteractive LLC. - All Rights Reserved
namespace FFNodes.Core.Utilities;

public static class Locations
{
    public static string Data => Directory.CreateDirectory(Path.Combine(Root, "data")).FullName;
    public static string NodeData => Directory.CreateDirectory(Path.Combine(Data, "nodes")).FullName;

    public static string Root => Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationData.CompanyName, ApplicationData.ApplicationName)).FullName;
    public static string Temp => Directory.CreateDirectory(Path.Combine(Root, "tmp")).FullName;

    public static string Working => Directory.CreateDirectory(Path.Combine(Root, "working")).FullName;
}