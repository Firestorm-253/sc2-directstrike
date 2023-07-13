using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace sc2_directstrike.api.Controllers;
using Services;
using DTOs;

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
