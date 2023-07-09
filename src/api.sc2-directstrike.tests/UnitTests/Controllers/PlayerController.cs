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

        Program.ConnectDb(true);
    }

    [Test]
    public void GetById()
    {
        string pkt = "read_test";
        int id = 0;

        var player = this.playerController.GetById(pkt, id).GetAwaiter().GetResult();

        Assert.IsNotNull(player);
    }
}
