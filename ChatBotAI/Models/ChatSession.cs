using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatBotAI.Models;

public class ChatSession
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string UserId { get; set; } 
    public DateTime StartTime { get; set; } 
    public DateTime? EndTime { get; set; } 
    public string Status { get; set; } 
}