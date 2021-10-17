namespace WAFAYU.DataService.ViewModels
{
    public class ImageUpdateViewModel
    {
        public int Id { get; set; }
        public int? StorageId { get; set; }
        public string ImageUrl { get; set; }
        public int? Type { get; set; }
        public string Location { get; set; }
    }
}
