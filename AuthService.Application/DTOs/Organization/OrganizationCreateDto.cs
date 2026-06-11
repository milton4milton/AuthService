namespace AuthService.Application.DTOs.Organization;

public class OrganizationCreateDto
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
}
