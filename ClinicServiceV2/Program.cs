using ClinicService.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Net;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, 5002, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });

            options.Listen(IPAddress.Any, 5001, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1;
            });

        });

        builder.Services.AddGrpc().AddJsonTranscoding();

        builder.Services.AddGrpcSwagger();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Clinic Srvise v2", Version = "1" });
            var filePath = Path.Combine(System.AppContext.BaseDirectory, "ClinicServiceV2.xml");
            c.IncludeXmlComments(filePath);
            c.IncludeGrpcXmlComments(filePath, includeControllerXmlComments: true);
        });

        builder.Services.AddDbContext<ClinicServiceDbContext>(options =>
        {
            options.UseSqlite(builder.Configuration["Settings:DatabaseOptions:ConnectionString"]);
            SQLitePCL.Batteries.Init();
        });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.UseAuthorization();

        app.UseRouting();
        app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

        app.MapGrpcService<ClinicServiceV2.Services.ClinicService>().EnableGrpcWeb();

        app.MapGet("/", () => " ........ ");

        app.Run();
    }
}