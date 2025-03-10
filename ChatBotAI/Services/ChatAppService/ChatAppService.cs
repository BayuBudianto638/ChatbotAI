using ChatBotAI.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ChatBotAI.Services.ChatAppService;

public class ChatAppService : IChatAppService
{
    private readonly IMongoCollection<ChatSession> _chatSessions;
    private readonly IMongoCollection<ChatHistory> _chatHistories;

    public ChatAppService(IConfiguration config)
    {
        var connectionString = config["MongoDB:ConnectionString"];
        var databaseName = config["MongoDB:DatabaseName"];
        
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        
        _chatSessions = database.GetCollection<ChatSession>("chatsession");
        _chatHistories = database.GetCollection<ChatHistory>("chathistory");
    }
    
    public async Task<ChatSession> GetActiveSessionAsync(string userId)
    {
        return await _chatSessions
            .Find(s => s.UserId == userId && s.Status == "active")
            .FirstOrDefaultAsync();
    }

    public async Task SaveSessionAsync(ChatSession session)
    {
        await _chatSessions.InsertOneAsync(session);
    }

    public async Task SaveMessageAsync(ChatHistory message)
    {
        await _chatHistories.InsertOneAsync(message);
    }
    
    public async Task<List<ChatSession>> GetAllSessionsAsync()
    {
        return await _chatSessions.Find(_ => true).ToListAsync();
    }
    
    public async Task<List<ChatHistory>> GetChatHistoryAsync(string sessionId)
    {
        return await _chatHistories
            .Find(m => m.SessionId == sessionId)
            .ToListAsync();
    }
    
    public async Task UpdateSessionStatusAsync(string sessionId, string status)
    {
        var filter = Builders<ChatSession>.Filter.Eq(s => s.Id, sessionId);
        var update = Builders<ChatSession>.Update.Set(s => s.Status, status);

        await _chatSessions.UpdateOneAsync(filter, update);
    }
    
    public async Task<ChatSession> GetOrCreateChatSession(string userId)
    {
        var activeSession = await GetActiveSessionAsync(userId);

        if (activeSession != null)
        {
            return activeSession;
        }

        var newSession = new ChatSession
        {
            UserId = userId,
            StartTime = DateTime.UtcNow,
            EndTime = null,
            Status = "active"
        };

        await SaveSessionAsync(newSession);

        return newSession;
    }
}