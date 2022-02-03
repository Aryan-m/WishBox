namespace WishBoxLibrary.Models;

public class StatusModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ID { get; set; }
    public string StatusName { get; set; }
    public string StatusDescription { get; set; }
}
