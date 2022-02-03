using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace WishBoxLibrary.DataAccess;

public class DbConnection
{
    private IConfiguration _config;
    private IMongoDatabase _db;
    private string         _connectionID = "MongoDB";

    public string DbName                   { get; private set; }
    public string CategoryCollectionName   { get; private set; } = "categories";
    public string StatusCollectionName     { get; private set; } = "statuses";
    public string UserCollectionName       { get; private set; } = "users";
    public string SuggestionCollectionName { get; private set; } = "suggestions";
    public MongoClient Client              { get; private set; }
    public IMongoCollection<CategoryModel>   CategoryCollection   { get; private set; }
    public IMongoCollection<StatusModel>     StatusCollection     { get; private set; }
    public IMongoCollection<UserModel>       UserCollection       { get; private set; }
    public IMongoCollection<SuggestionModel> SuggestionCollection { get; private set; }

    public DbConnection(IConfiguration config)
    {
        this._config = config;
        this.Client  = new MongoClient(config.GetConnectionString(this._connectionID));
        this.DbName  = _config["DatabaseName"];
        this._db     = this.Client.GetDatabase(DbName);

        // set up collections
        this.CategoryCollection   = this._db.GetCollection<CategoryModel>  (this.CategoryCollectionName);
        this.StatusCollection     = this._db.GetCollection<StatusModel>    (this.StatusCollectionName);
        this.UserCollection       = this._db.GetCollection<UserModel>      (this.UserCollectionName);
        this.SuggestionCollection = this._db.GetCollection<SuggestionModel>(this.SuggestionCollectionName);
    }
}
