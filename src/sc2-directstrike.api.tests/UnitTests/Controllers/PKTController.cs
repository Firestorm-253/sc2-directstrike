namespace sc2_directstrike.api.tests.UnitTests.Controllers;

using api.Controllers;

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
