namespace api.sc2_directstrike.tests.UnitTests.Controllers;

using api.sc2_directstrike.Controllers;

public class PlayerControllerTests
{
    PlayerController playerController = null!;

    [SetUp]
    public void Setup()
    {
        this.playerController = new PlayerController();

        Program.DbContext = new DbContext();
        Program.ConnectDb("sc2_directstrike_tests");
    }

    [Test]
    public void GetById()
    {
        string pkt = "read_test";
        int id = 0;

        var player = this.playerController.GetById(pkt, id).GetAwaiter().GetResult();

        Assert.IsNotNull(player);
    }

    [Test]
    public void Get()
    {
        string pkt = "read_test";
        string name = "A";
        int inGameId = 184085;

        var player = this.playerController.Get(pkt, name, null).GetAwaiter().GetResult();
        Assert.IsNotNull(player);

        player = this.playerController.Get(pkt, null, inGameId).GetAwaiter().GetResult();
        Assert.IsNotNull(player);

        player = this.playerController.Get(pkt, name, inGameId).GetAwaiter().GetResult();
        Assert.IsNotNull(player);
    }
}
