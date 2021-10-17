using System.Collections.Generic;

namespace WAFAYU.DataService.ViewModels
{
    public class OrderDetailListViewModel
    {
        public virtual ICollection<OrderDetailViewModel> OrderDetails { get; set; }
        public string MailMessage { get; set; }
    }
}
