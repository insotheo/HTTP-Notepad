using System;
using System.Text;

namespace HTTPNotepad.Tools
{
    static class PasswordsTool
    {
        public static string Encrypt(string input)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(input);
            Array.Reverse(buffer);
            return Convert.ToBase64String(buffer);
        }

        public static string Decrypt(string input)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(input);
            Array.Reverse(buffer);
            return Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.UTF8.GetString(buffer)));
        }

    }
}
