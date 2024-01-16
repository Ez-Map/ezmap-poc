using FluentValidation;

namespace EzMap.Domain.Dtos;

public class UserSignInDto
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class UserSignInDtoValidator : AbstractValidator<UserSignInDto>
{
    public UserSignInDtoValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}