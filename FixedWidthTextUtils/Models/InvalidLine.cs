namespace Models.FixedWidthTextUtils
{
    public class InvalidLine
    {
        public long Number { get; set; }
        public string Line { get; set; }
        public string ErrorDescription { get; set; }

        public InvalidLine(long number, string line, string errorDescription)
        {
            Number = number;
            Line = line;
            ErrorDescription = errorDescription;
        }

    }
}
