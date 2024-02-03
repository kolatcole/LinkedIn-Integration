using System.Text.Json.Serialization;
using System.Text.Json;

namespace LinkedIn_Integration.Services
{
    public static class Helper
    {
        public static async Task<HttpResponseMessage> ExecuteAsync(HttpRequestMessage request, HttpClient client)
        {
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                            $"Unable to get data at this moment");
            }
            return response;
        }
    }
}
