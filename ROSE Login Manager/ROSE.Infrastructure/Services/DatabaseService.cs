using Microsoft.EntityFrameworkCore;
using ROSE.Core.Configuration;
using ROSE.Core.Models;
using ROSE.Core.Services;
using ROSE.Infrastructure.Data;

namespace ROSE.Infrastructure.Services;

public class DatabaseService : IDatabaseService
{
    private readonly ICryptoService _cryptoService;
    private readonly IDbContextFactory<ProfileDbContext> _dbContextFactory;

    public DatabaseService(
        ICryptoService cryptoService,
        IDbContextFactory<ProfileDbContext> dbContextFactory)
    {
        _cryptoService = cryptoService;
        _dbContextFactory = dbContextFactory;
    }

    public void InitializeDatabase()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        dbContext.Database.EnsureCreated();
    }

    public async Task ExportDatabaseAsync(string filePath)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var databaseBytes = await File.ReadAllBytesAsync(AppPaths.DatabasePath);
        var encryptedBytes = _cryptoService.EncryptDatabase(databaseBytes);
        await File.WriteAllBytesAsync(filePath, encryptedBytes);
    }

    public async Task ImportDatabaseAsync(string filePath)
    {
        var encryptedBytes = await File.ReadAllBytesAsync(filePath);
        var databaseBytes = _cryptoService.DecryptDatabase(encryptedBytes);
        await File.WriteAllBytesAsync(AppPaths.DatabasePath, databaseBytes);
    }

    public bool IsDatabaseInitialized()
    {
        return File.Exists(AppPaths.DatabasePath);
    }

    public async Task<IEnumerable<Profile>> GetAllProfilesAsync()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.Profiles.ToListAsync();
    }

    public async Task<Profile?> GetProfileAsync(string profileName)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.Profiles.FindAsync(profileName);
    }

    public async Task AddProfileAsync(Profile profile, string plainPassword)
    {
        byte[] iv;
        var encryptedPassword = _cryptoService.EncryptPassword(plainPassword, out iv);
        profile.EncryptedPassword = Convert.ToBase64String(encryptedPassword);
        profile.PasswordIV = iv;

        using var context = _dbContextFactory.CreateDbContext();
        context.Profiles.Add(profile);
        await context.SaveChangesAsync();
    }

    public async Task UpdateProfileAsync(Profile profile, string? newPlainPassword = null)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var existing = await context.Profiles.FindAsync(profile.ProfileName);
        if (existing == null)
            throw new InvalidOperationException($"Profile '{profile.ProfileName}' not found");

        if (newPlainPassword != null)
        {
            byte[] iv;
            var encryptedPassword = _cryptoService.EncryptPassword(newPlainPassword, out iv);
            profile.EncryptedPassword = Convert.ToBase64String(encryptedPassword);
            profile.PasswordIV = iv;
        }
        else
        {
            profile.EncryptedPassword = existing.EncryptedPassword;
            profile.PasswordIV = existing.PasswordIV;
        }

        context.Entry(existing).CurrentValues.SetValues(profile);
        await context.SaveChangesAsync();
    }

    public async Task DeleteProfileAsync(string profileName)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var profile = await context.Profiles.FindAsync(profileName);
        if (profile != null)
        {
            context.Profiles.Remove(profile);
            await context.SaveChangesAsync();
        }
    }
} 