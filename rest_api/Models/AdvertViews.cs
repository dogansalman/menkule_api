using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api.Models
{
    public class AdvertViews
    {
        [Key]
     
        public int id { get; set; }
        [Index("IX_AdvertView", 1, IsUnique = true)]
        public int advert_id { get; set; }
        [StringLength(16)]
        [Index("IX_AdvertView", 2, IsUnique = true)]
        public string ip { get; set; }

    }
}