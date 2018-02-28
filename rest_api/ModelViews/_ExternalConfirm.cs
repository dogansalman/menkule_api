using rest_api.Filters;
using System.ComponentModel.DataAnnotations;


namespace rest_api.ModelViews
{
    public class _ExternalConfirm
    {
        [Required]
        public string password { get; set; }
        [Required]
        public string reply { get; set; }
        [PhoneMask("0000000000")]
        [StringLength(11)]
        [Required]
        public string gsm { get; set; }

    }
}