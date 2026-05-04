using AuthService.Application.DTOs.Role;

namespace AuthService.Application.Commands.Role;

public class UpdateRoleCommand
{
    public Guid RoleId { get; }
    public RoleUpdateDto RoleDto { get; }

    public UpdateRoleCommand(Guid roleId, RoleUpdateDto dto)
    {
        RoleId = roleId;
        RoleDto = dto;
    }
}
