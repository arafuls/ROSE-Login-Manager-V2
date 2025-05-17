namespace ROSE.Core.Services;

public interface ICryptoService
{
    // Password encryption
    byte[] EncryptPassword(string password, out byte[] iv);
    string DecryptPassword(byte[] encryptedPassword, byte[] iv);
    
    // Database encryption
    byte[] EncryptDatabase(byte[] data);
    byte[] DecryptDatabase(byte[] encryptedData);
} 