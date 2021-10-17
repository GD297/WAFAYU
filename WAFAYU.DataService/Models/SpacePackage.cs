using System;
using System.Collections.Generic;

#nullable disable

namespace WAFAYU.DataService.Models
{
    public partial class SpacePackage
    {
        public SpacePackage()
        {
            PendingOrders = new HashSet<PendingOrder>();
        }

        public int Id { get; set; }
        public int StorageId { get; set; }
        public string BoxType { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }

        public virtual Storage Storage { get; set; }
        public virtual ICollection<PendingOrder> PendingOrders { get; set; }
    }
}
