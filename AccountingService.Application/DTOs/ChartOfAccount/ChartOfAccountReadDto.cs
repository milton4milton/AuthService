namespace AccountingService.Application.DTOs.ChartOfAccount;

public class ChartOfAccountReadDto
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentCode { get; set; }
    public string? ParentName { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string AccountType { get; set; } = null!;
    public string Nature { get; set; } = null!;
    public bool IsGroup { get; set; }
    public int Level { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
