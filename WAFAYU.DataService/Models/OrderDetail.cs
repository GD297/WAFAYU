using System;
using System.Collections.Generic;

#nullable disable

namespace WAFAYU.DataService.Models
{
    public partial class OrderDetail
    {
        public int BoxId { get; set; }
        public int OrderId { get; set; }
        public decimal? Price { get; set; }
        public int? Type { get; set; }
        public int? BoxId2 { get; set; }
        public string BoxCode { get; set; }
        public int? Status { get; set; }

        public virtual Box Box { get; set; }
        public virtual Box BoxId2Navigation { get; set; }
        public virtual Order Order { get; set; }
    }
}
