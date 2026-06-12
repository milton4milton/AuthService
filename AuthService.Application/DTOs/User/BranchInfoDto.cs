namespace AuthService.Application.DTOs.User;

public class BranchInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
}
