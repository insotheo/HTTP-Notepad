using HTTPNotepad.Net;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HTTPNotepad
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using(Server server = new Server("http://localhost:5050/"))
            {
                server.Start();
                while (true)
                {
                    HttpListenerContext ctx = await server.GetReference().GetContextAsync();
                    HttpListenerRequest request = ctx.Request;
                    HttpListenerResponse response = ctx.Response;

                    string respString = "<html><body><h1>Hello, World!</h1></body></html>";
                    byte[] buffer = Encoding.UTF8.GetBytes(respString);
                    response.ContentLength64 = buffer.Length;
                    using(var output = response.OutputStream)
                    {
                        await output.WriteAsync(buffer, 0, buffer.Length);
                    }
                }
            }
        }
    }
}
