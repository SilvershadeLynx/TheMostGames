namespace Words_counter
{
    //class to store pre ouptut data
    public class ResultTable
    {
        public ResultTable(string Text, int WordsCount, int VowelsCount)
        {
            this.Text = Text;
            this.WordsCount = WordsCount;
            this.VowelsCount = VowelsCount;
        }
        public string Text { get; set; }
        public int WordsCount { get; set; }
        public int VowelsCount { get; set; }
    }
}
