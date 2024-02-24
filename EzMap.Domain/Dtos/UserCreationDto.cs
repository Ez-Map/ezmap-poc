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
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.DisplayName).MaximumLength(100);
    }
}