namespace api.sc2_directstrike;
using Controllers;


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

        string dbName = string.Empty;
        if (app.Environment.IsProduction())
        {
            dbName = "sc2_directstrike";
        }
        else if (app.Environment.IsDevelopment())
        {
            dbName = "sc2_directstrike_dev";
        }

        ConnectDb(dbName);

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }

    public static void ConnectDb(string dbName, DbContext? context = null)
    {
        if (context == null)
        {
            context = Program.DbContext;
        }

        var privatData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("privatdata.json"))!;

        context.Connect("server90.hostfactory.ch", 3306, dbName,
            user: privatData["user"],
            password: privatData["password"]);
    }
}
