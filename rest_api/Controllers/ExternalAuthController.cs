using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Web.Helpers;
using rest_api.OAuth.FacebookResults;
using rest_api.Models;
using rest_api.Context;
using rest_api.Libary.Bcrypt;
using rest_api.Libary.Cloudinary;
using rest_api.OAuth.AuthBackdoor;
using rest_api.Libary.StringEncode;

namespace rest_api.Controllers
{
    [RoutePrefix("social")]
    public class ExternalAuthController : ApiController
    {

        DatabaseContext db = new DatabaseContext();

        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [HttpGet]
        [Route("facebook/sing-in", Name = "social/facebook/sing-in")]
        public async Task<IHttpActionResult> facebookLogin(string provider, string error = null)
        {
            string redirectUri = string.Empty;
            Dictionary<string, string> AccessToken = null;

            // Validate request
            if (error != null) return BadRequest(Uri.EscapeDataString(error));

            // Exist Identity
            if (!User.Identity.IsAuthenticated) return new ChallengeResult(provider, this);

            // Validate redirect url
            var redirectUriValidationResult = ValidateClientAndRedirectUri(Request, ref redirectUri);
            if (!string.IsNullOrWhiteSpace(redirectUriValidationResult)) return BadRequest(redirectUriValidationResult);

            // User data from Identity
            UserFacebook externalLogin = UserFacebook.FromIdentity(User.Identity as ClaimsIdentity);
            if (externalLogin == null) return InternalServerError();

            // Exist user in db
            Users user = db.users.Where(u => (u.facebook_id == externalLogin.ProviderKey && u.email == externalLogin.Email && u.source == "facebook")).FirstOrDefault();
            bool hasRegistered = user != null;

            // If not identity register to db add new user
            if (!hasRegistered)
            {

                try
                {
                    // check user e-mail exist validation
                    if (db.users.Any(u => u.email == externalLogin.Email)) return Redirect(GenerateErrorRedirect(externalLogin, "e-posta zaten kullanilmakta", redirectUri));

                    string password = Users.generatePassword(5, 3);

                    //generate activation code
                    Random rnd = new Random();
                    string gsm_code = rnd.Next(9999, 999999).ToString();
                    string email_code = rnd.Next(9999, 999999).ToString();

                    // disable db validations
                    db.Configuration.ValidateOnSaveEnabled = false;

                    // create user 
                    Users userData = new Users
                    {
                        name = externalLogin.FirstName,
                        lastname = externalLogin.LastName,
                        email = externalLogin.Email,
                        facebook_id = externalLogin.ProviderKey,
                        gender = externalLogin.Gender == "male" ? "Bay" : "Bayan",
                        gsm = string.Empty,
                        password = Bcrypt.hash(password),
                        source = "facebook",
                        email_activation_code = email_code,
                        gsm_activation_code = gsm_code,
                        is_external_confirm = false,

                    };
                    db.users.Add(userData);
                    
                    // save photos
                    byte[] imageData = null;
                    using (var wc = new System.Net.WebClient())
                        imageData = wc.DownloadData(externalLogin.Photo.Data.Url);
                    MemoryStream photoStreamData = new MemoryStream(imageData);

                    // send cloud
                    var image = new WebImage(photoStreamData);
                    var httpRequest = HttpContext.Current.Request;
                    Images userImage = Cloudinary.upload(image, "users/" + userData.name.ReduceWhitespace().Replace(" ", "-").ToEng() + "-" + userData.lastname.ReduceWhitespace().Replace(" ", "-").ToEng() + "-" + userData.id);
                    if(userImage != null)
                    {
                        db.images.Add(userImage);
                        db.SaveChanges();
                        userData.image_id = userImage.id;
                    }
                    
                    db.SaveChanges();

                    // enable db validations
                    db.Configuration.ValidateOnSaveEnabled = false;

                    return Redirect(GenerateRedirectUrl(AuthBackdoor.auth(userData, Request), externalLogin, hasRegistered, redirectUri));
                }
                catch (Exception ex)
                {
                    return Redirect(GenerateErrorRedirect(externalLogin, ex.Message.ToString(), redirectUri));
                }
            }
            return Redirect(GenerateRedirectUrl(AuthBackdoor.auth(user, Request), externalLogin, hasRegistered,redirectUri));
        }

        private string GenerateRedirectUrl(AuthenticationProperties prop, UserFacebook externalLogin, bool hasRegistered, string redirectUri)
        {
            dynamic timeoffset = prop.ExpiresUtc - DateTimeOffset.UtcNow;          
            return  string.Format("{0}?external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}&access_token={5}&refresh_token={6}&expires_in={7}&token_type={8}",
                            redirectUri,
                            externalLogin.ExternalAccessToken,
                            externalLogin.LoginProvider,
                            hasRegistered.ToString(),
                            externalLogin.UserName,
                            prop.Dictionary["access_token"],
                            prop.Dictionary["refresh_token"],
                            timeoffset.Ticks.ToString().Substring(0, 5),
                            "bearer"
                            );
        }
        private string GenerateErrorRedirect(UserFacebook externalLogin, string message, string redirectUri)
        {
            return string.Format("{0}?email={1}&provider={2}&err={3}",
                            redirectUri,
                            externalLogin.Email,
                            externalLogin.LoginProvider,
                            message
                            );
        }

        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {

            Uri redirectUri;

            var redirectUriString = GetQueryString(Request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            var clientId = GetQueryString(Request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return "client_Id is required";
            }
            


            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;

        }
        private string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }

    }
}
