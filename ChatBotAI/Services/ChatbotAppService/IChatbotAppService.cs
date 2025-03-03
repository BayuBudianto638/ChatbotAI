using ChatBotAI.Models;

namespace ChatBotAI.Services.ChatbotAppService;

public interface IChatbotAppService
{
    void StartNewChat();
    public Task<string> GetResponse(string prompt);
    public Task<List<Book>> ReadBooksFromFile(string filePath);
}