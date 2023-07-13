namespace api.sc2_directstrike.tests.UnitTests.Controllers;

using sc2_directstrike.Controllers;

public class PKTControllerTests
{
    PKTController pktController = null!;

    [SetUp]
    public void Setup()
    {
        pktController = new PKTController();
    }

    [Test]
    public void RequestNewPKT()
    {
        string pkt = pktController.RequestNewPKT();

        Assert.AreEqual(24, pkt.Length);
    }
}
