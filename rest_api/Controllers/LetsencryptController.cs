using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Web;

namespace rest_api.Controllers
{
  

    [RoutePrefix(".well-known")]
    public class LetsencryptController : ApiController
    {
        [HttpGet]
        [Route("acme-challenge/{id}")]
        public object LetsEncrypt(string id)
        { 
            string path = HttpContext.Current.Server.MapPath("~/.well-known/acme-challenge/" + id);
            FileInfo file = new FileInfo(path);
            if (!file.Exists) return NotFound();

            using (StreamReader sr = new StreamReader(path))
            {
               
                var response = new HttpResponseMessage();
                response.Content = new StringContent(sr.ReadToEnd());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                return response;
            }
            return NotFound();
        }
    }
}
