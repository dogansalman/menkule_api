using System.ComponentModel.DataAnnotations;
using rest_api.Context;

namespace rest_api.Models
{
    public class Images
    {
        public Images add(string url)
        {
            using (var db = new DatabaseContext())
            {
                var img = new Images
                {
                    url = url
                };
                db.images.Add(img);
                db.SaveChanges();
                return img;
            }
        }

        [Key]
        public int id { get; set; }
        [Required]
        [StringLength(255)]
        public string url { get; set; }
    }
}