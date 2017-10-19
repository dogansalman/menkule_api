using System.ComponentModel.DataAnnotations;

namespace rest_api.ModelViews
{
    public class _AdvertImages
    {
        [Required]
        public int id { get; set; }
        public bool is_default { get; set; } = false;
        public bool deleted { get; set; } = false;
        public bool is_new { get; set; } = false;
        [Required]
        public string url { get; set; }
    }
}