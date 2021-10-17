using AutoMapper;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WAFAYU.DataService.Constants;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.Repositories;
using WAFAYU.DataService.Responses;
using WAFAYU.DataService.UnitOfWorks;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.Services
{
    public interface IUserService : IBaseService<User>
    {
        Task<TokenViewModel> Login(LoginViewModel model);
        Task<TokenViewModel> SignUp(SignUpViewModel model);
        Task<TokenViewModel> ChangePassword(ChangePasswordViewModel model);
        Task<UpdateProfileViewModel> UpdateProfile(UpdateProfileViewModel model, string idToken);
        User GetUser(string uid);
        Task<User> GetUser(int id);
    }
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IRoleService _roleService;
        private readonly IWebHostEnvironment env;
        public UserService(IUnitOfWork unitOfWork, IUserRepository repository, IMapper mapper, IRoleService roleService, IWebHostEnvironment _env) : base(unitOfWork, repository)
        {
            _mapper = mapper;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://identitytoolkit.googleapis.com")
            };
            _roleService = roleService;
            env = _env;
        }
        public bool CheckPassword(string uid, string password)
        {
            var result = Get(x => x.Uid == uid && x.Password == password).FirstOrDefault();
            if (result != null) return true;
            return false;
        }
        public async Task<TokenViewModel> ChangePassword(ChangePasswordViewModel model)
        {
            var secureToken = new JwtSecurityTokenHandler().ReadJwtToken(model.firebaseTokenId);
            var uid = secureToken.Claims.First(claim => claim.Type == "user_id").Value;
            if (!CheckPassword(uid, model.oldPassword)) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Password is invalid");
            if (model.oldPassword == model.newPassword) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "New password must different from old password");
            if (model.newPassword != model.confirmPassword) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Confirm password not matched");
            var firebaseModel = _mapper.Map<FirebaseChangePasswordModel>(model);
            firebaseModel.idToken = model.firebaseTokenId;
            var url = string.Format("/v1/accounts:update?key=AIzaSyAB2yOeCYLksKBNl5gYnvgFc2xXibeBEyY");
            var response = await _httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(firebaseModel), Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
                string errorMessage = errorResponse.SelectToken("$.error.message").Value<string>();
                throw new ErrorResponse((int)response.StatusCode, errorMessage);
            }
            var stringResponse = await response.Content.ReadAsStringAsync();
            var entity = Get(x => x.Uid == uid).FirstOrDefault();
            entity.Password = model.newPassword;
            await UpdateAsync(entity);
            var result = _mapper.Map<TokenViewModel>(JObject.Parse(stringResponse));
            return result;
        }

        public User GetUserByEmail(string email)
        {
            var user = Get(x => x.Email == email).FirstOrDefault();
            if (user == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Email can not found");
            return user;
        }
        public string GetRoleName(string email)
        {
            var user = Get(x => x.Email == email).FirstOrDefault();
            if (user == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Email can not found");
            string roleName = _roleService.GetRoleName((int)user.RoleId);
            return roleName;
        }
        public async Task<TokenViewModel> Login(LoginViewModel model)
        {
            var url = string.Format("/v1/accounts:lookup?key=AIzaSyAB2yOeCYLksKBNl5gYnvgFc2xXibeBEyY");
            var firebaseModel = _mapper.Map<FirebaseLoginViewModel>(model);
            var response = await _httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(firebaseModel), Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
                string errorMessage = errorResponse.SelectToken("$.error.message").Value<string>();
                throw new ErrorResponse((int)response.StatusCode, errorMessage);
            }
            var stringResponse = await response.Content.ReadAsStringAsync();
            var firebaseResult = _mapper.Map<FirebaseModel>(JObject.Parse(stringResponse));
            var firebaseUser = firebaseResult.users.FirstOrDefault();
            var result = _mapper.Map<TokenViewModel>(firebaseUser);
            var user = GetUser(result.localId);
            result = _mapper.ConfigurationProvider.CreateMapper().Map(user, result);
            result.roleName = GetRoleName(result.email);
            var token = GenerateToken(user, result.roleName);
            var refreshToken = GenerateRefreshToken(user, result.roleName);
            token.refreshToken = refreshToken;
            result = _mapper.Map(token, result);
            return result;
        }
        public async Task<TokenViewModel> SignUp(SignUpViewModel model)
        {
                var url = string.Format("/v1/accounts:signUp?key=AIzaSyAB2yOeCYLksKBNl5gYnvgFc2xXibeBEyY");
                var firebaseModel = _mapper.Map<FirebaseSignUpModel>(model);
                var response = await _httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(firebaseModel), Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
                    string errorMessage = errorResponse.SelectToken("$.error.message").Value<string>();
                    throw new ErrorResponse((int)response.StatusCode, errorMessage);
                }
                var stringResponse = await response.Content.ReadAsStringAsync();
                var result = _mapper.Map<TokenViewModel>(JObject.Parse(stringResponse));
                var entity = _mapper.Map<User>(model);
                entity.RoleId = _roleService.GetRoleId(model.RoleName);
                entity.Uid = result.localId;
                await CreateAsync(entity);

                if (FirebaseApp.DefaultInstance == null)
                {
                    string tmp = "{\n  \"type\": \"service_account\",\n  \"project_id\": \"wafayu-82753\",\n  \"private_key_id\": \"24fc65fb6ed4e1ce4f5434f61ebf050b6b267ba1\",\n  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDwsVYVwVIsGagF\\nOfTJLWcbYlDyPSsfIOa3hK+Zgptjg3nbZOvC9KjsmmU1NebP+UGsIHG0lXUo1U3o\\neK5RzF0u5T6Eh2Awioc9YF17q5/THp8XZiENaY8Un3fRHMsVadUGF76HWA8FpOMj\\nVSSJtc87OAPaOnD90LFPcWn8QhZ+4UdO9Grr8oQLUSfTcxGBDMyHA6KxnNi7NWbv\\nhBHmnIuLWN6fcoOO3DXnWB8T3LKMHbrr8uMkfe4Pp6uMQs2ib8lNYUQqqOk7GTp/\\nmGdl7RLeIhlENmzFcV6uS9tDzQa/D/OvKID1EsCrFiWEH5dF4aklyqqP+qbpbqEz\\njsMfwzuXAgMBAAECggEADTt1Q+9464Uw5nkZs9lyF/WyzFEjykDshUv7oBLSXgIh\\nqbez6/YNRWNZILfQK869SOwr6zrI54HWic6nUrg9VTiB8MFholX6bCoAIpHKl/ht\\nKaBSIL9cDjCUm6qboEIinK8xYZYBeWogmQ7W9o/WR+dU11UdgTzpvFSflKBP8Gbi\\nwhDpoWNlTt5ki5stgg+aCYyxOMbXI4KHK9a43lBIOfbUGOL663st4gFWf7ihFQ7+\\nIFB0wpkGkBy+iOXQDrKlt1+FrsAjXdZHj4JZzGYAvQjolRYFoeALjqTyqpnLeQIr\\n73wA9whKDKGy5f7hEHHZnd+ijNfNiB0uk4/WMKlwjQKBgQD9iaZvRS1sgUq0CVPt\\nNRcOxh2XNdBx3KGQumST8Z+ZQs6xeiHWQmv+7SgmjcTp8MgUiQFFmo/+yil6bKVE\\nrR0lcA8yhU8563/2CPspPiJa1qIctbMliEA+92Sjq6+2TQx/47Dra2y/5ibgaRSt\\nU5PzVpQcPBfssmr2R7qcP3kqowKBgQDzB8AvzoCgVz9dLM5p3EG+wt0n4iBKM8ot\\n7pErhGXZcNOsIDixzCMgwpesHzDwQb1Df5aOQS60LlvxGT5HoNWKpq8BmJ7y/6JL\\nY+XnegzBwMOe5LhmCDKKtvdK47e4EgXA8X0eNqT5jhyJVZMUJxQN4SfllxiwcG4B\\nMp+YHnGOfQKBgQDAI4sQFDriWfFFEzSZNXyt45IVP+EiCPQf/EGwpJIw0aJWeZof\\nB+NUJGIjJZkvORxIA7QFDadGjep1LtTogNDvyEacIM7zs/cbe57rmHzsnm/olgKX\\n1PPrRAzuxHO7EhUA7orrPlQB8edQdhIHcKlU5i4EX7450NSio6VWN/wZRQKBgHjd\\nRO5Bo+SaH3AmRW2OVcAaR0R7iBV/FjCrsdTTnpd6LjsvNyLtZBb5z9aRGJSUDR1R\\nib2rmx+O4rjQFhInYav345dGoN421i7qOr9ZCpkdVexi0XYk44XrIqdDAu5vQB9J\\nujDZekiGLZj7Tw85tBaHAIGFGIk/EOZVl9teYJk9AoGBAIKmBc4eJu796EQ1+UpW\\nuULJwxeHtOzVG2Anh668AJnXZBXTiOrwGYIGF5uyDUfb6/0kOnCfHMx1yid60vjd\\nKpSVtbzuHMpfT71geA2M/FEcwIVOqHbwY5RdJFnHwiK/XYReTFiC/uOfigqWT1zl\\nU5upSJat30JbKzXs2obc7Hrj\\n-----END PRIVATE KEY-----\\n\",\n  \"client_email\": \"firebase-adminsdk-5jees@wafayu-82753.iam.gserviceaccount.com\",\n  \"client_id\": \"116204506323899846175\",\n  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-5jees%40wafayu-82753.iam.gserviceaccount.com\"\n}\n";
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromJson(tmp)
                    });
                }
                var claims = new Dictionary<string, object>
                {
                    {ClaimTypes.Role, model.RoleName }
                };
                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(result.localId, claims);
                return result;
        }

        public async Task<UpdateProfileViewModel> UpdateProfile(UpdateProfileViewModel model, string idToken)
        {
            var secureToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
            var uid = secureToken.Claims.First(claim => claim.Type == "user_id").Value;
            var entity = Get(x => x.Uid == uid).FirstOrDefault();
            var updateEntity = _mapper.ConfigurationProvider.CreateMapper().Map(model, entity);
            await UpdateAsync(updateEntity);
            return model;
        }

        public User GetUser(string uid)
        {
            var user = Get(x => x.Uid == uid).FirstOrDefault();
            if (user == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Email can not found");
            return user;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await GetAsync(id);
            //if(user == null) throw new ErrorResponse((int)HttpStatusCode.BadRequest, "Id can not found");
            return user;
        }
        private TokenModel GenerateToken(User user, string role)
        {
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim("user_id",user.Uid),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
            authClaims.Add(new Claim(ClaimTypes.Role, role));
            //string secret = "https://securetoken.google.com/wafayu-82753";
            string secret = SecretKeyConstant.SECRET_KEY;
            //var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            //var token = new JwtSecurityToken(
            //    issuer: secret,
            //    audience: "wafayu-82753",
            //    expires: DateTime.Now.AddMinutes(60),
            //    claims: authClaims,
            //    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            //    );
            IdentityModelEventSource.ShowPII = true;

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var token = new JwtSecurityToken(
                issuer: SecretKeyConstant.ISSUER,
                audience: SecretKeyConstant.ISSUER,
                expires: DateTime.Now.AddMinutes(60),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            TimeSpan expires = DateTime.Now.Subtract(token.ValidTo);
            return new TokenModel
            {
                idToken = new JwtSecurityTokenHandler().WriteToken(token),
                expiresIn = expires.TotalMinutes,
                tokenType = "Bearer",
            };
        }
        private string GenerateRefreshToken(User user, string role)
        {
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim("user_id",user.Uid),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
            authClaims.Add(new Claim(ClaimTypes.Role, role));
            //string secret = "https://securetoken.google.com/wafayu-82753";
            //string secret = "this is secret key mot hai ba bon nam sau bay";
            string secret = SecretKeyConstant.SECRET_KEY;
            IdentityModelEventSource.ShowPII = true;
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var token = new JwtSecurityToken(
                //issuer: "this is issuer fht mot hai",
                issuer: SecretKeyConstant.ISSUER,
                audience: SecretKeyConstant.ISSUER,
                expires: DateTime.Now.AddMonths(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
