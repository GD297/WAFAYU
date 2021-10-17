using System.Collections.Generic;
using WAFAYU.DataService.Models;

namespace WAFAYU.DataService.AutoMapperConditions
{
    public static class ShelfAutoMapperPrecondition
    {
        public static int CalculateBoxUsaged(ICollection<Box> boxes)
        {
            int total = 12;
            int used = 0;
            foreach (var box in boxes)
            {
                if (box.Status == 2) used++;
            }
            return ((int)(used * 100 / total));
        }
    }
}
