using FluentValidation;

namespace EzMap.Domain.Dtos;

public class PoiUpdateDto
{
    
    public PoiUpdateDto(Guid id, string name, string address)
    {
        Id = id;
        Name = name;
        Address = address;
    }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    
    public Guid UserId { get; private set; }
    
    public PoiUpdateDto WithUserId(Guid userId)
    {
        UserId = userId;
        return this;
    }
}

public class PoiUpdateDtoValidator : AbstractValidator<PoiUpdateDto>
{
    public PoiUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotNull().WithMessage("ID is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required.");
    }
}