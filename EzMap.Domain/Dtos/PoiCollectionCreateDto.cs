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
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}