namespace AuthService.Domain.Entities;

public class Menu
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int OrderNo { get; set; }

    public Guid? ParentId { get; set; }

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}
