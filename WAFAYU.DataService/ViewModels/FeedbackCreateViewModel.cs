namespace WAFAYU.DataService.ViewModels
{
    public class FeedbackCreateViewModel
    {
        public int StorageId { get; set; }
        public int OrderId { get; set; }
        public double? Rating { get; set; }
        public string Comment { get; set; }
    }
}
