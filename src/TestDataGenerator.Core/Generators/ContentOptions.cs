namespace TestDataGenerator.Core.Generators
{
    internal class ContentOptions
    {
        public bool IsNegated { get; set; }
        public bool ContainsAlternation { get; set; }
        public string Content { get; set; }
        public int Repeat { get; set; }
        public bool IsAnagram { get; set; }
        public bool IsShuffle { get; set; }
        public bool IsShuffle2 { get; set; }
        public string QuantifierContent { get; set; }

        public ContentOptions()
        {
            IsNegated = false;
            IsAnagram = false;
            IsShuffle = false;
            IsShuffle2 = false;
            Content = "";
            ContainsAlternation = false;
            Repeat = 1;
        }
    }
}