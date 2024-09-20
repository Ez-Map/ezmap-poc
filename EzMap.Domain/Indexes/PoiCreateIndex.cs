using FluentValidation;

namespace EzMap.Domain.Indexes;

public class PoiCreateIndex
{
    public PoiCreateIndex(string name, string address)
    {
        Name = name;
        Address = address;
    }
    
    public string Name { get; set; }
    
    public string Address { get; set; }
}

public class PoiCreateIndexValidator : AbstractValidator<PoiCreateIndex>
{
    public PoiCreateIndexValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Address).NotEmpty();
    }
}