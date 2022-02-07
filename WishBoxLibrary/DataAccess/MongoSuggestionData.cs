namespace WishBoxLibrary.DataAccess;

public class MongoSuggestionData : ISuggestionData
{
    private readonly IDbConnection _db;
    private readonly IUserData _userData;
    private readonly IMemoryCache _cache;
    private const string CACHE_NAME = "SuggestionData";
    public readonly IMongoCollection<SuggestionModel> _suggestions;


    public MongoSuggestionData(IDbConnection db, IUserData userData, IMemoryCache cache)
    {
        this._db = db;
        this._userData = userData;
        this._cache = cache;
        this._suggestions = db.SuggestionCollection;
    }

    public async Task<List<SuggestionModel>> GetSuggestionsAsync()
    {
        var cachedResults = _cache.Get<List<SuggestionModel>>(CACHE_NAME);

        // cache data if not cached
        if (cachedResults == null)
        {
            var results = await _suggestions.FindAsync(x => x.Archived == false);
            cachedResults = results.ToList();

            _cache.Set(CACHE_NAME, cachedResults, TimeSpan.FromMinutes(1));
        }

        return cachedResults.ToList();
    }
    public async Task<List<SuggestionModel>> GetApprovedSuggestionsAsync()
    {
        var suggestions = await this.GetSuggestionsAsync();

        return suggestions.Where(x => x.ApprovedForRelease == true).ToList();
    }

    public async Task<List<SuggestionModel>> GetSuggestionsWaitingApprovalAsync()
    {
        var suggestions = await this.GetSuggestionsAsync();

        return suggestions.Where(x => x.ApprovedForRelease == false
                                   && x.Rejected == false)
              .ToList();
    }

    public async Task<SuggestionModel> GetSuggestionAsync(string id)
    {
        var results = (await this._suggestions.FindAsync(suggestion => suggestion.ID == id)).FirstOrDefault();
        return results;
    }
    public async Task UpdateSuggestion(SuggestionModel suggestion)
    {
        var selectedRec = Builders<SuggestionModel>.Filter.Eq("ID", suggestion.ID);
        await _suggestions.ReplaceOneAsync(selectedRec, suggestion, new ReplaceOptions { IsUpsert = true });

        // reset cache
        _cache.Remove(CACHE_NAME);
    }
    public async Task UpvoteSuggestion(string suggestionID, string userID)
    {
        var client = _db.Client;

        // session is used to handle a transaction so that our
        // database update either completely succeeds or completely
        // fails
        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var suggestionsInTransaction =
                db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);

            // 1. Add user upvote to selected suggestion
            var suggestion = (await suggestionsInTransaction.FindAsync(s => s.ID == suggestionID))
                .First();

            // the below line returns false if userID is already in the hash table
            bool isUpvote = suggestion.UserVotes.Add(userID);

            // if user already voted before, remove their vote
            if (isUpvote == false)
            {
                suggestion.UserVotes.Remove(userID);
            }

            // 2. Add user to list of upvotes for suggestion
            await suggestionsInTransaction.ReplaceOneAsync(s => s.ID == suggestionID, suggestion);

            var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
            var user = await _userData.GetUserAsync(suggestion.Author.ID);

            if (isUpvote)
            {
                user.VotedOnSuggestions.Add(new BasicSuggestionModel(suggestion));
            }
            else
            {
                var suggestionToRemove = user.VotedOnSuggestions.Where(s => s.ID == suggestionID)
                    .First();
            }

            await usersInTransaction.ReplaceOneAsync(u => u.ID == userID, user);
            await session.CommitTransactionAsync();

            _cache.Remove(CACHE_NAME);
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
    public async Task CreateSuggestion(SuggestionModel suggestion)
    {
        var client = _db.Client;

        // session is used to handle a transaction so that our
        // database update either completely succeeds or completely
        // fails
        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var suggestionsInTransaction =
                db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);

            var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
            var user = await _userData.GetUserAsync(suggestion.Author.ID);
            user.AuthoredSuggestions.Add(new BasicSuggestionModel(suggestion));
            await usersInTransaction.ReplaceOneAsync(u => u.ID == user.ID, user);

            await session.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}
