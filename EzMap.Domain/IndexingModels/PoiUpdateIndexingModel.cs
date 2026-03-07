using FluentValidation;

namespace EzMap.Domain.Indexes;

public class PoiUpdateIndexingModel
{
    public PoiUpdateIndexingModel(Guid poiId, string name, string address)
    {
        Id = poiId;
        Name = name;
        Address = address;
    }


    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }
}