using System.Text;
using System.Text.Json;
using ChatBotAI.Models;

namespace ChatBotAI.Services;

public class GeminiUploader
{
    private readonly string apiKey = "YOUR_GOOGLE_API_KEY";
    private readonly string baseUrl = "https://generativelanguage.googleapis.com/v1beta";
    private readonly HttpClient client;

    public GeminiUploader()
    {
        client = new HttpClient();
        client.DefaultRequestHeaders.Add("Content-Type", "application/json");
    }

    public async Task UploadBooksAsync(List<Book> books)
    {
        if (books.Count == 0)
        {
            Console.WriteLine("No books to upload.");
            return;
        }

        string requestUrl = $"{baseUrl}/tunedModels?key={apiKey}";

        var requestBody = new
        {
            display_name = "book knowledge model",
            base_model = "models/gemini-1.5-flash-001-tuning",
            tuning_task = new
            {
                hyperparameters = new
                {
                    batch_size = 2,
                    learning_rate = 0.001,
                    epoch_count = 5
                },
                training_data = new
                {
                    examples = new
                    {
                        examples = books.Select(book => new
                        {
                            text_input = $"Book ID: {book.BookId}, Title: {book.Title}, Author: {book.Author}, Year: {book.Year}, Edition: {book.Edition}, Text: {book.Text}, File Key: {book.FileKey}, External Company ID: {book.ExternalCompanyId}",
                            output = $"Metadata stored for book '{book.Title}' by {book.Author}."
                        }).ToArray()
                    }
                }
            }
        };

        string jsonRequest = JsonSerializer.Serialize(requestBody);
        HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync(requestUrl, content);
        string responseJson = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Tuning Request Response: " + responseJson);

        using JsonDocument doc = JsonDocument.Parse(responseJson);
        string operation = doc.RootElement.GetProperty("name").GetString();

        await MonitorTuningProgress(operation);
    }

    private async Task MonitorTuningProgress(string operation)
    {
        string operationUrl = $"https://generativelanguage.googleapis.com/v1/{operation}?key={apiKey}";
        bool tuningDone = false;

        while (!tuningDone)
        {
            await Task.Delay(5000);

            HttpResponseMessage response = await client.GetAsync(operationUrl);
            string responseJson = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(responseJson);
            int completedPercent = doc.RootElement.GetProperty("metadata").GetProperty("completedPercent").GetInt32();
            Console.WriteLine($"Tuning... {completedPercent}%");

            tuningDone = doc.RootElement.TryGetProperty("done", out JsonElement doneElement) && doneElement.GetBoolean();
        }

        Console.WriteLine("Tuning Complete!");
        await GetTunedModelState(operation);
    }

    private async Task GetTunedModelState(string operation)
    {
        string tunedModelUrl = $"https://generativelanguage.googleapis.com/v1beta/{operation}?key={apiKey}";

        HttpResponseMessage response = await client.GetAsync(tunedModelUrl);
        string responseJson = await response.Content.ReadAsStringAsync();

        using JsonDocument doc = JsonDocument.Parse(responseJson);
        string state = doc.RootElement.GetProperty("state").GetString();

        Console.WriteLine($"Tuned Model State: {state}");
    }
}