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
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.").MaximumLength(50)
            .WithMessage("Username cannot exceed 50 characters.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.").MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");
    }
}