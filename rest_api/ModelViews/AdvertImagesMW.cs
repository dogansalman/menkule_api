namespace rest_api.ModelViews
{
    public class AdvertImagesMW
    {
        public int image_id { get; set; }
        public string url { get; set; }
        public bool is_new {get; set;}
        public bool is_default { get; set; }
        public bool deleted { get; set; }

    }
}