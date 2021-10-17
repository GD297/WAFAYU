using System.Collections.Generic;

namespace WAFAYU.DataService.ViewModels
{
    public class StorageCreateSuccessViewModel
    {
        public int Id { get; set; }
        public virtual ICollection<ImageUpdateViewModel> Images { get; set; }
    }
}
