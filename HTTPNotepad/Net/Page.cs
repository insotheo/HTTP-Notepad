using System.IO;
using System.Text;

namespace HTTPNotepad.Net
{
    class Page
    {
        byte[] buffer;

        public Page(string path)
        {
            buffer = Encoding.UTF8.GetBytes(File.ReadAllText(path));
        }

        public ref byte[] GetBuffer() => ref buffer;
        public int GetLength() => buffer.Length;
    }
}
