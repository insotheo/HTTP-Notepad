using HTTPNotepad.Net;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HTTPNotepad
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                using (Server server = new Server("http://localhost:5050/", 1))
                {
                    server.Start();
                    server.PushPage("index", new Page(Path.Combine(Directory.GetCurrentDirectory(), ".content", "index.html")));
                    await server.Work();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }
    }
}
