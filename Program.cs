using MySql.Data.MySqlClient;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseRouting();
        app.MapDefaultControllerRoute();
        app.MapControllers();

        app.UseAuthorization();

        // //     // app.Use(async (c, next) =>
        // //     // {
        // //     //     var http = new HttpClient();

        // //     //     await next();

        // //     //     if (c.Response.StatusCode != 404) return;

        // //     //     if (c.Request.Method == "GET")
        // //     //     {
        // //     //         var res = await http.GetAsync($"http://127.0.0.1:8080/{c.Request.Path}{(c.Request.QueryString.HasValue ? $"?{c.Request.QueryString}" : "")}");
        // //     //         var stream = await res.Content.ReadAsStringAsync();

        // //     //         c.Response.ContentType = res.Content.Headers.ContentType?.ToString() ?? "text/html";
        // //     //         c.Response.StatusCode = (int)res.StatusCode;
        // //     //         await c.Response.WriteAsync(stream);
        // //     //         //await c.Response.WriteAsync($"http://127.0.0.1:6081/{c.Request.Path}{(c.Request.QueryString.HasValue ? $"?{c.Request.QueryString}" : "")}");
        // //     //         return;
        // //     //     }

        // //     //await c.Response.WriteAsync("<html><body>");
        // //     //await c.Response.WriteAsync($"<div>{c.Request.Path}</div>");
        // //     //await c.Response.WriteAsync("</body></html>");
        // // });

        app.Run();
    }
}