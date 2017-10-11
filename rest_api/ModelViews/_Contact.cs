using System.ComponentModel.DataAnnotations;

namespace rest_api.ModelViews
{
    public class _Contact
    {
        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string email { get; set; }
        [StringLength(255)]
        [Required]
        public string name { get; set; }
        [StringLength(500)]
        [Required]
        public string message { get; set; }
    }
}