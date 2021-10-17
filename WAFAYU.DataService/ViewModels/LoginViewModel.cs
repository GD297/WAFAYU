using System.Collections.Generic;

namespace WAFAYU.DataService.ViewModels
{
    public class LoginViewModel
    {
        //public string email { get; set; }
        //public string password { get; set; }
        public string idToken { get; set; }
    }
    public class FirebaseLoginViewModel
    {
        //public string email { get; set; }
        //public string password { get; set; }
        //public bool returnSecureToken { get; set; }
        public string idToken { get; set; }

    }
    public class FirebaseModel
    {
        public IList<FirebaseUserViewModel> users { get; set; }
    }
    public class FirebaseUserViewModel
    {
        public string email { get; set; }
        public string localId { get; set; }
    }
    public class TokenViewModel
    {
        public string displayName { get; set; }
        public string roleName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string email { get; set; }
        public string Avatar { get; set; }
        public string idToken { get; set; }
        public string refreshToken { get; set; }
        public double expiresIn { get; set; }
        public string localId { get; set; }
        public string tokenType { get; set; }
        public string role { get; set; }
    }
    public class TokenModel
    {
        public string idToken { get; set; }
        public string refreshToken { get; set; }
        public double expiresIn { get; set; }
        public string tokenType { get; set; }
    }
}
