using ROSE.Core.Models;

namespace ROSE.Core.Services;

public interface IDatabaseService
{
    // Database initialization
    void InitializeDatabase();
    bool IsDatabaseInitialized();
    
    // Import/Export
    Task ExportDatabaseAsync(string filePath);
    Task ImportDatabaseAsync(string filePath);
    
    // Profile operations
    Task<IEnumerable<Profile>> GetAllProfilesAsync();
    Task<Profile?> GetProfileAsync(string profileName);
    Task AddProfileAsync(Profile profile, string plainPassword);
    Task UpdateProfileAsync(Profile profile, string? newPlainPassword = null);
    Task DeleteProfileAsync(string profileName);
} 