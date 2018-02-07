using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using rest_api.Filters;
using rest_api.Context;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace rest_api.Models
{
    public class Users
    {
        
        public object getPhotosUrl()
        {
            using (var db = new DatabaseContext())
            {
                return db.images.Where(img => img.id == this.image_id).FirstOrDefault()?.url;
            }
        }

        /* Generate Random Password
           * Char max len 24
           * Num max len 10
        */
        public static string generatePassword(int charLen, int numLen)
        {
            char[] chars = new char[] { 'a', 'A', 'J', 'm', 'I', 'i', 'P', 'p', 'U', 's', 'g', 'E', 'e', 'Z', 'L', 'q', 'h', 'H', '_', '!', 'W', 'w', 'F', 'f' };
            int[] numbs = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            Random rnd = new Random();
            string password = "";
            for (int c = 0; c < charLen; c++)
            {
                password += chars[rnd.Next(0, (chars.Length - 1))].ToString();
            }
            for (int n = 0; n < numLen; n++)
            {
                password += numbs[rnd.Next(0, (numbs.Length - 1))].ToString();
            }

            return password;
        }

        public static object LoginOnBackDoor(string userName, string password)
        {
            HttpClient client = new HttpClient();
            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "password" ),
                        new KeyValuePair<string, string>( "username", userName ),
                        new KeyValuePair<string, string> ( "Password", password )
                    };
            var content = new FormUrlEncodedContent(pairs);
            // Attempt to get a token from the token endpoint of the Web Api host:
            HttpResponseMessage response = client.PostAsync("https://webapi.menkule.com.tr/auth", content).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            // De-Serialize into a dictionary and return:
            Dictionary<string, string> tokenDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            return tokenDictionary;
        }
        [Key]
        public int id { get; set; }
        private string _name;
        [Required]
        [StringLength(255)]
        public string name
        {
            get { return _name; }
            set { _name = value.ToLower(); }
        }
        private string _lastname;
        [Required]
        [StringLength(255)]
        public string lastname
        {
            get { return _lastname; }
            set { _lastname = value.ToLower(); }
        }
        private string _email;
        [Required]
        [StringLength(255)]
        [EmailAddress]
        [Index("IX_UserEmail", 1, IsUnique = true)]
        public string email
        {
            get { return _email; }
            set { _email = value.ToLower(); }
        }
        [StringLength(255)]
        public string password { get; set; }
        [Required]
        [PhoneMask("0000000000")]
        [StringLength(11)]
        [Index("IX_UserGsm", 2, IsUnique = true)]
        public string gsm { get; set; }
        public int? image_id { get; set; }
        public bool email_state { get; set; } = true;
        public bool gsm_state { get; set; } = false;
        [StringLength(255)]
        public string email_activation_code { get; set; }
        [StringLength(255)]
        public string gsm_activation_code { get; set; }
        [StringLength(5)]
        [Gender]
        public string gender { get; set; }
        [StringLength(90)]
        public string source { get; set; }
        [StringLength(255)]
        public string facebook_id { get; set; }
        public DateTime? gsm_last_update { get; set; }
        public bool ownershiping { get; set; }
        [StringLength(11)]
        [MinLength(11)]
        public string identity_no { get; set; } = null;
        public bool state { get; set; }
        [StringLength(255)]
        public string description { get; set; }
        public DateTime? forgot_last_date { get; set; }
        [StringLength(255)]
        public string password_token { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }

    }


}