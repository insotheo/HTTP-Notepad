using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Data.SQLite;
using HTTPNotepad.Tools;

namespace HTTPNotepad.Net
{
    static class RequestHandler
    {
        private static string dbString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), ".server", "my_notes.db")}";

        public static (HttpStatusCode code, string message) HandleRegistration(ref HttpListenerRequest rq)
        {
            string body = parseBody(rq.InputStream, rq.ContentEncoding);
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(body);

            string name = data["name"];
            string username = data["username"];
            string password = PasswordsTool.Encrypt(data["password"]);

            using(SQLiteConnection connection = new SQLiteConnection(dbString))
            {
                connection.Open();

                using (SQLiteCommand check = new SQLiteCommand("SELECT COUNT(*) FROM USERS Where Username = @username", connection))
                {
                    check.Parameters.AddWithValue("@username", username);
                    long userCount = (long)check.ExecuteScalar();

                    if(userCount > 0)
                    {
                        return (HttpStatusCode.Conflict, $"User with username \"{username}\" already exists!");
                    }
                }

                using(SQLiteCommand insert = new SQLiteCommand("INSERT INTO USERS (Username, Name, Password) VALUES (@username, @name, @pswrd)", connection))
                {
                    insert.Parameters.AddWithValue("@username", username);
                    insert.Parameters.AddWithValue("@name", name);
                    insert.Parameters.AddWithValue("@pswrd", password);
                    insert.ExecuteNonQuery();
                }

            }

            return (HttpStatusCode.Created, "Success");
        }

        private static string parseBody(Stream input, Encoding encoding)
        {
            string body;
            using(StreamReader reader = new StreamReader(input, encoding))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }
    }
}
