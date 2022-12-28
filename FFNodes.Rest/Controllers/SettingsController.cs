// LFInteractive LLC. - All Rights Reserved
using FFNodes.Server.Collections;
using Microsoft.AspNetCore.Mvc;

namespace FFNodes.Rest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SettingsController : ControllerBase
{
    [HttpDelete("directories")]
    public IActionResult DeleteDirectories([FromForm] string[] directories)
    {
        ProcessFileCollection.Instance.RemoveDirectories(directories);
        return Ok();
    }

    [HttpGet("directories")]
    public IActionResult GetDirectories()
    {
        return new JsonResult(ProcessFileCollection.Instance.GetDirectories());
    }

    [HttpPost("directories")]
    public IActionResult SetDirectories([FromForm] string[] directories)
    {
        List<string> existing_directories = new();
        List<string> nonexisting_directories = new();
        for (int i = 0; i < directories.Length; i++)
        {
            string directory = Path.GetFullPath(directories[i]);
            if (Directory.Exists(directory))
            {
                existing_directories.Add(directory);
            }
            else
            {
                nonexisting_directories.Add(directory);
            }
        }
        ProcessFileCollection.Instance.SetDirectories(existing_directories.ToArray());
        if (nonexisting_directories.Any())
        {
            return Ok(new
            {
                message = "Some directories were invalid! They were not saved!",
                nonexisting_directories
            });
        }
        return Ok();
    }
}