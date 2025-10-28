using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRMinimalApijob3143;
using SignalRMinimalApijob3143.Data;
using SignalRMinimalApijob3143.Models;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços SignalR
builder.Services.AddSignalR();

// Configurar Entity Framework com In-Memory Database
builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseInMemoryDatabase("ChatDatabase"));

// Configurar CORS para permitir conexões do frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Usar CORS
app.UseCors();

// Mapear o Hub do SignalR
app.MapHub<ChatHub>("/chathub");

// Endpoint para enviar mensagem para todos
app.MapPost("/api/sendmessage", async (IHubContext<ChatHub> hubContext, ChatDbContext context, MessageRequest request) =>
{
    // Salvar mensagem no banco de dados
    var messageEntity = new Message
    {
        User = request.User,
        Content = request.Message,
        Timestamp = DateTime.UtcNow,
        ConnectionId = "API"
    };

    context.Messages.Add(messageEntity);
    await context.SaveChangesAsync();

    // Enviar mensagem para todos os clientes conectados
    await hubContext.Clients.All.SendAsync("ReceiveMessage", request.User, request.Message);
    return Results.Ok(new { success = true, message = "Mensagem enviada para todos!", messageId = messageEntity.Id });
});


// Endpoint para obter informações sobre a aplicação
app.MapGet("/", () => 
{
    return Results.Ok(new 
    { 
        message = "SignalR Minimal API com Entity Framework está funcionando!", 
        endpoints = new 
        {
            hub = "/chathub",
            sendMessage = "/api/sendmessage",
            getMessages = "/api/messages?limit=50",
            getUserMessages = "/api/messages/user/{user}?limit=20",
            getStats = "/api/stats",
            testPage = "/test"
        }
    });
});


app.Run();
