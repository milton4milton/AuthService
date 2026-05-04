using AuthService.Application.DTOs.User;

namespace AuthService.Application.Commands.User;

public class UpdateUserCommand
{
    public Guid UserId { get; }
    public UserUpdateDto UserDto { get; }

    public UpdateUserCommand(Guid userId, UserUpdateDto dto)
    {
        UserId = userId;
        UserDto = dto;
    }
}
