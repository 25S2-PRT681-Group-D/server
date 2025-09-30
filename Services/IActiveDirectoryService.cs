using System.DirectoryServices.AccountManagement;

namespace AgroScan.API.Services
{
    public interface IActiveDirectoryService
    {
        Task<bool> ValidateUserAsync(string username, string password);
        Task<UserPrincipal?> GetUserAsync(string username);
        Task<IEnumerable<UserPrincipal>> SearchUsersAsync(string searchTerm);
        Task<IEnumerable<GroupPrincipal>> GetUserGroupsAsync(string username);
        Task<bool> IsUserInGroupAsync(string username, string groupName);
        Task<bool> IsUserActiveAsync(string username);
    }
}
