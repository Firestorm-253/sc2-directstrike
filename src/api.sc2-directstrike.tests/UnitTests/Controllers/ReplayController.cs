namespace api.sc2_directstrike.tests.UnitTests.Controllers;

using api.sc2_directstrike.Controllers;

public class ReplayControllerTests
{
    ReplayController replayController = null!;

    [SetUp]
    public void Setup()
    {
        this.replayController = new ReplayController();

        Program.DbContext = new DbContext();
        Program.ConnectDb(true);
    }

    [Test]
    public void GetById()
    {
        string pkt = "read_test";
        int id = 0;

        var replay = this.replayController.GetById(pkt, id).GetAwaiter().GetResult();

        Assert.IsNotNull(replay);
    }
}
