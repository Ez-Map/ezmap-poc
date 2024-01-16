using EzMap.Domain.Constants;
using EzMap.Domain.Models;
using FluentValidation;

namespace EzMap.Domain.Dtos;

public class PoiCollectionUpdateDto
{
    public PoiCollectionUpdateDto(Guid id, string name, string description, PoiEnum.ViewType viewType)
    {
        Id = id;
        Name = name;
        Description = description;
        ViewType = viewType;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public List<Poi> Pois { get; } = new();

    public PoiEnum.ViewType ViewType { get; set; } = PoiEnum.ViewType.List;

    public List<Tag> Tags { get; } = new();
}

public class PoiCollectionUpdateDtoValidator : AbstractValidator<PoiCollectionUpdateDto>
{
    public PoiCollectionUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.ViewType).NotEmpty();
    }
}