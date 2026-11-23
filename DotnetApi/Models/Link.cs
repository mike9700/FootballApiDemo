public class Link
    {
        public string Href { get; set; }
        public bool IsExternal { get; set; }
        public bool IsHidden { get; set; }
        public bool IsPremium { get; set; }
        public string Language { get; set; }
        public List<string> Rel { get; set; }
        public string ShortText { get; set; }
        public string Text { get; set; }
    }