using FluentValidation;

namespace EzMap.Domain.Dtos;

public class PoiCollectionCreateDto
{
    public PoiCollectionCreateDto(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; set; }

    public string Description { get; init; }

    public Guid UserId { get; private set; }

    public PoiCollectionCreateDto WithUserId(Guid userId)
    {
        UserId = userId;
        return this;
    }
}

public class PoiCollectionCreateDtoValidator : AbstractValidator<PoiCollectionCreateDto>
{
    public PoiCollectionCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters.");
        RuleFor(x => x.Description).MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}