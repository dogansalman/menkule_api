using System.Net.Http;
using System.Net.Http.Headers;

namespace rest_api.Libary.MultipartStreamProvider
{
    public class MultipartStreamProvider : MultipartFormDataStreamProvider
    {
        public MultipartStreamProvider(string path) : base(path) { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
        }
    }
}