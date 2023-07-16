using Microsoft.AspNetCore.Mvc;
using sc2_directstrike.api.Contexts;

namespace sc2_directstrike.api.Controllers;

[Route(NAME)]
[ApiController]
public class PKTController : ControllerBase
{
    private readonly IServiceProvider serviceProvider;

    public PKTController(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    const string NAME = "pkt";

    static readonly Random random = new();
    static readonly char[] charSet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRTSUFWXYZ".ToCharArray();

    [HttpGet]
    public async Task<string> RequestNewPKT()
    {
        using var scope = this.serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        string pkt = string.Empty;
        for (int i = 0; i < 24; i++)
        {
            pkt += charSet[random.Next(charSet.Length)];
        }

        string query =
            $"INSERT INTO pkts (PKT) " +
            $"VALUES ('{pkt}') ";

        await dbContext.WriteToDb(query);
        return pkt;
    }


    public static string GetQuery(string pkt)
        => $"(SELECT Id FROM pkts WHERE PKT = '{pkt}')";
}
