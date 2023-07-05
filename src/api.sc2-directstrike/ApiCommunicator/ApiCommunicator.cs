using System.Net.Http.Headers;

namespace api.sc2_directstrike;

public partial class ApiCommunicator
{
    private readonly HttpClient client;

    public ApiCommunicator()
    {
        client = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:33043/")
        };
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
