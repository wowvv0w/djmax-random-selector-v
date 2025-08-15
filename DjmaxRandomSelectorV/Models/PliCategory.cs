namespace DjmaxRandomSelectorV.Models
{
    public class PliCategory
    {
        public string Major { get; set; }
        public PliCategoryMinor[] Minors { get; set; }
    }

    public class PliCategoryMinor
    {
        public string Name { get; set; }
        public int[][] Items { get; set; }
    }
}
