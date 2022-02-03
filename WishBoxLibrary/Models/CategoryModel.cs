namespace WishBoxLibrary.Models;

public class CategoryModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ID { get; set; }
    public string CategoryName { get; set; }
    public string CategoryDescription { get; set; }
}
