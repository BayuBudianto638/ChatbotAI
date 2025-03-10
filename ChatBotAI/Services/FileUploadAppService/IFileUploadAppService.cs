using ChatBotAI.Models;

namespace ChatBotAI.Services.FileUploadAppService;

public interface IFileUploadAppService
{
    Task SaveFileAsync(UploadedFile message);
    Task<List<UploadedFile>> GetAllFileAsync();
    Task<List<UploadedFile>> GetFileByIdAsync(string id);
}