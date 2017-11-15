using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Security.Claims;
using System.Web.Script.Serialization;
using rest_api.ModelViews;
using rest_api.Context;
using rest_api.Models;
using Newtonsoft.Json;

namespace rest_api.Controllers
{
    public class Item
    {
        public string Html { get; set; }
    }

    [RoutePrefix("message")]
    public class MessagesController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
        [HttpPost]
        [Authorize]
        [Route("")]
        public IHttpActionResult create([FromBody] _Message _message)
        {
            #region User Validation
            var identity = User.Identity as ClaimsIdentity;
            int user_id = Int32.Parse(identity.FindFirst("user_id").Value);

            // get User
            Users user = db.users.Find(user_id);
            if (user == null) return NotFound();

            //Check user exist
            if (!db.users.Any(u => u.id == _message.user_id)) return NotFound();
            #endregion

            List<_MessageDetail> msgList = new List<_MessageDetail>();
            _MessageDetail msgDetail = new _MessageDetail()
            {
                date = DateTime.Now,
                fullname = user.name + " " + user.lastname,
                message = _message.message
            };
            msgList.Add(msgDetail);

            //Create Message
            Messages message = new Messages()
            {
                last_message = new JavaScriptSerializer().Serialize(msgDetail),
                messages =  new JavaScriptSerializer().Serialize(msgList),
                last_message_on = DateTime.Now
            };
            db.messages.Add(message);
            db.SaveChanges();

            //create User Messages
            UserMessages userMessage_Sender = new UserMessages()
            {
                is_owner = true,
                user_id = user_id,
                message_id = message.id,
                last_view = DateTime.Now
            };
            UserMessages userMessage_Recipient = new UserMessages()
            {
                user_id = _message.user_id,
                message_id = message.id
            };
            db.user_messages.Add(userMessage_Sender);
            db.user_messages.Add(userMessage_Recipient);
            db.SaveChanges();

            return Ok(message);
        }


        [HttpGet]
        [Authorize]
        [Route("")]
        public object get()
        {
            var identity = User.Identity as ClaimsIdentity;
            int user_id = Int32.Parse(identity.FindFirst("user_id").Value);
            
            return (
                from m in db.messages
                join um in db.user_messages on m.id equals um.message_id
                where um.user_id == user_id && um.is_deleted == false
                select new _Messages()
                {
                    message = m.last_message,
                    last_view = um.last_view,
                    user = (um.user_id != user_id ? (from u in db.users where u.id == user_id join uimg in db.images on u.image_id equals uimg.id into j1 from j2 in j1.DefaultIfEmpty() select new _MessageUser() { fullname = u.name + " " + u.lastname, photo = j2.url }).FirstOrDefault() : (from mm in db.user_messages where mm.id == m.id join u in db.users on mm.user_id equals u.id join uimg in db.images on u.image_id equals uimg.id into j1 from j2 in j1.DefaultIfEmpty() select new _MessageUser() { fullname = u.name + " " + u.lastname, photo = j2.url }).FirstOrDefault())
                }
                ).ToList();
        }
    }
}
