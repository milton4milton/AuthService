namespace AuthService.Application.DTOs.Menu;

public class MenuReadDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Url { get; set; } = null!;
    public Guid? ParentId { get; set; }
}
