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
        RuleFor(x => x.Id).NotEmpty().WithMessage("ID is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters.");
        RuleFor(x => x.Description).MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        RuleFor(x => x.ViewType).NotEmpty().WithMessage("View type is required.");
    }
}