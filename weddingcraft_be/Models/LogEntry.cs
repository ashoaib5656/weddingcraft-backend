using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace weddingcraft_be.Models;

[Table("logs", Schema = "public")]
public class LogEntry
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("message")]
    public string? Message { get; set; }

    [Column("message_template")]
    public string? MessageTemplate { get; set; }

    [Column("level")]
    public string? Level { get; set; }

    [Column("timestamp")]
    public DateTime Timestamp { get; set; }

    [Column("exception")]
    public string? Exception { get; set; }

    [Column("properties", TypeName = "jsonb")]
    public string? Properties { get; set; }

    [Column("useremail")]
    public string? UserEmail { get; set; }

    [Column("endpoint")]
    public string? Endpoint { get; set; }

    [Column("ipaddress")]
    public string? IpAddress { get; set; }
}
