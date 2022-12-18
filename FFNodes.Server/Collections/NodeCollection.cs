// LFInteractive LLC. - All Rights Reserved
using FFNodes.Core.Utilities;
using FFNodes.Server.Models;
using Newtonsoft.Json.Linq;
using System.IO.Compression;

namespace FFNodes.Server.Collections;

/// <summary>
/// A collection of all worker nodes.
/// </summary>
public class NodeCollection
{
    /// <summary>
    /// The singleton pattern for the node collection
    /// </summary>
    public static NodeCollection Instance = Instance ??= new();

    private readonly Dictionary<string, NodeModel> _nodes;

    private NodeCollection()
    {
        _nodes = new();
        Load();
    }

    /// <summary>
    /// Gets a node based on the Node's name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public NodeModel GetNode(string name) => _nodes[name];

    /// <summary>
    /// Loads existing nodes from the hard drive.
    /// </summary>
    public void Load()
    {
        _nodes.Clear();
        string[] files = Directory.GetFiles(Locations.NodeData, "*", SearchOption.TopDirectoryOnly);
        foreach (string file in files)
        {
            string json = "";
            using (FileStream fs = new(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using GZipStream compressed = new(fs, CompressionMode.Decompress);
                using StreamReader reader = new(compressed);
                json = reader.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(json))
            {
                NodeModel? node = JObject.Parse(json).ToObject<NodeModel>();
                if (node != null && node.HasValue)
                {
                    _nodes.Add(node.Value.Name, node.Value);
                }
            }
        }
    }

    /// <summary>
    /// Creates and Registers a new node.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool RegisterNode(string name)
    {
        if (!_nodes.ContainsKey(name))
        {
            NodeModel node = new()
            {
                ID = Guid.NewGuid(),
                Name = name,
                CompletedProcesses = new()
            };
            _nodes.Add(name, node);
            node.Save();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Runs the <seealso cref="NodeModel.Save()"/> method on each node.
    /// </summary>
    public void Save()
    {
        foreach (NodeModel node in _nodes.Values)
        {
            node.Save();
        }
    }
}