using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HTTPNotepad.Net
{
    class Server : IDisposable
    {
        HttpListener listener;
        Dictionary<String, Page> pages;
        string defaultPageName;

        public Server(string prefix, string defaultPageName = null)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(prefix);

            pages = new Dictionary<string, Page>();
            this.defaultPageName = defaultPageName;
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

                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS, DELETE");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                //Console.WriteLine($"Request from: {urlPath}");

                using (Stream responseOut = response.OutputStream)
                {
                    //Handling requests
                    if (request.HttpMethod == "POST" && urlPath == "/register")
                    {
                        (HttpStatusCode code, string message) data = RequestHandler.HandleRegistration(ref request);
                        byte[] buffer = Encoding.UTF8.GetBytes(data.message);
                        response.StatusCode = (int)data.code;
                        response.ContentLength64 = buffer.Length;
                        await responseOut.WriteAsync(buffer, 0, buffer.Length);
                    }
                    else if (request.HttpMethod == "GET" && urlPath == "/login")
                    {
                        (HttpStatusCode code, string message) data = RequestHandler.HandleLogin(ref request);
                        if (data.code != HttpStatusCode.OK)
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes(data.message);
                            response.StatusCode = (int)data.code;
                            response.ContentLength64 = buffer.Length;
                            await responseOut.WriteAsync(buffer, 0, buffer.Length);
                        }
                        else
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes(data.message);
                            response.StatusCode = (int)data.code;
                            response.ContentLength64 = buffer.Length;
                            response.ContentType = "application/json";
                            await responseOut.WriteAsync(buffer, 0, buffer.Length);
                        }
                    }
                    else if (request.HttpMethod == "GET" && urlPath == "/get_data")
                    {
                        (HttpStatusCode code, string message) data = RequestHandler.HandleGettingData(ref request);
                        if (data.code != HttpStatusCode.OK)
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes(data.message);
                            response.StatusCode = (int)data.code;
                            response.ContentLength64 = buffer.Length;
                            await responseOut.WriteAsync(buffer, 0, buffer.Length);
                        }
                        else
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes(data.message);
                            response.StatusCode = (int)data.code;
                            response.ContentLength64 = buffer.Length;
                            response.ContentType = "application/json";
                            await responseOut.WriteAsync(buffer, 0, buffer.Length);
                        }
                    }
                    else if (request.HttpMethod == "DELETE" && urlPath == "/delete_account")
                    {
                        (HttpStatusCode code, string message) data = RequestHandler.HandleDeletingAccount(ref request);
                        if (data.code != HttpStatusCode.OK)
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes(data.message);
                            response.StatusCode = (int)data.code;
                            response.ContentLength64 = buffer.Length;
                            await responseOut.WriteAsync(buffer, 0, buffer.Length);
                        }
                    }

                    else if (pages.ContainsKey(urlPath)) //loading a page
                    {
                        response.ContentType = "text/html";
                        response.ContentLength64 = pages[urlPath].GetLength();
                        await responseOut.WriteAsync(pages[urlPath].GetBuffer(), 0, pages[urlPath].GetLength());
                    }
                    else if (String.IsNullOrEmpty(urlPath) && defaultPageName != null)
                    {
                        response.StatusCode = (int)HttpStatusCode.Found;
                        response.RedirectLocation = listener.Prefixes.ElementAt(0).TrimEnd('/') + "/" + defaultPageName;

                        string responseString = "Redirecting...";
                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                        response.ContentLength64 = buffer.Length;
                        await responseOut.WriteAsync(buffer, 0, buffer.Length);
                    }

                    else if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), ".content", urlPath.TrimStart('/')))) //loading resources
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
                        response.ContentLength64 = fileBuffer.Length;
                        await responseOut.WriteAsync(fileBuffer, 0, fileBuffer.Length);
                    }

                    else //404
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.ContentType = "text/html";
                        byte[] buffer = Encoding.UTF8.GetBytes($"<!DOCTYPE html><html><head><title>PAGE NOT FOUND</title></head><body><h1>Error: 404</h1><p>Page \"{request.Url.ToString()}\" not found!</p></body></html>");
                        response.ContentLength64 = buffer.Length;
                        await responseOut.WriteAsync(buffer, 0, buffer.Length);
                    }
                }

                response.Close();
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
