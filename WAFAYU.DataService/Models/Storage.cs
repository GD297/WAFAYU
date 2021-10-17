using System;
using System.Collections.Generic;

#nullable disable

namespace WAFAYU.DataService.Models
{
    public partial class Storage
    {
        public Storage()
        {
            Feedbacks = new HashSet<Feedback>();
            Images = new HashSet<Image>();
            Shelves = new HashSet<Shelf>();
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

        public virtual User Owner { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Shelf> Shelves { get; set; }
        public virtual ICollection<SpacePackage> SpacePackages { get; set; }
    }
}
