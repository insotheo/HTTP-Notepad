using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Text.Json;

namespace HTTPNotepad.Tools
{
    class NotesController
    {
        public static readonly string @Path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), ".server", "NOTES");

        public static void Init()
        {
            if (!Directory.Exists(@Path))
            {
                Directory.CreateDirectory(@Path);
            }

            List<string> paths = new List<string>();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={System.IO.Path.Combine(Directory.GetCurrentDirectory(), ".server", "my_notes.db")}"))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand("SELECT NotesFileName FROM USERS", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            paths.Add(System.IO.Path.Combine(@Path, reader["NotesFileName"].ToString() + ".json"));
                        }
                    }
                }
            }

            foreach (string p in paths)
            {
                CreateNote(p);
            }

        }

        public static void CreateNote(string p)
        {
            if (!File.Exists(p))
            {
                List<Note> notes = new List<Note>();
                string data = JsonSerializer.Serialize(notes);
                using (FileStream note = File.Create(p))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    note.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public static List<Note> GetNotes(string p)
        {
            if (!File.Exists(p))
            {
                return new List<Note>();
            }
            using (FileStream note = File.OpenRead(p))
            {
                using (StreamReader reader = new StreamReader(note))
                {
                    string content = reader.ReadToEnd();
                    return JsonSerializer.Deserialize<List<Note>>(content);
                }
            }
        }

        public static Note GetNote(long uuid, string p)
        {
            List<Note> notes = GetNotes(p);
            foreach (Note note in notes)
            {
                if (note.UUID == uuid)
                {
                    return note;
                }
            }
            return null;
        }

    }
}
