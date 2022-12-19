// LFInteractive LLC. - All Rights Reserved
using FFNodes.Core.Utilities;
using FFNodes.Server.Collections;
using FFNodes.Server.Utilities;
using Serilog;

namespace FFNodes.Rest;

public class WebAPIStart
{
    public static void Main(string[] args)
    {
        using Serilog.Core.Logger log = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add using statements
        builder.WebHost.UseKestrel(i => { i.ListenAnyIP(ApplicationData.ServerAPIPort); });
        builder.WebHost.UseSerilog();

        _ = ServerManager.Instance;

        Task.Run(() =>
        {
            try
            {
                log.Information("Getting Node Collection");
                _ = NodeCollection.Instance;
            }
            catch (Exception ex)
            {
                log.Error("Unable to gather nodes.", ex);
            }
            try
            {
                log.Information("Getting Process Files");
                ProcessFileCollection.Instance.Refresh();
            }
            catch (Exception ex)
            {
                log.Error("Unable to gather files.", ex);
            }
        });

        var app = builder.Build();

        log.Information("Starting API");
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();
        app.UseSerilogRequestLogging();

        log.Information("API is running at http://127.0.0.1:{0}/api", ApplicationData.ServerAPIPort);
        app.Run();
    }
}