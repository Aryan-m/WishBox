
namespace WishBoxLibrary.DataAccess;

public interface ISuggestionData
{
    Task CreateSuggestion(SuggestionModel suggestion);
    Task<List<SuggestionModel>> GetApprovedSuggestionsAsync();
    Task<SuggestionModel> GetSuggestionAsync(string id);
    Task<List<SuggestionModel>> GetSuggestionsAsync();
    Task<List<SuggestionModel>> GetSuggestionsWaitingApprovalAsync();
    Task UpdateSuggestion(SuggestionModel suggestion);
    Task UpvoteSuggestion(string suggestionID, string userID);
}