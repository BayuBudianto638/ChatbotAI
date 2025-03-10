using Microsoft.AspNetCore.Mvc;
using ChatBotAI.Services.ChatbotAppService;
using ChatBotAI.Models;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ChatBotAI.Models.Dto;
using ChatBotAI.Services.ChatAppService;
using ChatBotAI.Services.WeaviateAppService;

[ApiController]
[Route("api/chatbot")]
public class ChatbotController : ControllerBase
{
    private readonly IChatbotAppService _chatbotService;
    private readonly IChatAppService _chatAppService;
    private readonly IWeaviateAppService _weaviateAppService;

    public ChatbotController(IChatbotAppService chatbotService, IChatAppService chatAppService, IWeaviateAppService weaviateAppService)
    {
        _chatbotService = chatbotService;
        _chatAppService = chatAppService;
        _weaviateAppService = weaviateAppService;
    }

    /// <summary>
    /// Start new chat
    /// </summary>
    [HttpPost("new-chat")]
    public async Task<IActionResult> StartNewChat()
    {
        var activeSession = await _chatAppService.GetActiveSessionAsync("user123");
        if (activeSession != null)
        {
            await _chatAppService.UpdateSessionStatusAsync(activeSession.Id, "not active");
        }
        
        return Ok();
    }


    /// <summary>
    /// Sends a user prompt to the chatbot and returns a response.
    /// </summary>
    [HttpGet("ask")]
    public async Task<IActionResult> AskBot([FromQuery] string prompt, bool knowledge)
    {
        string? response = "null";
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return BadRequest("Prompt cannot be empty.");
        }

        var session = await _chatAppService.GetOrCreateChatSession("user123");

        if (knowledge == false)
        {
            response = await _chatbotService.GetResponse(prompt); 
        }
        else
        {
            var context = await _weaviateAppService.QueryWeaviate(prompt);
            var jsonResponse = await _chatbotService.GetResponseFromWeaviate(context.ToString()); 
            JsonNode rootNode = JsonNode.Parse(jsonResponse);

            response = rootNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
        }
         
        var userMessage = new ChatHistory
        {
            SessionId = session.Id,
            User = "user123", 
            Message = prompt,
            Timestamp = DateTime.UtcNow,
            Role = "user"
        };
        await _chatAppService.SaveMessageAsync(userMessage);

       
        var botMessage = new ChatHistory
        {
            SessionId = session.Id,
            User = "Chatbot", 
            Message = response,
            Timestamp = DateTime.UtcNow,
            Role = "ai"
        };
        await _chatAppService.SaveMessageAsync(botMessage);

        return Ok(new { message = response });
    }
}