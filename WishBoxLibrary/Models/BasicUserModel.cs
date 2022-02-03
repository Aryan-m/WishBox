namespace WishBoxLibrary.Models;

public class BasicUserModel
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ID { get; set; }
    public string DisplayName { get; set; }
    public BasicUserModel()
    {

    }

    public BasicUserModel(UserModel user)
    {
        this.ID = user.ID;
        this.DisplayName = user.DisplayName;
    }
}
