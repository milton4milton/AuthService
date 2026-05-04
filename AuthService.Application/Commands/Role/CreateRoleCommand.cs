using AuthService.Application.DTOs.Role;

namespace AuthService.Application.Commands.Role;

public class CreateRoleCommand
{
    public RoleCreateDto RoleDto { get; }

    public CreateRoleCommand(RoleCreateDto dto)
    {
        RoleDto = dto;
    }
}
