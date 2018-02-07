using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;

namespace rest_api.Models
{
    public class UserFacebook
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string UserName { get; set; }
        public string ExternalAccessToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public FacebookPictureData Photo { get; set; }
        public class FacebookPictureData
        {
            public FacebookPicture Data { get; set; }
        }
        public class FacebookPicture
        {
            public int Height { get; set; }
            public int Width { get; set; }
            [JsonProperty("is_silhouette")]
            public bool IsSilhouette { get; set; }
            public string Url { get; set; }
        }

        public static UserFacebook FromIdentity(ClaimsIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }
            Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (providerKeyClaim == null || string.IsNullOrEmpty(providerKeyClaim.Issuer) || string.IsNullOrEmpty(providerKeyClaim.Value))
            {
                return null;
            }
            if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
            {
                return null;
            }

            return new UserFacebook
            {
                LoginProvider = providerKeyClaim.Issuer,
                ProviderKey = providerKeyClaim.Value,
                UserName = identity.FindFirstValue(ClaimTypes.Name),
                ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken"),
                Email = identity.Claims.First(c => c.Type == "urn:facebook:email").Value,
                FirstName = identity.Claims.First(c => c.Type == "urn:facebook:first_name").Value,
                LastName = identity.Claims.First(c => c.Type == "urn:facebook:last_name").Value,
                Gender = identity.Claims.First(c => c.Type == "urn:facebook:gender").Value,
                Photo = JsonConvert.DeserializeObject<FacebookPictureData>(identity.Claims.First(c => c.Type == "urn:facebook:picture").Value)
            };
        }
    }
}