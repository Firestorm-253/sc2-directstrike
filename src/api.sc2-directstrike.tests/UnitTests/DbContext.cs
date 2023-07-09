namespace api.sc2_directstrike.tests.UnitTests;

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
}
