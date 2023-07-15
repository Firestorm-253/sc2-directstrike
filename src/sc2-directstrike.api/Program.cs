namespace sc2_directstrike.api;
using Contexts;
using Controllers;
using Services;
using System.Runtime.CompilerServices;

public class Program
{
    public static IWebHostEnvironment Environment { get; set; }

    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddTransient<DbContext>();

        builder.Services.AddTransient<ReplayContext>();
        builder.Services.AddTransient<ReplayPlayerContext>();
        builder.Services.AddTransient<PlayerContext>();

        builder.Services.AddTransient<RatingService>();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        //app.UseEndpoints(endpoints =>
        //{
        //});

        app.UseHttpsRedirection();

        app.MapControllers();

        Program.Environment = app.Environment;
        app.Run();
    }
}
