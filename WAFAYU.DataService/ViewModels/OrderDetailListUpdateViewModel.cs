using System.Collections.Generic;

namespace WAFAYU.DataService.ViewModels
{
    public class OrderDetailListUpdateViewModel
    {
        public virtual ICollection<OrderDetailUpdateViewModel> OrderDetails { get; set; }
        public string MailMessage { get; set; }
    }
}
