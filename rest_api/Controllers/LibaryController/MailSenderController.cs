using System.Net;
using System.Web.Http;
using rest_api.Libary.MailSender;
using rest_api.Libary.Responser;
using rest_api.ModelViews;
namespace rest_api.Controllers.LibaryController
{
    [RoutePrefix("contact")]
    public class MailSenderController : ApiController
    {
        [HttpPost]
        [Route("")]
        public IHttpActionResult send([FromBody]MessageMV message)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!MailSender.Send(message)) Responser.Response(HttpStatusCode.NotImplemented, "Mesajınız iletilemedi. Lütfen tekrar deneyin.");
            return Ok();
        }
    }
}
