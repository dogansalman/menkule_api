using System.ComponentModel.DataAnnotations;
namespace rest_api.ModelViews
{
    public class UserApproveMV
    {
        [StringLength(255)]
        [Required]
        public string code { get; set; }
    }
}