namespace AuthService.Application.Queries.Menu;

public class GetUserMenuQuery
{
    // User Id
    public Guid UserId { get; }

    // User এর Role Ids
    public List<Guid> RoleIds { get; }

    // Constructor
    public GetUserMenuQuery(Guid userId, List<Guid> roleIds)
    {
        UserId = userId;
        RoleIds = roleIds;
    }
}
