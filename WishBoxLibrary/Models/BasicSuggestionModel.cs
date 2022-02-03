namespace WishBoxLibrary.Models;

public class BasicSuggestionModel
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ID { get; set; }
    public string Title { get; set; }
    public BasicSuggestionModel()
    {

    }

    public BasicSuggestionModel(SuggestionModel suggestion)
    {
        this.ID = suggestion.ID;
        this.Title = suggestion.Title;
    }

}
