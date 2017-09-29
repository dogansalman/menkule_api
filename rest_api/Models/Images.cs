using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace rest_api.Models
{
    public class Images
    {
        [Key]
        public int id { get; set; }
        [Required]
        [StringLength(255)]
        public string url { get; set; }
    }
}