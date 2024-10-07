using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

public class RekorScoutService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public RekorScoutService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> RecognizeLicensePlateAsync(IFormFile imageFile)
    {
        var apiKey = _configuration["RekorScout:ApiKey"];
        var apiUrl = "https://api.rekor.ai/v1/plate-reader"; // Rekor Scout API endpoint

        using (var content = new MultipartFormDataContent())
        {
            content.Add(new StreamContent(imageFile.OpenReadStream())
            {
                Headers = { ContentType = new MediaTypeHeaderValue(imageFile.ContentType) }
            }, "image", imageFile.FileName);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                return result.results[0].plate.ToString(); // 返回车牌号
            }
            else
            {
                throw new Exception("Error calling Rekor Scout API: " + response.ReasonPhrase);
            }
        }
    }
}
