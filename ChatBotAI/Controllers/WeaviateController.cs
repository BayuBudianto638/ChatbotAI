using ChatBotAI.Models;
using ChatBotAI.Services.ChatAppService;
using ChatBotAI.Services.ChatbotAppService;
using ChatBotAI.Services.WeaviateAppService;
using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.Globalization;
using ChatBotAI.Services.FileUploadAppService;
using CsvHelper.Configuration;

namespace ChatBotAI.Controllers;

[ApiController]
[Route("api/Knowledge")]
public class WeaviateController : ControllerBase
{
    private readonly IWeaviateAppService _weaviateAppService;
    private readonly IFileUploadAppService _fileUploadAppService;

    public WeaviateController(IWeaviateAppService weaviateAppService, IFileUploadAppService fileUploadAppService)
    {
        _weaviateAppService = weaviateAppService;
        _fileUploadAppService = fileUploadAppService;
    }
    
    // <summary>
    /// Upload Data from CSV
    /// </summary>
    [HttpPost("UploadData")]
    public async Task<IActionResult> UploadData(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using (var reader = new StreamReader(file.OpenReadStream()))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<ArticleMap>();

            var records = csv.GetRecords<Article>();

            foreach (var article in records)
            {
                _weaviateAppService.UploadData(article);
            }
        }
        
        var uploadedFile = new UploadedFile
        {
            FileName = file.FileName,
            createdBy = "User123",
            UploadedAt = DateTime.Now,
            WeaviateObjectId = "10001"
        };

        await _fileUploadAppService.SaveFileAsync(uploadedFile);

        return Ok("Data uploaded successfully.");
    }
    
    [HttpGet("GetListFiles")]
    public async Task<IActionResult> GetAllChatSessions()
    {
        var data = await _fileUploadAppService.GetAllFileAsync();
        return Ok(data);
    }
    
    private sealed class ArticleMap : ClassMap<Article>
    {
        public ArticleMap()
        {
            Map(m => m.Title).Name("title");
            Map(m => m.Content).Name("content");
        }
    }
}