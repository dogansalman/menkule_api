using System.ComponentModel.DataAnnotations;

namespace rest_api.ModelViews
{
    public class _TokenResetPassword
    {
        [Required]
        [StringLength(255)]
        public string token { get; set; }
        [Required]
        [StringLength(255)]
        public string password { get; set; }
        [Required]
        [StringLength(255)]
        public string reply { get; set; }
    }
}