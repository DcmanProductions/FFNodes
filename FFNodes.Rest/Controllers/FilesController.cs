// LFInteractive LLC. - All Rights Reserved
using FFNodes.Core.Utilities;
using FFNodes.Server.Collections;
using FFNodes.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace FFNodes.Rest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetFiles()
    {
        return new JsonResult(ProcessFileCollection.Instance.GetFiles());
    }

    [HttpGet("next")]
    public IActionResult GetNext([FromQuery(Name = "node")] string node_name)
    {
        NodeModel? node = NodeCollection.Instance.GetNode(node_name);
        if (node == null)
        {
            return BadRequest(new
            {
                message = $"Node doesn't exist: {node_name}"
            });
        }
        string file = ProcessFileCollection.Instance.GetNextFile();
        FileInfo info = new(file);
        FileStream fs = new(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        FileStreamResult result = new(fs, "video/*")
        {
            FileDownloadName = info.Name,
            EnableRangeProcessing = true,
            LastModified = info.LastWriteTime
        };

        HttpContext.Response.Headers.Add("OriginalPath", file);

        return result;
    }

    [HttpHead("next")]
    public IActionResult GetNextName()
    {
        FileInfo info = new(ProcessFileCollection.Instance.GetNextFile(false));
        HttpContext.Response.Headers.Add("Path", info.FullName);
        HttpContext.Response.Headers.Add("Name", info.Name);
        HttpContext.Response.Headers.Add("Size", info.Length.ToString());
        return Ok();
    }

    [HttpPost("next"), DisableRequestSizeLimit]
    public async Task<IActionResult> UploadNext([FromQuery(Name = "path")] string originalFilePath)
    {
        Stream bodyStream = Request.Body;
        if (bodyStream == null)
        {
            return BadRequest(new
            {
                message = "No file was present in body"
            });
        }
        if (!System.IO.File.Exists(originalFilePath))
        {
            return BadRequest(new
            {
                message = $"The original file path specified does not exist: {originalFilePath}"
            });
        }
        FileInfo info = new(originalFilePath);
        string tmp_file = Path.Combine(Locations.Temp, info.Name);
        using (FileStream fs = new(tmp_file, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await bodyStream.CopyToAsync(fs);
        }
        System.IO.File.Move(tmp_file, originalFilePath, true);
        return Ok();
    }
}