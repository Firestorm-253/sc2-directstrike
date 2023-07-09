using api.sc2_directstrike.Controllers;

namespace api.sc2_directstrike;

public class Program
{
    public static DbContext DbContext { get; set; } = null!;

    static void Main(string[] args)
    {
        Program.DbContext = new DbContext();
        RunAPI(args);
    }

    static void RunAPI(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        //app.UseEndpoints(endpoints =>
        //{
        //});

        ConnectDb(app.Environment.IsDevelopment());

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }

    public static void ConnectDb(bool isDevelop, DbContext? context = null)
    {
        if (context == null)
        {
            context = Program.DbContext;
        }

        var privatData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("privatdata.json"))!;

        if (isDevelop)
        {
            context.Connect("server90.hostfactory.ch", 3306, "sc2_directstrike_test",
                user: privatData["user"],
                password: privatData["password"]);
        }
        else
        {
            context.Connect("server90.hostfactory.ch", 3306, "sc2_directstrike",
                user: privatData["user"],
                password: privatData["password"]);
        }
    }
}