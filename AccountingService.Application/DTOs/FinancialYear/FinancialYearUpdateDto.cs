namespace AccountingService.Application.DTOs.FinancialYear;

public class FinancialYearUpdateDto
{
    public string? Name { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsActive { get; set; }
}
