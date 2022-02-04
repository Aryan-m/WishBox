
namespace WishBoxLibrary.DataAccess;

public interface IStatusData
{
    Task CreateStatus(StatusModel Status);
    Task<List<StatusModel>> GetStatusesAsync();
}