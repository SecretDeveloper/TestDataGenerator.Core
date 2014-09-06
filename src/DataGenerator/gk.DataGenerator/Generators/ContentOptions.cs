namespace gk.DataGenerator.Generators
{
    internal class ContentOptions
    {
        public bool IsNegated { get; set; }
        public bool ContainsAlternation { get; set; }
        public string Content { get; set; }
        public int Repeat { get; set; }

        public ContentOptions()
        {
            this.IsNegated = false;
            this.Content = "";
            this.ContainsAlternation = false;
            this.Repeat = 1;
        }
    }
}