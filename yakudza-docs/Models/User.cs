namespace yakudza_docs.Models;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;
}
