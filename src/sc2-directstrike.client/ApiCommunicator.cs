using System.Net.Http.Headers;

namespace sc2_directstrike.client;

public class ApiCommunicator
{
    private readonly HttpClient client;

    public ApiCommunicator(string endPoint)
    {
        client = new HttpClient()
        {
            BaseAddress = new Uri(endPoint)
        };
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public bool Execute(string requestUrl, HttpMethod method)
    {
        var request = new HttpRequestMessage(method, requestUrl);
        var response = client.SendAsync(request).GetAwaiter().GetResult();

        return response.IsSuccessStatusCode;
    }

    public T? Get<T>(string requestUrl)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        var response = client.SendAsync(request).GetAwaiter().GetResult();

        if (response.IsSuccessStatusCode)
        {
            return response.Content.ReadAsAsync<T>().GetAwaiter().GetResult();
        }
        return default;
    }

    public async Task<bool> Post<T>(string requestUrl, T[] objs)
    {
        var response = await client.PostAsJsonAsync(requestUrl, objs);
        if (!response.IsSuccessStatusCode)
        {
            await Console.Out.WriteLineAsync("ERROR: Post failed!");
        }
        return response.IsSuccessStatusCode;
    }

    public void Delete(string requestUrl)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUrl);

        var response = client.SendAsync(request).GetAwaiter().GetResult();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("ERROR: Delete failed!");
        }
    }
}