using System.ComponentModel.DataAnnotations;

namespace EzMap.Domain.Models;

public class Enum
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