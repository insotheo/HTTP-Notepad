using HTTPNotepad.Tools;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Text;

namespace HTTPNotepad.Net
{
    static class RequestHandler
    {
        private static string dbString = $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), ".server", "my_notes.db")}";

        public static (HttpStatusCode code, string message) HandlePostingNotes(ref HttpListenerRequest rq)
        {
            string body = parseBody(rq.InputStream, rq.ContentEncoding);
            List<Net.Note> notes = JsonConvert.DeserializeObject<List<Net.Note>>(body);
            string username = rq.QueryString["username"];
            string filePath = "";

            using (SQLiteConnection connection = new SQLiteConnection(dbString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("SELECT NotesFileName FROM USERS WHERE Username = @username", connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            filePath = Path.Combine(NotesController.Path, reader["NotesFileName"].ToString() + ".json");
                        }
                        else
                        {
                            return (HttpStatusCode.NotFound, "User not found.");
                        }
                    }
                }
            }

            List<Tools.Note> serverNotes = new List<Tools.Note>();
            foreach (Net.Note note in notes)
            {
                Tools.Note n = new Tools.Note(notes.IndexOf(note), note.title);
                n.SetNote(note.title, note.content);
                serverNotes.Add(n);
            }

            NotesController.WriteNotes(filePath, serverNotes);

            return (HttpStatusCode.OK, "");
        }

        public static (HttpStatusCode code, string message) HandleDeletingAccount(ref HttpListenerRequest rq)
        {
            string username = rq.QueryString["username"];
            string password = PasswordsTool.Encrypt(rq.QueryString["password"]);

            string filePath = "";
            using (SQLiteConnection connection = new SQLiteConnection(dbString))
            {
                connection.Open();

                using (SQLiteCommand check = new SQLiteCommand("SELECT Password, NotesFileName FROM USERS WHERE Username = @username", connection))
                {
                    check.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = check.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return (HttpStatusCode.Conflict, $"User with username \"{username}\" not found!");
                        }
                        if (password != reader["Password"].ToString())
                        {
                            return (HttpStatusCode.Conflict, "Incorrect password!");
                        }
                        filePath = Path.Combine(NotesController.Path, reader["NotesFileName"].ToString() + ".json");
                    }
                }

                using (SQLiteCommand delete = new SQLiteCommand("DELETE FROM USERS WHERE Username = @username", connection))
                {
                    delete.Parameters.AddWithValue("@username", username);
                    delete.ExecuteNonQuery();
                }
            }

            File.Delete(filePath);

            return (HttpStatusCode.OK, "");
        }

        public static (HttpStatusCode code, string message) HandleGettingData(ref HttpListenerRequest rq)
        {
            string username = rq.QueryString["username"];
            string name = rq.QueryString["name"];

            List<Net.Note> notes = new List<Net.Note>();
            using (SQLiteConnection connection = new SQLiteConnection(dbString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM USERS Where Username = @username AND Name = @name", connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@name", name);

                    long count = (long)command.ExecuteScalar();

                    if (count == 0)
                    {
                        return (HttpStatusCode.Conflict, "No user found!");
                    }
                }

                using (SQLiteCommand command = new SQLiteCommand("SELECT NotesFileName FROM USERS Where Username = @username", connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string pathToFile = Path.Combine(NotesController.Path, reader["NotesFileName"].ToString() + ".json");
                            List<Tools.Note> serverNotes = NotesController.GetNotes(pathToFile);
                            foreach(Tools.Note note in serverNotes)
                            {
                                notes.Add(new Net.Note() { title = note.Title, content = note.Content });
                            }
                        }
                    }
                }
            }

            return (HttpStatusCode.OK, JsonConvert.SerializeObject(notes));
        }

        public static (HttpStatusCode code, string message) HandleRegistration(ref HttpListenerRequest rq)
        {
            string body = parseBody(rq.InputStream, rq.ContentEncoding);
            var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(body);

            string name = data["name"];
            string username = data["username"];
            string password = PasswordsTool.Encrypt(data["password"]);
            long counter = 0;

            string filename = "";
            using (SQLiteConnection connection = new SQLiteConnection(dbString))
            {
                connection.Open();

                using (SQLiteCommand check = new SQLiteCommand("SELECT COUNT(*) FROM USERS Where Username = @username", connection))
                {
                    check.Parameters.AddWithValue("@username", username);
                    long userCount = (long)check.ExecuteScalar();

                    if (userCount > 0)
                    {
                        return (HttpStatusCode.Conflict, $"User with username \"{username}\" already exists!");
                    }
                }

                using (SQLiteCommand count = new SQLiteCommand("SELECT COUNT(*) FROM USERS", connection))
                {
                    counter = (long)count.ExecuteScalar();
                }

                using (SQLiteCommand insert = new SQLiteCommand("INSERT INTO USERS (Username, Name, Password, NotesFileName) VALUES (@username, @name, @pswrd, @filename)", connection))
                {
                    insert.Parameters.AddWithValue("@username", username);
                    insert.Parameters.AddWithValue("@name", name);
                    insert.Parameters.AddWithValue("@pswrd", password);
                    string username2 = username;
                    foreach(char c in Path.GetInvalidFileNameChars())
                    {
                        if (username2.Contains(c))
                        {
                            username2 = username2.Replace(c.ToString(), "");
                        }
                    }
                    filename = username2 + "_" + counter.ToString();
                    insert.Parameters.AddWithValue("@filename", filename);
                    insert.ExecuteNonQuery();
                }

            }

            NotesController.CreateNote(Path.Combine(NotesController.Path, filename + ".json"));

            return (HttpStatusCode.Created, "Success");
        }

        public static (HttpStatusCode code, string message) HandleLogin(ref HttpListenerRequest rq)
        {
            string username = rq.QueryString["username"];
            string inputPassword = PasswordsTool.Encrypt(rq.QueryString["password"]);

            string name = "";

            using (SQLiteConnection connection = new SQLiteConnection(dbString))
            {
                connection.Open();

                using (SQLiteCommand check = new SQLiteCommand("SELECT COUNT(*) FROM USERS Where Username = @username", connection))
                {
                    check.Parameters.AddWithValue("@username", username);
                    long userCount = (long)check.ExecuteScalar();

                    if (userCount == 0)
                    {
                        return (HttpStatusCode.Conflict, "Incorrect username or password!");
                    }
                }

                using (SQLiteCommand parse = new SQLiteCommand("SELECT Name, Username, Password FROM USERS WHERE Username = @username", connection))
                {
                    parse.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = parse.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string password = reader["Password"].ToString();
                            if (password != inputPassword)
                            {
                                return (HttpStatusCode.Conflict, "Incorrect username or password!");
                            }
                            password = null;
                            inputPassword = null;

                            name = reader["Name"].ToString();
                        }
                    }
                }
            }

            var dbg = new { name = name };

            return (HttpStatusCode.OK, System.Text.Json.JsonSerializer.Serialize(dbg));
        }

        private static string parseBody(Stream input, Encoding encoding)
        {
            string body;
            using (StreamReader reader = new StreamReader(input, encoding))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }
    }
}
