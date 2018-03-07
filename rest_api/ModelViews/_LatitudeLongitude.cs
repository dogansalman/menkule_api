using System.ComponentModel.DataAnnotations;

namespace rest_api.ModelViews
{
    public class _LatitudeLongitude
    {
        [Required]
        public string lat { get; set; }
        [Required]
        public string lng { get; set; }
    }
}