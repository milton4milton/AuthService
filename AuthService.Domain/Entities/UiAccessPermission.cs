namespace AuthService.Domain.Entities;

public class UiAccessPermission
{
    public Guid Id { get; set; }
    public string ComponentName { get; set; } = null!; // Button, Tab
    public Guid PermissionId { get; set; }

    public Permission Permission { get; set; } = null!;
}
