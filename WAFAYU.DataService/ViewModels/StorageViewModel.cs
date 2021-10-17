using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using WAFAYU.DataService.Attributes;

namespace WAFAYU.DataService.ViewModels
{
    public class StorageViewModel
    {
        public static string[] Fields = {
            "Id","Address","Rating","Name","Description","Status","OwnerName","Images","OwnerId","NumberOfRatings","RemainingBoxes","IsSortedPrice","IsSortedRating"
        };
        [BindNever]
        public int? Id { get; set; }
        [BindNever]
        public int? OwnerId { get; set; }
        [String]
        public string Address { get; set; }
        [BindNever]
        public double? Rating { get; set; }
        [BindNever]
        public int? NumberOfRatings { get; set; }
        [BindNever]
        public string Name { get; set; }
        [BindNever]
        public string Description { get; set; }
        [BindNever]
        public int? Status { get; set; }
        [BindNever]
        public string OwnerName { get; set; }
        [BindNever]
        public string OwnerPhone { get; set; }
        [BindNever]
        public string OwnerAvatar { get; set; }
        [BindNever]
        public decimal? PriceFrom { get; set; }
        [BindNever]
        public decimal? PriceTo { get; set; }
        [BindNever]
        public int? RemainingBoxes { get; set; }
        [BindNever]
        public string RejectedReason { get; set; }
        public bool? IsSortedPrice { get; set; }
        public bool? IsSortedRating { get; set; }
        [BindNever]
        public virtual ICollection<ImageUpdateViewModel> Images { get; set; }
    }
}
