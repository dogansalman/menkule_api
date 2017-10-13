using System.ComponentModel.DataAnnotations;

namespace rest_api.ModelViews
{
    public class _LatitudeLongitude
    {
        [Required]
        public double lat { get; set; }
        [Required]
        public double lng { get; set; }
    }
}