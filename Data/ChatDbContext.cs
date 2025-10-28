using Microsoft.EntityFrameworkCore;
using SignalRMinimalApijob3143.Models;

namespace SignalRMinimalApijob3143.Data;

/// <summary>
/// Contexto do banco de dados para o chat
/// </summary>
public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
    {
    }

    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações adicionais do modelo
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.User).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.ConnectionId).HasMaxLength(100);
            
            // Índice para melhorar performance de consultas por timestamp
            entity.HasIndex(e => e.Timestamp);
        });
    }
}