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
                using (Server server = new Server("http://localhost:5050/", "welcome"))
                {
                    server.PushPage("welcome", new Page(Path.Combine(Directory.GetCurrentDirectory(), ".content", "welcome.html")));

                    server.Start();
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
