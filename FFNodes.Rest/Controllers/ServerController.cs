// LFInteractive LLC. - All Rights Reserved
using FFNodes.Core.Utilities;
using FFNodes.Server.Collections;
using FFNodes.Server.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FFNodes.Rest.Controllers;

[Route("api")]
[ApiController]
public class ServerController : ControllerBase
{
    [HttpGet()]
    public IActionResult Status()
    {
        return new JsonResult(new
        {
            Uptime = Formatters.GetFormattedTime(ServerManager.Instance.GetUpTime()),
            ActiveNodes = NodeCollection.Instance.ActiveNodes.Length
        });
    }
}