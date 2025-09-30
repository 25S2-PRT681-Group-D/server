using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace AgroScan.API.Services
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ActiveDirectoryService> _logger;
        private readonly string _domain;
        private readonly string _container;

        public ActiveDirectoryService(IConfiguration configuration, ILogger<ActiveDirectoryService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _domain = _configuration["ActiveDirectory:Domain"] ?? "localhost";
            _container = _configuration["ActiveDirectory:Container"] ?? "CN=Users,DC=domain,DC=com";
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, _domain);
                var isValid = await Task.Run(() => context.ValidateCredentials(username, password));
                
                _logger.LogInformation("User validation result for {Username}: {IsValid}", username, isValid);
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user {Username}", username);
                return false;
            }
        }

        public async Task<UserPrincipal?> GetUserAsync(string username)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, _domain);
                using var user = await Task.Run(() => UserPrincipal.FindByIdentity(context, username));
                
                if (user != null)
                {
                    _logger.LogInformation("User found: {Username} - {DisplayName}", username, user.DisplayName);
                }
                else
                {
                    _logger.LogWarning("User not found: {Username}", username);
                }
                
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {Username}", username);
                return null;
            }
        }

        public async Task<IEnumerable<UserPrincipal>> SearchUsersAsync(string searchTerm)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, _domain);
                using var searcher = new UserPrincipal(context);
                
                searcher.Name = $"*{searchTerm}*";
                
                using var search = new PrincipalSearcher(searcher);
                var results = await Task.Run(() => search.FindAll().Cast<UserPrincipal>().ToList());
                
                _logger.LogInformation("Found {Count} users matching search term: {SearchTerm}", results.Count, searchTerm);
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
                return Enumerable.Empty<UserPrincipal>();
            }
        }

        public async Task<IEnumerable<GroupPrincipal>> GetUserGroupsAsync(string username)
        {
            try
            {
                using var context = new PrincipalContext(ContextType.Domain, _domain);
                using var user = await Task.Run(() => UserPrincipal.FindByIdentity(context, username));
                
                if (user == null)
                {
                    _logger.LogWarning("User not found for group lookup: {Username}", username);
                    return Enumerable.Empty<GroupPrincipal>();
                }

                var groups = await Task.Run(() => user.GetGroups().Cast<GroupPrincipal>().ToList());
                
                _logger.LogInformation("User {Username} is member of {Count} groups", username, groups.Count);
                return groups;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting groups for user {Username}", username);
                return Enumerable.Empty<GroupPrincipal>();
            }
        }

        public async Task<bool> IsUserInGroupAsync(string username, string groupName)
        {
            try
            {
                var groups = await GetUserGroupsAsync(username);
                var isInGroup = groups.Any(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
                
                _logger.LogInformation("User {Username} in group {GroupName}: {IsInGroup}", username, groupName, isInGroup);
                return isInGroup;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {Username} is in group {GroupName}", username, groupName);
                return false;
            }
        }

        public async Task<bool> IsUserActiveAsync(string username)
        {
            try
            {
                var user = await GetUserAsync(username);
                if (user == null)
                {
                    return false;
                }

                var isActive = user.Enabled == true && user.AccountExpirationDate == null;
                
                _logger.LogInformation("User {Username} active status: {IsActive}", username, isActive);
                return isActive;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {Username} is active", username);
                return false;
            }
        }
    }
}
