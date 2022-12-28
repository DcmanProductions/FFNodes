// LFInteractive LLC. - All Rights Reserved
using FFNodes.Server.Collections;
using FFNodes.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace FFNodes.Rest.Controllers;

/// <summary>
/// Handles node commands
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class NodesController : ControllerBase
{
    /// <summary>
    /// Gests a list of all nodes
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult GetNodes()
    {
        return new JsonResult(new
        {
            Active = new
            {
                Count = NodeCollection.Instance.ActiveNodes.Length,
                Nodes = NodeCollection.Instance.ActiveNodes
            },
            Registered = new
            {
                Count = NodeCollection.Instance.Nodes.Length,
                NodeCollection.Instance.Nodes
            },
        });
    }

    /// <summary>
    /// Pings the server with clients current workload
    /// </summary>
    /// <param name="name"></param>
    /// <param name="processes"></param>
    /// <returns></returns>
    [HttpPost()]
    public IActionResult PingNode([FromQuery] string name, [FromForm] NodeActiveProcessModel[] processes)
    {
        NodeModel? node = NodeCollection.Instance.GetNode(name);
        if (node == null)
        {
            return BadRequest(new
            {
                message = $"Node doesn't exist: {name}"
            });
        }
        node?.Ping(processes);
        return Ok();
    }

    [HttpPut()]
    public IActionResult RegisterNode([FromQuery] string name)
    {
        if (NodeCollection.Instance.RegisterNode(name))
            return Ok(new
            {
                message = "Node register successfully!"
            });
        return BadRequest(new
        {
            message = $"Node with name of \"{name}\" already exists!"
        });
    }
}