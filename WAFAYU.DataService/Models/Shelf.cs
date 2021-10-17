using System;
using System.Collections.Generic;

#nullable disable

namespace WAFAYU.DataService.Models
{
    public partial class Shelf
    {
        public Shelf()
        {
            Boxes = new HashSet<Box>();
        }

        public int Id { get; set; }
        public int? StorageId { get; set; }
        public string Size { get; set; }
        public int? Status { get; set; }

        public virtual Storage Storage { get; set; }
        public virtual ICollection<Box> Boxes { get; set; }
    }
}
