
namespace WishBoxLibrary.DataAccess;

public interface IUserData
{
    Task CreateUser(UserModel user);
    Task<UserModel> GetUserAsync(string id);
    Task<UserModel> GetUserFromAuthenticationAsync(string objectID);
    Task<List<UserModel>> GetUsersAsync();
    Task UpdateUser(UserModel user);
}