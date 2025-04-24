namespace Dmrsv.RandomSelector
{
    public class TitleComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }
            if (y == null)
            {
                return -1;
            }
            if (x == null)
            {
                return 1;
            }
            // Djmax sorts titles with case-insensitive and ignoring the characters below
            x = x.Replace("'", string.Empty).Replace("-", string.Empty).ToUpper();
            y = y.Replace("'", string.Empty).Replace("-", string.Empty).ToUpper();
            int index = x.Zip(y, (a, b) => a == b).TakeWhile(equals => equals).Count();
            if (index == Math.Min(x.Length, y.Length))
            {
                return x.Length - y.Length;
            }
            // priority order: white-space -> non-alphabetic letter -> special character -> number -> alphabet
            char a = x[index], b = y[index];
            int priorityA = GetPriority(a);
            int priorityB = GetPriority(b);
            if (priorityA == priorityB)
            {
                return a.CompareTo(b);
            }
            return priorityA - priorityB;
        }

        private int GetPriority(char ch)
        {
            if (char.IsWhiteSpace(ch))
            {
                return 0;
            }
            if (char.IsUpper(ch)) // alphabet
            {
                return 4;
            }
            if (char.IsLetter(ch)) // non-alphabetic letter
            {
                return 1;
            }
            if (char.IsDigit(ch))
            {
                return 3;
            }
            // symbol, punctuation, etc.
            return 2;
        }
    }
}
