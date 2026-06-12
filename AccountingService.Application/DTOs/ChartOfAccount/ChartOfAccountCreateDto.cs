using AccountingService.Domain.Enums;

namespace AccountingService.Application.DTOs.ChartOfAccount;

public class ChartOfAccountCreateDto
{
    public Guid OrganizationId { get; set; }
    public Guid? ParentId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public AccountType AccountType { get; set; }
    public AccountNature Nature { get; set; }
    public bool IsGroup { get; set; }
    public string? Description { get; set; }
}
