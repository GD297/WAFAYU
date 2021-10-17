using System;
using System.Collections.Generic;

#nullable disable

namespace WAFAYU.DataService.Models
{
    public partial class Order
    {
        public Order()
        {
            Feedbacks = new HashSet<Feedback>();
            OrderDetails = new HashSet<OrderDetail>();
            PendingOrders = new HashSet<PendingOrder>();
        }

        public int Id { get; set; }
        public int? OwnerId { get; set; }
        public int? CustomerId { get; set; }
        public string Size { get; set; }
        public decimal? Total { get; set; }
        public int? Status { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAvatar { get; set; }
        public int? Months { get; set; }
        public DateTime? PaidTime { get; set; }
        public DateTime? PickupTime { get; set; }

        public virtual User Customer { get; set; }
        public virtual User Owner { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<PendingOrder> PendingOrders { get; set; }
    }
}
