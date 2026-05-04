namespace AuthService.Application.Queries.User;

public class GetUserByIdQuery
{
    public Guid UserId { get; }

    public GetUserByIdQuery(Guid userId)
    {
        UserId = userId;
    }
}
