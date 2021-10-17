using System;
using System.Collections.Generic;

#nullable disable

namespace WAFAYU.DataService.Models
{
    public partial class User
    {
        public User()
        {
            Feedbacks = new HashSet<Feedback>();
            OrderCustomers = new HashSet<Order>();
            OrderOwners = new HashSet<Order>();
            Storages = new HashSet<Storage>();
        }

        public int Id { get; set; }
        public int? RoleId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public string Uid { get; set; }
        public int? Status { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Order> OrderCustomers { get; set; }
        public virtual ICollection<Order> OrderOwners { get; set; }
        public virtual ICollection<Storage> Storages { get; set; }
    }
}
