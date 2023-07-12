namespace api.sc2_directstrike;
using Contexts;
using Controllers;
using Services;

public class Program
{
    static void Main(string[] args)
    {
        RunAPI(args);
    }

    static void RunAPI(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<DbContext>();

        builder.Services.AddTransient<ReplayContext>();
        builder.Services.AddTransient<ReplayPlayerContext>();
        builder.Services.AddTransient<PlayerContext>();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        //app.UseEndpoints(endpoints =>
        //{
        //});

        ConnectDb(app);

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }

    public static void ConnectDb(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        var privatData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("privatdata.json"))!;

        string dbName = string.Empty;
        if (app.Environment.IsProduction())
        {
            dbName = "sc2_directstrike";
        }
        else if (app.Environment.IsDevelopment())
        {
            dbName = "sc2_directstrike_dev";
        }

        dbContext.Connect("server90.hostfactory.ch", 3306, dbName,
            user: privatData["user"],
            password: privatData["password"]);
    }
}
