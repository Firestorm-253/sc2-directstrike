namespace api.sc2_directstrike;

class Program
{
    static void Main(string[] args)
    {
        DbContext.Connect("server90.hostfactory.ch", 3306, "db_mysql", "LaurinZehnder", "La2003Sh");

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

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}