using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


public class RelayController : ControllerBase
{

    [Route("/")]
    public IActionResult Get()
    {
        var v = new ViewResult()
        {
            ViewName = "relay",
        };

        return v;
    }

    [Route("{ejm}/hls/stream.m3u8")]
    public async Task<IActionResult> GetStreamBase(string ejm)
    {
        if (!authorized(ejm)) return NotFound();

        var http = new HttpClient();
        var res = await http.GetAsync("http://127.0.0.1:6081/hls/stream.m3u8");
        var stream = await res.Content.ReadAsStreamAsync();
        var content = new StreamReader(stream).ReadToEnd();

        return Content(content, "application/x-mpegURL");
    }

    [Route("{ejm}/hls/{id}/stream.m3u8")]
    public async Task<IActionResult> GetStreamByID(int id, string ejm)
    {

        if (!authorized(ejm)) return NotFound();

        var http = new HttpClient();
        var res = await http.GetAsync($"http://127.0.0.1:6081/hls/{id}/stream.m3u8");
        var stream = await res.Content.ReadAsStreamAsync();
        var content = new StreamReader(stream).ReadToEnd();
        return Content(content, "application/x-mpegURL");
    }

    private bool authorized(string ejm)
    {

        string ip = "";
        if ((Request.Cookies["instance_tag"] ?? "") == "")
        {
            ip = Guid.NewGuid().ToString();
            Response.Cookies.Append("instance_tag", ip, new CookieOptions() { Path = "/" });
        }
        else
        {
            ip = Request.Cookies["instance_tag"] ?? "";
        }

        var ejmpath = "./ejm";

        if (!System.IO.File.Exists(ejmpath)) return false;
        var ejmdata = System.IO.File.ReadAllLines(ejmpath);

        if (ejmdata.Contains(ejm))
        {
            var filepath = $"./ejms/{ejm}";

            if (!System.IO.Directory.Exists("./ejms")) System.IO.Directory.CreateDirectory("./ejms");

            if (!System.IO.File.Exists(filepath))
            {
                System.IO.File.WriteAllText(filepath, ip);
                return true;
            }
            else
            {
                var lastime = System.IO.File.GetLastWriteTime(filepath);
                var fileip = System.IO.File.ReadAllText(filepath);
                var diff = DateTime.Now - lastime;
                if (fileip == ip && diff.Seconds < 180)
                {
                    System.IO.File.WriteAllText(filepath, ip);
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
        else
        {
            return false;
        }
    }

    [Route("{ejm}/hls/{id}/{segment}.ts")]
    public IActionResult GetStreamByID(string ejm, int id, string segment)
    {
        if (!authorized(ejm)) return NotFound();
        return new TSResult(id, segment);
    }

    public class TSResult : IActionResult
    {

        int id;
        string segment;
        public TSResult(int id, string segment)
        {
            this.id = id;
            this.segment = segment;

        }
        public async Task ExecuteResultAsync(ActionContext context)
        {

            var http = new HttpClient();
            var res = await http.GetAsync($"http://127.0.0.1:6081/hls/{id}/{segment}.ts");
            var stream = await res.Content.ReadAsByteArrayAsync();
            var response = context.HttpContext.Response;
            response.ContentType = "application/octet-stream";
            await response.Body.WriteAsync(stream, 0, stream.Length);
        }
    }



}
