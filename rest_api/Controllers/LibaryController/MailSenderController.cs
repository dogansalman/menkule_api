using System.Web.Http;
using rest_api.Libary.MailSender;
using rest_api.ModelViews;
using rest_api.Libary.Exceptions.ExceptionThrow;

namespace rest_api.Controllers.LibaryController
{
    [RoutePrefix("contact")]
    public class MailSenderController : ApiController
    {
        [HttpPost]
        [Route("")]
        public IHttpActionResult send([FromBody]_Contact message)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!MailSender.Send(message)) ExceptionThrow.Throw("Mesajınız iletilemedi. Lütfen tekrar deneyin.");
            return Ok();
        }
    }
}
