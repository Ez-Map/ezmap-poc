using System.Runtime.InteropServices.JavaScript;

namespace EzMap.Domain.Models;

public class EntityBase<T>
{
    public virtual T Id { get; set; }

    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public DateTime? DeletedDate { get; set; }
}