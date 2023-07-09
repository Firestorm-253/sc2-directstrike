using api.sc2_directstrike.Controllers;

namespace api.sc2_directstrike;

class Program
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

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        //app.UseEndpoints(endpoints =>
        //{
        //});

        var privatData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("privatdata.json"))!;

        if (app.Environment.IsProduction())
        {
            DbContext.Connect("server90.hostfactory.ch", 3306, "db_mysql",
                user: privatData["user"],
                password: privatData["password"]);
        }
        else if (app.Environment.IsDevelopment())
        {
            DbContext.Connect("server90.hostfactory.ch", 3306, "testdb",
                user: privatData["user"],
                password: privatData["password"]);
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}