using System.ComponentModel.DataAnnotations;

namespace ROSE.Core.Models;

public class Profile
{
    [Key]
    public string ProfileName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    // This will store the encrypted password
    public string EncryptedPassword { get; set; } = string.Empty;
    
    public string Status { get; set; } = string.Empty;
    
    // Salt used for password encryption
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    
    // IV used for password encryption
    public byte[] PasswordIV { get; set; } = Array.Empty<byte>();
} 