using Microsoft.AspNetCore.Mvc;

namespace sc2_directstrike.api.Controllers;

[Route(NAME)]
[ApiController]
public class PKTController : ControllerBase
{
    const string NAME = "pkt";

    static readonly Random random = new();
    static readonly char[] charSet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRTSUFWXYZ".ToCharArray();

    [HttpGet]
    public string RequestNewPKT()
    {
        string pkt = string.Empty;
        for (int i = 0; i < 24; i++)
        {
            pkt += charSet[random.Next(charSet.Length)];
        }
        return pkt;
    }
}
