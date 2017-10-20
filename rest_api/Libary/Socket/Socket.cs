using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace rest_api.Libary.Socket
{
    public static class Socket
    {
        
        public async static Task SendString(String data)
        {
            var ws = new ClientWebSocket();
            ws.Options.SetRequestHeader("ContentType", "application/json; charset=utf-8");
            string wsUri = string.Format("https://ws.menkule.com.tr/emit");
          
            var cancellation = new CancellationToken();
            await ws.ConnectAsync(new Uri(wsUri), cancellation);

            var encoded = Encoding.UTF8.GetBytes(data);
            var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);

            await ws.SendAsync(buffer, WebSocketMessageType.Text, true, cancellation);
        }
        public static async Task<String> ReadString(ClientWebSocket ws)
        {
            ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);

            WebSocketReceiveResult result = null;

            using (var ms = new MemoryStream())
            {
                do
                {
                    result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(ms, Encoding.UTF8))
                    return reader.ReadToEnd();
            }
        }

        public async static void emit()
        {
            var key = "qQrsBccsx0KHjPOuXOYg5sMeJ1GT0uFiwDVvVBrs";
            var cts2 = new CancellationToken();
            /*
               socket.Options.SetRequestHeader("X-Secret-Key", key);
            socket.Options.SetRequestHeader("ContentType", "application/json; charset=utf-8");
            string wsUri = string.Format("https://ws.menkule.com.tr/emit");
            await socket.ConnectAsync(new Uri(wsUri), cts2);
             */




        }

    }
}