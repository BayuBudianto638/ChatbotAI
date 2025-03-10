using ChatBotAI.Services.WeaviateAppService.Dto;

namespace ChatBotAI.Models;

public class Article
{
    public Additional Additional { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}