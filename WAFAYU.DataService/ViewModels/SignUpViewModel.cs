namespace WAFAYU.DataService.ViewModels
{
    public class SignUpViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string RoleName { get; set; }
        public string Avatar { get; set; }
    }
    public class FirebaseSignUpModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public bool returnSecureToken { get; set; }
    }
}
