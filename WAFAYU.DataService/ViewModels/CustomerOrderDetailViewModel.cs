using System;
using System.Collections.Generic;

namespace WAFAYU.DataService.ViewModels
{
    public class CustomerOrderDetailViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? Months { get; set; }
        public decimal? Total { get; set; }
        public int? SmallBoxQuantity { get; set; }
        public decimal? SmallBoxPrice { get; set; }
        public int? BigBoxQuantity { get; set; }
        public decimal? BigBoxPrice { get; set; }
        public string OwnerPhone { get; set; }
        public string OwnerName { get; set; }
        public string OwnerAvatar { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public double? Rating { get; set; }
        public string Comment { get; set; }
        public int? StorageId { get; set; }
        public int? Status { get; set; }
        public List<BoxUsedViewModel> BoxUsed { get; set; }
    }
}
