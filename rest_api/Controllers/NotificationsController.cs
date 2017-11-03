﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using rest_api.Context;
using rest_api.Models;
using rest_api.Libary.Exceptions;
using System.Security.Claims;

namespace rest_api.Controllers
{
    [RoutePrefix("notifications")]
    public class NotificationsController : ApiController
    {
        DatabaseContext db = new DatabaseContext();
        // Gets
        [HttpGet]
        [Route("")]
        [Authorize]
        public List<Notifications> get()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);

            List<Notifications> notifiys =  db.notifications.Where(n => n.user_id == user_id).OrderByDescending(n => n.created_date).ToList();

            using (var dbcon = new DatabaseContext())
            {
                dbcon.notifications.Where(n => n.user_id == user_id).ToList().ForEach(n => { n.state = false; n.updated_date = DateTime.Now; });
                dbcon.SaveChanges();
            }
          
            return notifiys;
        }

        // Delete
        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public IHttpActionResult delete(int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            try
            {
                Notifications notify = db.notifications.Where(n => n.user_id == user_id && n.id == id).FirstOrDefault();
                if (notify == null) return NotFound();
                db.notifications.Remove(notify);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
            return Ok();
        }

        // New Last 10 Gets
        [HttpGet]
        [Route("last/{count}")]
        [Authorize]
        public List<Notifications> getLast(int count)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int user_id = int.Parse(claimsIdentity.FindFirst("user_id").Value);
            return db.notifications.Where(n => n.user_id == user_id && n.state == true).OrderByDescending(n => n.created_date).Take(count).ToList();
        }

    }
}
