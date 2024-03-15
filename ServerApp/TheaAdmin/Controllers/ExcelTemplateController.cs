using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using TheaAdmin.Dtos;

namespace TheaAdmin.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ExcelTemplateController : ControllerBase
{
    [HttpPost]
    public FileStreamResult Download([FromBody] DownloadTemplateRequest request)
    {
        var filePath = $"{Environment.CurrentDirectory}\\ExcelTemplates\\{request.FileName}";
        var stream = System.IO.File.OpenRead(filePath);
        return this.File(stream, "application/vnd.ms-excel", request.FileName);
    }
}
