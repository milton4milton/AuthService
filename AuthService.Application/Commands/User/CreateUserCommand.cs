using AuthService.Application.DTOs.User;

namespace AuthService.Application.Commands.User;

public class CreateUserCommand
{
    public UserCreateDto UserDto { get; }

    public CreateUserCommand(UserCreateDto dto)
    {
        UserDto = dto;
    }
}
