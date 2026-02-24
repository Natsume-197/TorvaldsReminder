using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TorvaldsReminder.Domain.Entities;

public class Client
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id    { get; set; } = string.Empty;
    public string Name  { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}