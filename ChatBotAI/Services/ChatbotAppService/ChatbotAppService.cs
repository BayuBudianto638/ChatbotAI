namespace ChatBotAI.Services.ChatbotAppService;

using System;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;

public class ChatbotAppService : IChatbotAppService
{
    private static readonly string apiKey = "GEMINI_APIKEY";  
    private static readonly string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";
    public async Task<string> GetResponse(string prompt)
    {
        var client = new RestClient(apiUrl);
        var request = new RestRequest("", Method.Post);

        request.AddHeader("Content-Type", "application/json");

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 1,
                topK = 40,
                topP = 0.95,
                maxOutputTokens = 8192,
                responseMimeType = "text/html"
            }
        };

        request.AddJsonBody(requestBody);
        var response = await client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            var jsonResponse = JObject.Parse(response.Content);
            return jsonResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "No response";
        }
        else
        {
            return $"Error: {response.StatusCode} - {response.Content}";
        }
    }
}