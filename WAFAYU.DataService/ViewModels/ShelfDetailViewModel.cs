using System.Collections.Generic;

namespace WAFAYU.DataService.ViewModels
{
    public class ShelfDetailViewModel
    {
        public int Id { get; set; }
        public int? StorageId { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<BoxViewModel> Boxes { get; set; }
    }
}
