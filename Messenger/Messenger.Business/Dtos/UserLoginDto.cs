using System.ComponentModel.DataAnnotations;

namespace Messenger.Business.Dtos;

public class UserLoginDto
{
    public string UserName { get; init; }

    public string Password { get; init; }
}
