using System.Security.Cryptography;
using System.Text;
using ROSE.Core.Configuration;
using ROSE.Core.Services;

namespace ROSE.Infrastructure.Services;

public class MachineKeyService : IMachineKeyService
{
    public string GetOrCreateMachineKey()
    {
        if (File.Exists(AppPaths.MachineKeyPath))
        {
            return LoadMachineKey();
        }

        var key = GenerateRandomKey();
        SaveMachineKey(key);
        return key;
    }

    private string GenerateRandomKey()
    {
        using var rng = RandomNumberGenerator.Create();
        var keyBytes = new byte[32];
        rng.GetBytes(keyBytes);
        return Convert.ToBase64String(keyBytes);
    }

    private void SaveMachineKey(string key)
    {
        File.WriteAllText(AppPaths.MachineKeyPath, key);
    }

    private string LoadMachineKey()
    {
        return File.ReadAllText(AppPaths.MachineKeyPath);
    }
} 