using FluentValidation;

namespace EzMap.Domain.Dtos;

public class TagCreateDto
{
    public TagCreateDto(string description, string name)
    {
        Description = description;
        Name = name;
    }

    public Guid Id { get; set; }

    public string Description { get; set; }

    public string Name { get; set; }

    public Guid UserId { get; private set; }

    public TagCreateDto WithUserId(Guid userId)
    {
        UserId = userId;
        return this;
    }
}

public class TagCreateDtoValidator : AbstractValidator<TagCreateDto>
{
    public TagCreateDtoValidator()
    {
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(50)
            .WithMessage("Name cannot exceed 50 characters.");
    }
}