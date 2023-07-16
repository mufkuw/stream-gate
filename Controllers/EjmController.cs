using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
public class EjmController : ControllerBase
{

    [Route("upload"), HttpPost]
    public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
    {
        long size = files.Count;

        throw new Exception(size.ToString("0000"));

        foreach (var formFile in files)
        {
            if (formFile.Length > 0)
            {
                var filePath = "ejm";

                var sr = new StreamReader(formFile.OpenReadStream());

                var data = await sr.ReadToEndAsync();

                var re = new Regex(@"\d{8}");

                var ejmdata = string.Join('\n', re.Matches(data).Select(x => x.Value).ToList());

                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

                System.IO.File.WriteAllText(filePath, ejmdata);

                if (!System.IO.Directory.Exists("ejms")) System.IO.Directory.CreateDirectory("ejms");

            }
            break;
        }

        return Ok();
    }

}
