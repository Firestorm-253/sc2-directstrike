namespace api.sc2_directstrike.tests.UnitTests;

using sc2_directstrike.Controllers;

public class DbContextTests
{
    DbContext dbContext = null!;

    [SetUp]
    public void Setup()
    {
        dbContext = new DbContext();
        this.Connect();
    }

    [Test]
    public void Connect()
    {
        if (dbContext.Connection == null)
        {
            var privatData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("privatdata.json"))!;

            dbContext.Connect("server90.hostfactory.ch", 3306, "sc2_directstrike_test",
                user: privatData["user"],
                password: privatData["password"]);
        }

        Assert.AreEqual(System.Data.ConnectionState.Open, this.dbContext.Connection!.State);
    }

    [Test]
    public void ReadFromDb()
    {
        string pkt = "read_test";
        string table = "replays";

        string query = $"SELECT * " +
                       $"FROM {table} ";
        query += DbContext.AddCondition(query, "PKT", pkt);

        var result = this.dbContext.ReadFromDb(query).GetAwaiter().GetResult();
        
        Assert.IsNotNull(result);

        Assert.Greater(result.Count, 0);
    }

    [Test]
    public void WriteToDb()
    {
        string pkt = new PKTController().RequestNewPKT();
        string table = "replays";

        dbContext.WriteToDb(pkt, table, new DTOs.Replay()).Wait();


        string query = $"SELECT * " +
                       $"FROM {table} ";
        query += DbContext.AddCondition(query, "PKT", pkt);

        var result = dbContext.ReadFromDb(query).GetAwaiter().GetResult();

        Assert.IsNotNull(result);

        Assert.Greater(result.Count, 0);
    }

    [Test]
    public void AddCondition()
    {
        string pkt = new PKTController().RequestNewPKT();
        string table = "replays";

        string query = $"SELECT * " +
                       $"FROM {table} ";
        string partialQuery = DbContext.AddCondition(query, "PKT", pkt);

        Assert.AreEqual($"WHERE PKT='{pkt}' ", partialQuery);

        query += partialQuery;
        partialQuery = DbContext.AddCondition(query, "TestName", "TestValue");

        Assert.AreEqual($"AND TestName='TestValue' ", partialQuery);
    }
}
