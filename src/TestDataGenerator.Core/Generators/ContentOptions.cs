namespace TestDataGenerator.Core.Generators
{
    internal class ContentOptions
    {
        public bool IsNegated { get; set; }
        public bool ContainsAlternation { get; set; }
        public string Content { get; set; }
        public int Repeat { get; set; }

        public ContentOptions()
        {
            IsNegated = false;
            Content = "";
            ContainsAlternation = false;
            Repeat = 1;
        }
    }
}