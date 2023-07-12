using api.sc2_directstrike.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace api.sc2_directstrike.Controllers;
using Services;
using System.Xml.Linq;

[ApiController]
[Route("{pkt}/" + NAME)]
public class RatingController : ControllerBase
{
    const string NAME = "rating";

    private readonly IServiceProvider serviceProvider;
    
    public RatingController(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    [HttpPost]
    public async Task ReCalc(string pkt)
    {
        if (pkt.Length != 24)
        {
            throw new ArgumentException("ERROR: Invalid PKT length!");
        }

        using var scope = this.serviceProvider.CreateScope();
        var ratingService = scope.ServiceProvider.GetRequiredService<RatingService>();

        await ratingService.ReCalc(pkt);
    }
}
