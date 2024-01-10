using FluentValidation;

namespace EzMap.Domain.Dtos;

public class TagUpdateDto
{
    public TagUpdateDto(Guid id, string description, string name)
    {
        Id = id;
        Description = description;
        Name = name;
    }

    public Guid Id { get; set; }

    public string Description { get; set; }

    public string Name { get; set; }

    public Guid UserId { get; private set; }

    public TagUpdateDto WithUserId(Guid userId)
    {
        UserId = userId;
        return this;
    }
}

public class TagUpdateDtoValidator : AbstractValidator<TagUpdateDto>
{
    public TagUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ID is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(50)
            .WithMessage("Name cannot exceed 50 characters.");
        // Assuming UserId is not required for updates
    }
}