using ChatBotAI.Services.ChatbotAppService;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Add Controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register custom service
builder.Services.AddSingleton<IChatbotAppService, ChatbotAppService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chatbot API", Version = "v1" });
    c.OperationFilter<FileUploadOperationFilter>();
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; 
});


var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization(); // Enable Authorization Middleware

app.MapControllers(); // Map Controller Endpoints

app.Run();