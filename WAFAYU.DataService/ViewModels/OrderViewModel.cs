using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace WAFAYU.DataService.ViewModels
{
    public class CustomerOrderViewModel
    {
        public static string[] Fields = {
            "Id","Name","Address","Total","SmallBoxQuantity","SmallBoxPrice","BigBoxQuantity","BigBoxPrice","Months","Status"
                ,"ExpiredDate","OwnerName","OwnerPhone","OwnerAvatar","StorageId","Rating","Comment","PickupTime","BoxUsed"
        };
        [BindNever]
        public int? Id { get; set; }
        [BindNever]
        public string Name { get; set; }
        [BindNever]
        public string Address { get; set; }
        [BindNever]
        public decimal? Total { get; set; }
        [BindNever]
        public int? SmallBoxQuantity { get; set; }
        [BindNever]
        public decimal? SmallBoxPrice { get; set; }
        [BindNever]
        public int? BigBoxQuantity { get; set; }
        [BindNever]
        public decimal? BigBoxPrice { get; set; }
        [BindNever]
        public int? Months { get; set; }
        [BindNever]
        public int? Status { get; set; }
        [BindNever]
        public DateTime? ExpiredDate { get; set; }
        [BindNever]
        public string OwnerName { get; set; }
        [BindNever]
        public string OwnerPhone { get; set; }
        public string OwnerAvatar { get; set; }
        public double? Rating { get; set; }
        public DateTime? PickupTime { get; set; }
        public string Comment { get; set; }
        public int? StorageId { get; set; }
        public List<BoxUsedViewModel> BoxUsed { get; set; }
    }
    public class OwnerOrderViewModel
    {
        public static string[] Fields = {
            "Id","PaidTime","Name","Address","PickupTime","Total","SmallBoxQuantity","SmallBoxPrice","BigBoxQuantity","BigBoxPrice","Months","Status"
                ,"CustomerPhone","CustomerName","CustomerAvatar","StorageId","ExpiredDate","Rating","Comment","PickupTime","BoxUsed"
        };
        [BindNever]
        public int? Id { get; set; }
        [BindNever]
        public string Name { get; set; }
        [BindNever]
        public string Address { get; set; }
        [BindNever]
        public decimal? Total { get; set; }
        [BindNever]
        public int? Months { get; set; }
        [BindNever]
        public int? SmallBoxQuantity { get; set; }
        [BindNever]
        public decimal? SmallBoxPrice { get; set; }
        [BindNever]
        public int? BigBoxQuantity { get; set; }
        [BindNever]
        public decimal? BigBoxPrice { get; set; }
        [BindNever]
        public string CustomerPhone { get; set; }
        [BindNever]
        public string CustomerName { get; set; }
        [BindNever]
        public string CustomerAvatar { get; set; }
        [BindNever]
        public DateTime? ExpiredDate { get; set; }
        public double? Rating { get; set; }
        public string Comment { get; set; }
        public DateTime? PickupTime { get; set; }
        public int? StorageId { get; set; }
        [BindNever]
        public int? Status { get; set; }
        public List<BoxUsedViewModel> BoxUsed { get; set; }
    }
    public class OrderPaymentViewModel
    {
        public static string[] Fields = {
            "Id","PaidTime","PickupTime","Total","SmallBoxQuantity","SmallBoxPrice","BigBoxQuantity","BigBoxPrice","Months","Status"
                ,"CustomerPhone","CustomerName","CustomerAvatar"
        };
        [BindNever]
        public int StorageId { get; set; }
        //[BindNever]
        //public DateTime? PaidTime { get; set; }
        [BindNever]
        public DateTime? PickupTime { get; set; }
        [BindNever]
        public decimal Total { get; set; }
        [BindNever]
        public int Months { get; set; }
        [BindNever]
        public int SmallBoxQuantity { get; set; }
        [BindNever]
        public decimal SmallBoxPrice { get; set; }
        [BindNever]
        public int BigBoxQuantity { get; set; }
        [BindNever]
        public decimal BigBoxPrice { get; set; }
        public int? Rating { get; set; }
    }
}
