namespace WAFAYU.DataService.ViewModels
{
    public class FeedbackViewModel
    {
        public int StorageId { get; set; }
        public int OrderId { get; set; }
        public double? Rating { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }

    }
}
