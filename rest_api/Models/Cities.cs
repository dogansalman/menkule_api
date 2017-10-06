using System.ComponentModel.DataAnnotations;

namespace rest_api.Models
{
    public class Cities
    {
        [Key]
        public int id { get; set; }
        [Required]
        [StringLength(255)]
        public string name { get; set; }

    }
}