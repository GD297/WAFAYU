namespace WAFAYU.DataService.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public string confirmPassword { get; set; }
        public string firebaseTokenId { get; set; }
    }
    public class FirebaseChangePasswordModel
    {
        public string idToken { get; set; }
        public string password { get; set; }
        public bool returnSecureToken { get; set; }
    }
}
