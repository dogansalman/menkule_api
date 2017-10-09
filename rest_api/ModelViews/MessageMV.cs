using System.ComponentModel.DataAnnotations;

namespace rest_api.ModelViews
{
    public class MessageMV
    {
        [StringLength(255)]
        [EmailAddress]
        public string email { get; set; }
        [StringLength(255)]
        public string name { get; set; }
        [StringLength(500)]
        public string message { get; set; }
    }
}