//namespace sc2_directstrike.api.tests.UnitTests.Controllers;

//using api.Controllers;

//public class ReplayControllerTests
//{
//    ReplayController replayController = null!;

//    [SetUp]
//    public void Setup()
//    {
//        this.replayController = new ReplayController();

//        Program.DbContext = new DbContext();
//        Program.ConnectDb("sc2_directstrike_tests");
//    }

//    [Test]
//    public void GetById()
//    {
//        string pkt = "read_test";
//        int id = 0;

//        var replay = this.replayController.GetById(pkt, id).GetAwaiter().GetResult();

//        Assert.IsNotNull(replay);
//    }

//    [Test]
//    public void Get()
//    {
//        string pkt = "read_test";

//        var player = this.replayController.Get(pkt).GetAwaiter().GetResult();
//        Assert.IsNotNull(player);
//    }
//}
