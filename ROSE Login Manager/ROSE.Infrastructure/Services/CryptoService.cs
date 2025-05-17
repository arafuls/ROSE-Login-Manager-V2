using System.Security.Cryptography;
using System.Text;
using ROSE.Core.Configuration;
using ROSE.Core.Services;

namespace ROSE.Infrastructure.Services;

public class CryptoService : ICryptoService
{
    private readonly IMachineKeyService _machineKeyService;

    public CryptoService(IMachineKeyService machineKeyService)
    {
        _machineKeyService = machineKeyService;
    }

    public byte[] EncryptDatabase(byte[] data)
    {
        var key = _machineKeyService.GetOrCreateMachineKey();
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(key);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        msEncrypt.Write(aes.IV, 0, aes.IV.Length);

        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            csEncrypt.Write(data, 0, data.Length);
        }

        return msEncrypt.ToArray();
    }

    public byte[] DecryptDatabase(byte[] encryptedData)
    {
        var key = _machineKeyService.GetOrCreateMachineKey();
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(key);

        using var msDecrypt = new MemoryStream(encryptedData);
        var iv = new byte[16];
        msDecrypt.Read(iv, 0, iv.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var msPlain = new MemoryStream();
        csDecrypt.CopyTo(msPlain);

        return msPlain.ToArray();
    }

    public byte[] EncryptPassword(string password, out byte[] iv)
    {
        using var aes = Aes.Create();
        aes.GenerateIV();
        iv = aes.IV;

        var key = _machineKeyService.GetOrCreateMachineKey();
        aes.Key = Convert.FromBase64String(key);

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(password);
        }

        return msEncrypt.ToArray();
    }

    public string DecryptPassword(byte[] encryptedPassword, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.IV = iv;

        var key = _machineKeyService.GetOrCreateMachineKey();
        aes.Key = Convert.FromBase64String(key);

        using var decryptor = aes.CreateDecryptor();
        using var msDecrypt = new MemoryStream(encryptedPassword);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        return srDecrypt.ReadToEnd();
    }
} 