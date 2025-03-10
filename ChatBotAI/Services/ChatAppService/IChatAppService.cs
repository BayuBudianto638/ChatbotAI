using ChatBotAI.Models;

namespace ChatBotAI.Services.ChatAppService;

public interface IChatAppService
{
    Task<ChatSession> GetActiveSessionAsync(string userId);
    Task SaveSessionAsync(ChatSession session);
    Task SaveMessageAsync(ChatHistory message);
    Task<List<ChatSession>> GetAllSessionsAsync();
    Task<List<ChatHistory>> GetChatHistoryAsync(string sessionId);
    Task UpdateSessionStatusAsync(string sessionId, string status);
    Task<ChatSession> GetOrCreateChatSession(string userId);
}