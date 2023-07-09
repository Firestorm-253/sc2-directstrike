namespace api.sc2_directstrike.tests.UnitTests.Controllers;

using api.sc2_directstrike.Controllers;

public class ReplayPlayerControllerTests
{
    ReplayPlayerController replayPlayerController = null!;

    [SetUp]
    public void Setup()
    {
        this.replayPlayerController = new ReplayPlayerController();

        Program.DbContext = new DbContext();
        Program.ConnectDb(true);
    }

    [Test]
    public void GetById()
    {
        string pkt = "read_test";
        int id = 0;

        var replayPlayer = this.replayPlayerController.GetById(pkt, id).GetAwaiter().GetResult();

        Assert.IsNotNull(replayPlayer);
    }
}
