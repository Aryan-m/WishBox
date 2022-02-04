
namespace WishBoxLibrary.DataAccess;

public interface ICategoryData
{
    Task CreateCategory(CategoryModel category);
    Task<List<CategoryModel>> GetCategoriesAsync();
}