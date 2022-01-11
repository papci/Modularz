using System.ComponentModel.DataAnnotations;
using UnitSense.Extensions.Encryption;

namespace Modularz.Data.EF;

public class BlogUser
{
    [Key]
    public int Id { get; set; }

    public string AccountName  { get; set; }
    public string Email { get; set; }
    public DateTime DateCreated { get; set; }
    public string HashedPassword { get; set; }
    public string Salt { get; set; }
    public string TempToken { get; set; }
    public UserState State { get; set; }
    public UserRank Rank { get; set; }
    
    public enum UserState
    {
        Pending,
        Activated,
        Banned
    }
    
    public enum UserRank
    {
        User,
        Admin
        
    }

    public bool PasswordIsValid(string password)    
    {
      return HashEncryption.SHA256Hash($"{Salt}{password}") == HashedPassword;
    }
}