using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace api.sc2_directstrike.Controllers;
using DTOs;

[ApiController]
[Route("replays")]
public class ReplayController : ControllerBase
{
    private readonly ILogger<ReplayController> logger;
    private readonly IConfiguration configuration;

    public ReplayController(ILogger<ReplayController> logger, IConfiguration configuration)
    {
        this.logger = logger;
        this.configuration = configuration;
    }

    [HttpGet]
    public async Task<IEnumerable<Replay?>> Get()
    {
        string query =
            $"SELECT * " +
            $"FROM replays ";

        var result = await DbContext.ReadFromDb(query);

        return result.Select(rows => Create(rows));
    }

    [HttpGet("id={id}")]
    public async Task<Replay?> GetById(int id)
    {
        string query =
            $"SELECT * " +
            $"FROM replays " +
            $"WHERE Id='{id}' ";

        var result = await DbContext.ReadFromDb(query);
        var row = result.Single();

        return Create(row);
    }

    private static Replay? Create(object[]? row)
    {
        if (row == null)
        {
            return null;
        }

        return new Replay()
        {
            Id = (int)row[0],
            GameTime = (DateTime)row[1]
        };
    }

    [HttpPost()]
    public async Task Post(Replay replay)
    {
        await DbContext.WriteToDb($"INSERT INTO replays (Id, GameTime) " +
                                  $"VALUES ({replay.Id}, '{replay.GameTime:yyyy-dd-MM hh:mm:ss}') ");
    }
}
