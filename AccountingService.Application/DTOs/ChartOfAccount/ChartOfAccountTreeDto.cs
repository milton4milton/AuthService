namespace AccountingService.Application.DTOs.ChartOfAccount;

/// <summary>Recursive tree node — used for the full hierarchy API.</summary>
public class ChartOfAccountTreeDto
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid? ParentId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string AccountType { get; set; } = null!;
    public string Nature { get; set; } = null!;
    public bool IsGroup { get; set; }
    public int Level { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<ChartOfAccountTreeDto> Children { get; set; } = new();
}
