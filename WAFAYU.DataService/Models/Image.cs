using System;
using System.Collections.Generic;

#nullable disable

namespace WAFAYU.DataService.Models
{
    public partial class Image
    {
        public int Id { get; set; }
        public int? StorageId { get; set; }
        public string ImageUrl { get; set; }
        public int? Type { get; set; }
        public string Location { get; set; }

        public virtual Storage Storage { get; set; }
    }
}
