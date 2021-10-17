using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WAFAYU.DataService.ViewModels
{
    public class ShelfViewModel
    {
        public static string[] Fields = {
            "Id","StorageId","Size","Usage"
        };
        [BindNever]
        public int? Id { get; set; }
        public int? StorageId { get; set; }
        [BindNever]
        public string Size { get; set; }
        [BindNever]
        public int? Status { get; set; }
        [BindNever]
        public int? Usage { get; set; }
    }
}
