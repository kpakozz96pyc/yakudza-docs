namespace yakudza_docs.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
