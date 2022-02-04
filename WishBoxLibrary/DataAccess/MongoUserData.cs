namespace WishBoxLibrary.DataAccess;

public class MongoUserData : IUserData
{
    private readonly IMongoCollection<UserModel> _users;

    public MongoUserData(IDbConnection db)
    {
        this._users = db.UserCollection;
    }

    public async Task<List<UserModel>> GetUsersAsync()
    {
        var results = await this._users.FindAsync(_ => true);
        return results.ToList();
    }

    public async Task<UserModel> GetUserAsync(string id)
    {
        var results = await this._users.FindAsync(user => user.ID == id);
        return results.FirstOrDefault();
    }

    public async Task<UserModel> GetUserFromAuthenticationAsync(string objectID)
    {
        var results = await this._users.FindAsync(user => user.ObjectIdentifier == objectID);
        return results.FirstOrDefault();
    }

    public Task CreateUser(UserModel user)
    {
        return _users.InsertOneAsync(user);
    }

    public Task UpdateUser(UserModel user)
    {
        var selectedUser = Builders<UserModel>.Filter.Eq("ID", user.ID);
        return _users.ReplaceOneAsync(selectedUser, user, new ReplaceOptions { IsUpsert = true });
    }


}
