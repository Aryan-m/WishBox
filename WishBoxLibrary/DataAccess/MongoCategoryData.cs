using Microsoft.Extensions.Caching.Memory;

namespace WishBoxLibrary.DataAccess;

public class MongoCategoryData : ICategoryData
{
    private readonly IMongoCollection<CategoryModel> _categories;
    private readonly IMemoryCache _cache;
    private const string _cacheName = "CategoryData";

    public MongoCategoryData(IDbConnection db, IMemoryCache cache)
    {
        this._categories = db.CategoryCollection;
        this._cache = cache;
    }

    public async Task<List<CategoryModel>> GetCategoriesAsync()
    {
        var cachedResults = _cache.Get<List<CategoryModel>>(_cacheName);

        // cache data if not cached
        if (cachedResults == null)
        {
            var results = await _categories.FindAsync(_ => true);
            cachedResults = results.ToList();

            _cache.Set(_cacheName, cachedResults, TimeSpan.FromDays(1));
        }

        return cachedResults.ToList();
    }

    public Task CreateCategory(CategoryModel category)
    {
        return _categories.InsertOneAsync(category);
    }
}
