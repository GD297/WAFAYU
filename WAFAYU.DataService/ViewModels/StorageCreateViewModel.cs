using System.Collections.Generic;

namespace WAFAYU.DataService.ViewModels
{
    public class StorageCreateViewModel
    {
        //public int Id { get; set; }
        //public int? OwnerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        //public int? Rating { get; set; }
        //public string Picture { get; set; }
        public string Description { get; set; }
        public int? ShelvesQuantity { get; set; }
        public int? SmallBoxPrice { get; set; }
        public int? BigBoxPrice { get; set; }
        //public int? Status { get; set; }
        public virtual ICollection<ImageViewModel> Images { get; set; }
    }
}
