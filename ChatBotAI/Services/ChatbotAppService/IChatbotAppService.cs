namespace ChatBotAI.Services.ChatbotAppService;

public interface IChatbotAppService
{
    public Task<string> GetResponse(string prompt);
}