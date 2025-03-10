using ChatBotAI.Models;

namespace ChatBotAI.Services.ChatbotAppService;

public interface IChatbotAppService
{
    void StartNewChat();
    Task<string> GetResponse(string prompt);
    Task<string> GetResponseFromWeaviate(string prompt);
}