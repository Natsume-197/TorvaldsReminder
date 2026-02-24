using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TorvaldsReminder.Domain.Enums;

namespace TorvaldsReminder.Domain.Entities;

public class Invoice
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string        Id       { get; set; } = string.Empty;
    public string        ClientId { get; set; } = string.Empty;
    public string        Number   { get; set; } = string.Empty;
    public decimal       Amount   { get; set; }
    public DateTime      DueDate  { get; set; }

    [BsonRepresentation(BsonType.String)]
    public InvoiceStatus Status   { get; set; }
}