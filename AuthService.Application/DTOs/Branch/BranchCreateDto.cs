namespace AuthService.Application.DTOs.Branch;

public class BranchCreateDto
{
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}
