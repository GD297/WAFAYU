using System;
using System.Collections.Generic;

#nullable disable

namespace WAFAYU.DataService.Models
{
    public partial class Feedback
    {
        public int StorageId { get; set; }
        public int OrderId { get; set; }
        public double? Rating { get; set; }
        public string Comment { get; set; }
        public int UserId { get; set; }

        public virtual Order Order { get; set; }
        public virtual Storage Storage { get; set; }
        public virtual User User { get; set; }
    }
}
