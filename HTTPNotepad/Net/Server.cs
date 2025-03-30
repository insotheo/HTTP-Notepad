using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HTTPNotepad.Net
{
    class Server : IDisposable
    {
        HttpListener listener;
        Dictionary<String, Page> pages;

        public Server(string prefix)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(prefix);

            pages = new Dictionary<string, Page>();
        }

        public async Task Work()
        {
            if (!listener.IsListening)
                throw new Exception("Listner not started!");

            while (listener.IsListening)
            {
                HttpListenerContext ctx = listener.GetContext();
                HttpListenerRequest request = ctx.Request;
                HttpListenerResponse response = ctx.Response;
                string urlPath = request.Url.AbsolutePath.TrimEnd('/');

                using (Stream responseOut = response.OutputStream)
                {
                    if (pages.ContainsKey(urlPath)) //loading a page
                    {
                        response.ContentType = "text/html";
                        await responseOut.WriteAsync(pages[urlPath].GetBuffer(), 0, pages[urlPath].GetLength());
                        
                    }

                    else if(File.Exists(Path.Combine(Directory.GetCurrentDirectory(), ".content", urlPath.TrimStart('/')))) //loading resources
                    {
                        string path = Path.Combine(Directory.GetCurrentDirectory(), ".content", urlPath.TrimStart('/'));
                        string extension = Path.GetExtension(urlPath.TrimStart('/'));
                        response.ContentType = extension switch
                        {
                            ".css" => "text/css",
                            ".js" => "application/javascript",
                            _ => "application/octet-stream"
                        };

                        byte[] fileBuffer = await File.ReadAllBytesAsync(path);
                        await responseOut.WriteAsync(fileBuffer, 0, fileBuffer.Length);
                    }

                    else //404
                    {
                        response.StatusCode = 404;
                        response.ContentType = "text/html";
                        byte[] buffer = Encoding.UTF8.GetBytes($"<!DOCTYPE html><html><head><title>PAGE NOT FOUND</title></head><body><h1>Error: 404</h1><p>Page \"{request.Url.ToString()}\" not found!</p></body></html>");
                        await responseOut.WriteAsync(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        public void PushPage(string prefix, Page page)
        {
            if (!prefix.StartsWith("/"))
            {
                prefix = "/" + prefix;
            }
            pages.Add(prefix, page);
        }

        public void Start() => listener.Start();

        //public ref HttpListener GetReference() => ref listener;

        public void Dispose()
        {
            listener.Stop();
            listener.Close();
        }
    }
}
