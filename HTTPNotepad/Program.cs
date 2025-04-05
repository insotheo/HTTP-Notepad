using HTTPNotepad.Net;
using HTTPNotepad.Tools;
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
                NotesController.Init();
                using (Server server = new Server("http://127.0.0.1:5050/", "welcome"))
                {
                    server.PushPage("welcome", new Page(Path.Combine(Directory.GetCurrentDirectory(), ".content", "welcome.html")));
                    server.PushPage("home", new Page(Path.Combine(Directory.GetCurrentDirectory(), ".content", "home.html")));

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
