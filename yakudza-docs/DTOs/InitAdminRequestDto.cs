using System.ComponentModel.DataAnnotations;

namespace yakudza_docs.DTOs;

public class InitAdminRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Login { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}
