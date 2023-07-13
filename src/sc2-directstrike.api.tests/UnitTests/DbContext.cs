namespace sc2_directstrike.api.tests.UnitTests;

using api.Contexts;
using api.Controllers;

public class DbContextTests
{
    DbContext DbContext { get; set; } = null!;

    [SetUp]
    public void Setup()
    {
        this.DbContext = new DbContext();
        Program.ConnectDb("sc2_directstrike_tests", this.DbContext);
    }

    [Test]
    public void Connect()
    {
        if (this.DbContext.Connection == null)
        {
            Program.ConnectDb("sc2_directstrike_tests", this.DbContext);
        }

        Assert.AreEqual(System.Data.ConnectionState.Open, this.DbContext.Connection!.State);
    }

    [Test]
    public void ReadFromDb()
    {
        string pkt = "read_test";
        string table = "replays";

        string query = $"SELECT * " +
                       $"FROM {table} ";
        query += DbContext.AddCondition(query, "PKT", pkt);

        var result = this.DbContext.ReadFromDb(query).GetAwaiter().GetResult();
        
        Assert.IsNotNull(result);

        Assert.Greater(result.Count, 0);
    }

    [Test]
    public void WriteToDb()
    {
        string pkt = new PKTController().RequestNewPKT();
        string table = "replays";

        this.DbContext.WriteToDb(pkt, table, new DTOs.Replay()).Wait();


        string query = $"SELECT * " +
                       $"FROM {table} ";
        query += DbContext.AddCondition(query, "PKT", pkt);

        var result = this.DbContext.ReadFromDb(query).GetAwaiter().GetResult();

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
