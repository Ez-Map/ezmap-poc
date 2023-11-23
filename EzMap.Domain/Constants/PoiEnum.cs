using System.ComponentModel.DataAnnotations;

namespace EzMap.Domain.Constants;

public abstract class PoiEnum
{
    public enum ViewType
    {
        [Display(Name = "Map")]
        Map = 1,
        [Display(Name = "List")]
        List = 2,
        [Display(Name = "Grid")]
        Grid = 3,
    }
}