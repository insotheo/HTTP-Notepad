namespace HTTPNotepad.Tools
{
    class Note
    {
        public long UUID
        {
            get;
            private set;
        }

        public string Title { get; private set; }
        public string Content { get; private set; }

        public Note(long uuid, string title)
        {
            UUID = uuid;
            Title = title;
            Content = string.Empty;
        }

        public void SetNote(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
}
