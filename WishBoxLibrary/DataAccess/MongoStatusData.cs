namespace WishBoxLibrary.DataAccess;

public class MongoStatusData : IStatusData
{
    private readonly IMongoCollection<StatusModel> _statuses;
    private readonly IMemoryCache                  _cache;
    private const    string                        CACHE_NAME = "StatusData";

    public MongoStatusData(IDbConnection db, IMemoryCache cache)
    {
        this._statuses = db.StatusCollection;
        this._cache = cache;
    }

    public async Task<List<StatusModel>> GetStatusesAsync()
    {
        var cachedResults = _cache.Get<List<StatusModel>>(CACHE_NAME);

        // cache data if not cached
        if (cachedResults == null)
        {
            var results = await _statuses.FindAsync(_ => true);
            cachedResults = results.ToList();

            _cache.Set(CACHE_NAME, cachedResults, TimeSpan.FromDays(1));
        }

        return cachedResults.ToList();
    }

    public Task CreateStatus(StatusModel Status)
    {
        return _statuses.InsertOneAsync(Status);
    }
}
