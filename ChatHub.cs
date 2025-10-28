using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRMinimalApijob3143.Data;
using SignalRMinimalApijob3143.Models;

namespace SignalRMinimalApijob3143;

public class ChatHub : Hub
{
    private readonly ChatDbContext _context;

    public ChatHub(ChatDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Envia uma mensagem para todos os clientes conectados
    /// </summary>
    /// <param name="user">Nome do usuário</param>
    /// <param name="message">Mensagem a ser enviada</param>
    public async Task SendMessageToAll(string user, string message)
    {
        // Salvar mensagem no banco de dados
        var messageEntity = new Message
        {
            User = user,
            Content = message,
            Timestamp = DateTime.UtcNow,
            ConnectionId = Context.ConnectionId
        };

        _context.Messages.Add(messageEntity);
        await _context.SaveChangesAsync();

        // Enviar mensagem para todos os clientes conectados
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    /// <summary>
    /// Método chamado quando um cliente se conecta
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "ChatRoom");
        
        // Enviar histórico de mensagens para o novo usuário
        await SendChatHistoryToUser();
        
        // Notificar outros usuários sobre a nova conexão
        await Clients.Others.SendAsync("UserConnected", $"Um novo usuário se conectou ao chat");
        
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Envia o histórico de mensagens para o usuário que acabou de se conectar
    /// </summary>
    private async Task SendChatHistoryToUser()
    {
        try
        {
            // Buscar as últimas 20 mensagens do banco de dados
            var recentMessages = await _context.Messages
                .OrderByDescending(m => m.Timestamp)
                .Take(20)
                .OrderBy(m => m.Timestamp) // Reordenar para mostrar na ordem cronológica
                .Select(m => new 
                {
                    User = m.User,
                    Content = m.Content,
                    Timestamp = m.Timestamp
                })
                .ToListAsync();

            if (recentMessages.Any())
            {
                // Enviar mensagem informativa sobre o histórico
                await Clients.Caller.SendAsync("ReceiveSystemMessage", 
                    "Sistema", 
                    $"Bem-vindo! Aqui estão as últimas {recentMessages.Count} mensagens:");

                // Enviar cada mensagem do histórico para o usuário que se conectou
                foreach (var msg in recentMessages)
                {
                    await Clients.Caller.SendAsync("ReceiveHistoryMessage", 
                        msg.User, 
                        msg.Content, 
                        msg.Timestamp.ToString("HH:mm"));
                }

                // Separador para distinguir histórico de novas mensagens
                await Clients.Caller.SendAsync("ReceiveSystemMessage", 
                    "Sistema", 
                    "--- Fim do histórico. Novas mensagens aparecerão abaixo ---");
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveSystemMessage", 
                    "Sistema", 
                    "Bem-vindo ao chat! Você é o primeiro usuário ou não há mensagens anteriores.");
            }
        }
        catch (Exception ex)
        {
            // Log do erro (em produção, use um logger apropriado)
            Console.WriteLine($"Erro ao enviar histórico: {ex.Message}");
            
            await Clients.Caller.SendAsync("ReceiveSystemMessage", 
                "Sistema", 
                "Bem-vindo ao chat! Não foi possível carregar o histórico de mensagens.");
        }
    }

    /// <summary>
    /// Método chamado quando um cliente se desconecta
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "ChatRoom");
        await Clients.All.SendAsync("UserDisconnected", $"Usuário {Context.ConnectionId} desconectado");
        await base.OnDisconnectedAsync(exception);
    }
}