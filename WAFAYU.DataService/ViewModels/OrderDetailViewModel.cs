namespace WAFAYU.DataService.ViewModels
{
    public class OrderDetailViewModel
    {
        public int BoxId { get; set; }
        public int OrderId { get; set; }
        public decimal? Price { get; set; }
        public int? Type { get; set; }
        public string BoxCode { get; set; }
        public int? BoxId2 { get; set; }
    }
}
