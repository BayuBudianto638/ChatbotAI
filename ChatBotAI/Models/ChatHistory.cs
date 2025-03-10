using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ChatBotAI.Models;

public class ChatHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string SessionId { get; set; } 
    public string User { get; set; } 
    public string Message { get; set; } 
    public DateTime Timestamp { get; set; } 
    public string Role { get; set; } 
}