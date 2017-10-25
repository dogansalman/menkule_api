using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;

namespace rest_api.Libary.Socket
{
    public static class Socket
    {
    
        public static void Emit(int user_id, string subject, object data)
        {
            var socket = IO.Socket("https://ws.menkule.com.tr");
            JObject jout = JObject.FromObject(new { subject = subject, user_id = user_id, message = data });
            socket.Emit("emit", jout);
        }
    }
}