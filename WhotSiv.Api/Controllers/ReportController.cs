namespace WhotSiv.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using WhotSiv.Api.Services;
using WhotSiv.Validator;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{

    private readonly ReportService _reportService;

    public ReportController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost("upload")]
    public IActionResult UploadReport(
        [FromForm] IFormFile file
    )
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        this._reportService.ProcessReport(file);
        return Ok("File processed successfully");
        
    }
}