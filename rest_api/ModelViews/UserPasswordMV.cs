using System.ComponentModel.DataAnnotations;

namespace rest_api.ModelViews
{
    public class UserPasswordMV
    {
        [Required]
        [StringLength(255)]
        public string currentpassword { get; set; }
        [Required]
        [StringLength(255)]
        public string password { get; set; }
        [Required]
        [StringLength(255)]
        public string reply { get; set; }
    }
}