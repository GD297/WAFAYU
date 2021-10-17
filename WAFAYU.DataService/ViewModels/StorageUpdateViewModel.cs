using System.Collections.Generic;

namespace WAFAYU.DataService.ViewModels
{
    public class StorageUpdateViewModel
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public virtual ICollection<ImageUpdateViewModel> Images { get; set; }
    }
}
