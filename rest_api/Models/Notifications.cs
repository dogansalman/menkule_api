using System;
using System.ComponentModel.DataAnnotations;
using rest_api.Context;
using rest_api.Libary.Socket;

namespace rest_api.Models
{
    public class Notifications
    {
        public  void add(int user_id, string title, int rezervation = 0)
        {
            using (var db = new DatabaseContext())
            {
                var notify = new Notifications
                {
                    user_id = user_id,
                    title = title,
                    created_date = DateTime.Now,
                    rezervation_id = rezervation == 0  ? 0 : rezervation,
                    state = true
                };
                db.notifications.Add(notify);
                Socket.Emit(user_id, "notification", notify);
                db.SaveChanges();
            }
        }

        [Key]
        public int id { get; set; }
        public int user_id { get; set; }
        [StringLength(255)]
        public string title { get; set; }
        public bool state { get; set; } = true;
        public int? rezervation_id { get; set; } = 0;
        public DateTime created_date { get; set; } = DateTime.Now;
        public DateTime? updated_date { get; set; }
      

    }

    
}