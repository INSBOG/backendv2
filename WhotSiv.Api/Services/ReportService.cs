using WhotSiv.Validator;

namespace WhotSiv.Api.Services;

public class ReportService
{
    public void ProcessReport(IFormFile file)
    {
        

        var filePath = "Uploads/" + file.FileName;
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

    }
}