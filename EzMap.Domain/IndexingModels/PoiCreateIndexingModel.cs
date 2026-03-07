using FluentValidation;

namespace EzMap.Domain.Indexes;

public class PoiCreateIndexingModel
{
    public PoiCreateIndexingModel(Guid id, string name, string address)
    {
        Id = id;
        Name = name;
        Address = address;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }
}