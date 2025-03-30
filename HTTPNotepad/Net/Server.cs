
using System;
using System.Net;

namespace HTTPNotepad.Net
{
    class Server : IDisposable
    {
        HttpListener listener;

        public Server(string prefix)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(prefix);
        }

        public void Start() => listener.Start();

        public ref HttpListener GetReference() => ref listener;

        public void Dispose()
        {
            listener.Stop();
            listener.Close();
        }
    }
}
