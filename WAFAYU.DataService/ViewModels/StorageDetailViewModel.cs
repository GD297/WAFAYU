using System.Collections.Generic;
using WAFAYU.DataService.Models;

namespace WAFAYU.DataService.ViewModels
{
    public class StorageDetailViewModel
    {
        public StorageDetailViewModel()
        {
            Feedbacks = new HashSet<Feedback>();
            Images = new HashSet<ImageViewModel>();
            Shelves = new HashSet<ShelfViewModel>();
            SpacePackages = new HashSet<SpacePackage>();
        }

        public int Id { get; set; }
        public int? OwnerId { get; set; }
        public string Address { get; set; }
        public string Picture { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
        public string RejectedReason { get; set; }
        public int RemainingBoxes { get; set; }

        public virtual User Owner { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<ImageViewModel> Images { get; set; }
        public virtual ICollection<ShelfViewModel> Shelves { get; set; }
        public virtual ICollection<SpacePackage> SpacePackages { get; set; }
    }
}
