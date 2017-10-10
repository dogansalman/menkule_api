using System.ComponentModel.DataAnnotations;

namespace rest_api.ModelViews
{
    public class UserMailMV
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string email { get; set; }
    }
}