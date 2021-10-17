namespace WAFAYU.DataService.ViewModels
{
    public class OrderDetailUpdateViewModel
    {
        public int BoxId { get; set; }
        public int OrderId { get; set; }
        public int? Type { get; set; }
        public int? BoxId2 { get; set; }
        public string BoxCode { get; set; }
        public int? Status { get; set; }
    }
}
