using System.ComponentModel.DataAnnotations;

namespace SignalRMinimalApijob3143.Models;

/// <summary>
/// Entidade que representa uma mensagem no chat
/// </summary>
public class Message
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string User { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public string ConnectionId { get; set; } = string.Empty;
}