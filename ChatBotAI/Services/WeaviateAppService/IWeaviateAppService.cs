using ChatBotAI.Models;

namespace ChatBotAI.Services.WeaviateAppService;

public interface IWeaviateAppService
{
    Task  CreateSchema();
    Task UploadData(Article input);
    Task QueryData();
    Task DeleteArticlesByTitle(string title);
    Task<string> GetArticleId();
    Task<string> QueryWeaviate(string query);
}