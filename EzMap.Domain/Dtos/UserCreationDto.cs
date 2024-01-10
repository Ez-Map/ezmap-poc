using FluentValidation;

namespace EzMap.Domain.Dtos;

public class UserCreationDto
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
}

public class UserCreationDtoValidator : AbstractValidator<UserCreationDto>
{
    public UserCreationDtoValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.").MaximumLength(50)
            .WithMessage("Username cannot exceed 50 characters.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.").MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress()
            .WithMessage("Invalid email format.");
        RuleFor(x => x.DisplayName).MaximumLength(100).WithMessage("Display name cannot exceed 100 characters.");
    }
}