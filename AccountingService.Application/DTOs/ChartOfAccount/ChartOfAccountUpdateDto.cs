using AccountingService.Domain.Enums;

namespace AccountingService.Application.DTOs.ChartOfAccount;

public class ChartOfAccountUpdateDto
{
    public string? Name { get; set; }
    public AccountType? AccountType { get; set; }
    public AccountNature? Nature { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
