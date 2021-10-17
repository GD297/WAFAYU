using System;
using System.Collections.Generic;

#nullable disable

namespace WAFAYU.DataService.Models
{
    public partial class Box
    {
        public Box()
        {
            OrderDetailBoxId2Navigations = new HashSet<OrderDetail>();
            OrderDetailBoxes = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public int? ShelfId { get; set; }
        public string Position { get; set; }
        public int? Type { get; set; }
        public string Size { get; set; }
        public decimal? Price { get; set; }
        public string BoxCode { get; set; }
        public int? Status { get; set; }

        public virtual Shelf Shelf { get; set; }
        public virtual ICollection<OrderDetail> OrderDetailBoxId2Navigations { get; set; }
        public virtual ICollection<OrderDetail> OrderDetailBoxes { get; set; }
    }
}
