﻿//namespace sc2_directstrike.api.tests.UnitTests.Controllers;

//using api.Controllers;

//public class ReplayPlayerControllerTests
//{
//    ReplayPlayerController replayPlayerController = null!;

//    [SetUp]
//    public void Setup()
//    {
//        this.replayPlayerController = new ReplayPlayerController();

//        Program.DbContext = new DbContext();
//        Program.ConnectDb("sc2_directstrike_tests");
//    }

//    [Test]
//    public void GetById()
//    {
//        string pkt = "read_test";
//        int id = 0;

//        var replayPlayer = this.replayPlayerController.GetById(pkt, id).GetAwaiter().GetResult();

//        Assert.IsNotNull(replayPlayer);
//    }

//    [Test]
//    public void Get()
//    {
//        string pkt = "read_test";
//        int replayId = 0;
//        int playerId = 0;

//        var player = this.replayPlayerController.Get(pkt, replayId, null).GetAwaiter().GetResult();
//        Assert.IsNotNull(player);

//        player = this.replayPlayerController.Get(pkt, null, playerId).GetAwaiter().GetResult();
//        Assert.IsNotNull(player);

//        player = this.replayPlayerController.Get(pkt, replayId, playerId).GetAwaiter().GetResult();
//        Assert.IsNotNull(player);
//    }
//}
