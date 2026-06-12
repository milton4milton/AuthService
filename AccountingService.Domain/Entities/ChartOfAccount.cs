using AccountingService.Domain.Enums;

namespace AccountingService.Domain.Entities;

public class ChartOfAccount
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid? ParentId { get; set; }              // null = root node
    public string Code { get; set; } = null!;        // e.g. "1000", "1100", "1110"
    public string Name { get; set; } = null!;
    public AccountType AccountType { get; set; }     // Asset | Liability | Equity | Revenue | Expense
    public AccountNature Nature { get; set; }        // Debit | Credit
    public bool IsGroup { get; set; }                // true = group/header, false = leaf/ledger
    public int Level { get; set; }                   // 0 = root, 1 = child, 2 = grandchild …
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ChartOfAccount? Parent { get; set; }
    public ICollection<ChartOfAccount> Children { get; set; } = new List<ChartOfAccount>();
}
