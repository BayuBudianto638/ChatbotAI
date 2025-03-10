using ChatBotAI.Services.ChatAppService;
using Microsoft.AspNetCore.Mvc;

namespace ChatBotAI.Controllers;

[ApiController]
[Route("api/chatbothistory")]
public class ChatHistoryController : ControllerBase
{
    private readonly IChatAppService _chatAppService;
    
    public ChatHistoryController(IChatAppService chatAppService)
    {
        _chatAppService = chatAppService;
    }
    
    [HttpGet("sessions")]
    public async Task<IActionResult> GetAllChatSessions()
    {
        var sessions = await _chatAppService.GetAllSessionsAsync();
        return Ok(sessions);
    }
    
    [HttpGet("history/{sessionId}")]
    public async Task<IActionResult> GetChatHistory(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return BadRequest("Session ID cannot be empty.");
        }

        var history = await _chatAppService.GetChatHistoryAsync(sessionId);
        return Ok(history);
    }
}