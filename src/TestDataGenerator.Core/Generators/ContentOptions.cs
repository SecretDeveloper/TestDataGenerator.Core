namespace TestDataGenerator.Core.Generators
{
    internal class ContentOptions
    {
        public bool IsNegated { get; set; }
        public bool ContainsAlternation { get; set; }
        public string Content { get; set; }
        public int Repeat { get; set; }
        public bool IsAnagram { get; set; }
        public bool IsSpecialFunction { get { return IsAnagram; } }
        public string QuantifierContent { get; set; }

        public ContentOptions()
        {
            IsNegated = false;
            IsAnagram = false;
            Content = "";
            ContainsAlternation = false;
            Repeat = 1;
        }
    }
}