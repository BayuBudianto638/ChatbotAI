using System.Text.Json;
using ChatBotAI.Models;

namespace ChatBotAI.Services.ChatbotAppService;

using System;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;

public class ChatbotAppService : IChatbotAppService
{
    private readonly string _apiKey;
    private readonly string _apiUrl;
    private readonly List<object> _chatHistory;

    public ChatbotAppService()
    {
        _apiKey = Environment.GetEnvironmentVariable("GEMINI_APIKEY");
        _apiUrl = Environment.GetEnvironmentVariable("GEMINI_APIURL");

        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new InvalidOperationException("API Key is missing. Set GEMINI_APIKEY in environment variables.");
        }

        if (string.IsNullOrEmpty(_apiUrl))
        {
            throw new InvalidOperationException("API URL is missing. Set GEMINI_APIURL in environment variables.");
        }
        
        _chatHistory = new List<object>();
    }
    
    public void StartNewChat()
    {
        _chatHistory.Clear(); 
    }
    
    public async Task<string> GetResponse(string prompt)
    {
        _chatHistory.Add(new
        {
            role = "user",
            parts = new[] { new { text = prompt } }
        });

        var requestBody = new
        {
            contents = _chatHistory, 
            generationConfig = new
            {
                temperature = 1,
                topK = 40,
                topP = 0.95,
                maxOutputTokens = 8192
            }
        };

        string apiUrl = $"{_apiUrl}?key={_apiKey}";
        var client = new RestClient(apiUrl);
        var request = new RestRequest("", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody(requestBody);

        var response = await client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            var jsonResponse = JObject.Parse(response.Content);
            string botResponse = jsonResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "No response";

            _chatHistory.Add(new
            {
                role = "model",
                parts = new[] { new { text = botResponse } }
            });

            return botResponse;
        }
        else
        {
            return $"Error: {response.StatusCode} - {response.Content}";
        }
    }
    
    public async Task<string> GetResponseFromWeaviate(string prompt)
    {
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new { text = $"Based on the following information, answer the user's question:\n{prompt}" +
                            $"\nQuestion: What is the title about? Provide a detailed explanation of the context and summarize the content. Do not repeat the title." }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 1.2,
                topK = 40,
                topP = 0.95,
                maxOutputTokens = 8192
            }
        };

        string jsonBody = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions { WriteIndented = true });
       
        string apiUrl = $"{_apiUrl}?key={_apiKey}";
        var client = new RestClient(apiUrl);
        var request = new RestRequest("", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody(jsonBody);

        var response = await client.ExecuteAsync(request);

        return response.Content;
    }
}