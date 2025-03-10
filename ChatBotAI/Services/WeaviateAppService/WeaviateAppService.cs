using System.Text;
using ChatBotAI.Models;
using ChatBotAI.Services.WeaviateAppService.Dto;
using Microsoft.Extensions.Configuration;

namespace ChatBotAI.Services.WeaviateAppService;

public class WeaviateAppService: IWeaviateAppService
{
    private static readonly HttpClient httpClient = new HttpClient();
    private static string _weaviateUrl = "";
    
    public WeaviateAppService(IConfiguration config)
    {
        _weaviateUrl = config["WeaviateUrl"];
    }
    
    public async Task CreateSchema()
    {
        var schema = new
        {
            @class = "Article",
            description = "An article with a title and content",
            properties = new[]
            {
                new { name = "title", dataType = new[] { "string" } },
                new { name = "content", dataType = new[] { "string" } }
            },
            vectorizer = "text2vec-transformers" 
        };

        var json = System.Text.Json.JsonSerializer.Serialize(schema);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{_weaviateUrl}/schema", content);
        response.EnsureSuccessStatusCode();

        Console.WriteLine("Schema created successfully!");
    }

    public async Task UploadData(Article input)
    {
        var article = new
        {
            @class = "Article",
            properties = new
            {
                title = input.Title,
                content = input.Content
            }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(article);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{_weaviateUrl}/objects", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Data uploaded successfully!");
        Console.WriteLine(responseJson);
    }

    public async Task QueryData()
    {
        var query = new
        {
            query = @"
            {
                Get {
                    Article {
                        title
                        content
                    }
                }
            }"
        };

        var json = System.Text.Json.JsonSerializer.Serialize(query);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{_weaviateUrl}/graphql", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Data queried successfully!");
        Console.WriteLine(responseJson);
    }

    public async Task DeleteArticlesByTitle(string title)
    {
        var deleteRequest = new
        {
            @class = "Article",
            where = new
            {
                path = new[] { "title" },
                @operator = "Equal",
                valueString = title
            }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(deleteRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{_weaviateUrl}/batch/objects", content);
        response.EnsureSuccessStatusCode();

        Console.WriteLine("Articles deleted successfully!");
    }

    public async Task<string> GetArticleId()
    {
        var query = new
        {
            query = @"
            {
                Get {
                    Article {
                        _additional {
                            id
                        }
                        title
                        content
                    }
                }
            }"
        };

        var json = System.Text.Json.JsonSerializer.Serialize(query);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{_weaviateUrl}/graphql", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = System.Text.Json.JsonSerializer.Deserialize<QueryResult>(responseJson);

        return result.Data.Get.Article[0].Additional.Id;
    }

    public async Task<string> QueryWeaviate(string query)
    {
        var searchQuery = new
        {
            query = $@"
        {{
            Get {{
                Article(
                    nearText: {{
                        concepts: [""{query}""]
                    }}
                ) {{
                    title
                    content
                    _additional {{
                        distance
                    }}
                }}
            }}
        }}"
        };

        var json = System.Text.Json.JsonSerializer.Serialize(searchQuery);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{_weaviateUrl}/graphql", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        return responseJson;
    }
}