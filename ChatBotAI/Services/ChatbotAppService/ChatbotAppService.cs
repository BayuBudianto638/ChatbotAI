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

    public async Task<List<Book>> ReadBooksFromFile(string filePath)
    {
        var books = new List<Book>();

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return books;
        }

        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines.Skip(1))
        {
            string[] parts;

            if (filePath.EndsWith(".csv"))
            {
                parts = line.Split(",");
            }
            else
            {
                parts = line.Split("|", StringSplitOptions.TrimEntries);
            }

            if (parts.Length == 8)
            {
                books.Add(new Book
                {
                    BookId = parts[0].Trim(),
                    Title = parts[1].Trim(),
                    Author = parts[2].Trim(),
                    Year = int.Parse(parts[3].Trim()),
                    Edition = parts[4].Trim(),
                    Text = parts[5].Trim(),
                    FileKey = parts[6].Trim(),
                    ExternalCompanyId = parts[7].Trim()
                });
            }
        }

        return books;
    }
}