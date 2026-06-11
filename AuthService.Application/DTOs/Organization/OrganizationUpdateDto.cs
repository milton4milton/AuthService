namespace AuthService.Application.DTOs.Organization;

public class OrganizationUpdateDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
    public bool? IsActive { get; set; }
}
