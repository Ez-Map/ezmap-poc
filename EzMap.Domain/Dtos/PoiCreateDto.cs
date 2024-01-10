using Azure.Core;
using FluentValidation;

namespace EzMap.Domain.Dtos;

public class PoiCreateDto
{
    public PoiCreateDto(string name, string address)
    {
        Name = name;
        Address = address;
    }

    public string Name { get; init; }
    public string Address { get; init; }
    public Guid UserId { get; private set; }

    public PoiCreateDto WithUserId(Guid userId)
    {
        UserId = userId;
        return this;
    }
}

public class PoiCreateDtoValidator : AbstractValidator<PoiCreateDto>
{
    public PoiCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(50)
            .WithMessage("Name cannot exceed 50 characters.");
        RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required.");
    }
}