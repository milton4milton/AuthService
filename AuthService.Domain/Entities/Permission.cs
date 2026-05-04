namespace AuthService.Domain.Entities;

public class Permission
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!; // USER_VIEW
    public string Name { get; set; } = null!;

    public Guid ModuleId { get; set; }
    public Module Module { get; set; } = null!;
}
