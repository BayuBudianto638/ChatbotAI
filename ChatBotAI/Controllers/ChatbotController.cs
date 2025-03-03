using Microsoft.AspNetCore.Mvc;
using ChatBotAI.Services.ChatbotAppService;
using ChatBotAI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBotAI.Models.Dto;

[ApiController]
[Route("api/chatbot")]
public class ChatbotController : ControllerBase
{
    private readonly IChatbotAppService _chatbotService;

    public ChatbotController(IChatbotAppService chatbotService)
    {
        _chatbotService = chatbotService;
    }

    /// <summary>
    /// Start new chat
    /// </summary>
    [HttpPost("new-chat")]
    public IActionResult StartNewChat()
    {
        _chatbotService.StartNewChat();
        return Ok("New chat session started.");
    }
    
    /// <summary>
    /// Sends a user prompt to the chatbot and returns a response.
    /// </summary>
    [HttpGet("ask")]
    public async Task<IActionResult> AskBot([FromQuery] string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return BadRequest("Prompt cannot be empty.");
        }

        var response = await _chatbotService.GetResponse(prompt);
        return Ok(new { message = response });
    }

    /// <summary>
    /// Reads books from a file and returns the list.
    /// </summary>
    [HttpPost("read-books")]
    [Consumes("multipart/form-data")] 
    public async Task<IActionResult> ReadBooks([FromForm] FileUploadDto file)
    {
        if (file.File == null || file.File.Length == 0)
        {
            return BadRequest("File is required.");
        }

        var tempFilePath = Path.GetTempFileName();

        using (var stream = new FileStream(tempFilePath, FileMode.Create))
        {
            await file.File.CopyToAsync(stream);
        }

        var books = await _chatbotService.ReadBooksFromFile(tempFilePath);

        if (books == null || books.Count == 0)
        {
            return NotFound("No books found in the uploaded file.");
        }

        return Ok(books);
    }
}