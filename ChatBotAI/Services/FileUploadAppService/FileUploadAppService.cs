using ChatBotAI.Models;
using MongoDB.Driver;

namespace ChatBotAI.Services.FileUploadAppService;

public class FileUploadAppService: IFileUploadAppService
{
    private readonly IMongoCollection<UploadedFile> _uploadedFile;

    public FileUploadAppService(IConfiguration config)
    {
        var connectionString = config["MongoDB:ConnectionString"];
        var databaseName = config["MongoDB:DatabaseName"];
        
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        
        _uploadedFile = database.GetCollection<UploadedFile>("UploadedFiles");
    }

    public async Task SaveFileAsync(UploadedFile message)
    {
        await _uploadedFile.InsertOneAsync(message);
    }

    public async Task<List<UploadedFile>> GetAllFileAsync()
    {
        return await _uploadedFile.Find(_ => true).ToListAsync();
    }

    public async Task<List<UploadedFile>> GetFileByIdAsync(string id)
    {
        return await _uploadedFile
            .Find(m => m.Id == id)
            .ToListAsync();
    }
}