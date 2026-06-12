namespace AccountingService.Application.DTOs.FinancialYear;

public class FinancialYearCreateDto
{
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
