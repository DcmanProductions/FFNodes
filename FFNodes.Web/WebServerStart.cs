// LFInteractive LLC. - All Rights Reserved

using FFNodes.Core.Utilities;
using FFNodes.Rest;
using Serilog;

namespace FFNodes.Web;

public class WebServerStart
{
    public static void Main(string[] args)
    {
        using Serilog.Core.Logger log = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        Task.Run(() => WebAPIStart.Main(args));
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        // Add using statmenets
        builder.WebHost.UseSerilog();
        builder.WebHost.UseKestrel(i => { i.ListenAnyIP(ApplicationData.ServerWebPort); });

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        log.Information("Web is running at http://127.0.0.1:{0}", ApplicationData.ServerWebPort);

        app.Run();
    }
}