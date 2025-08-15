using DjmaxRandomSelectorV.Models;

namespace DjmaxRandomSelectorV
{
    public class Dmrsv3AppData
    {
        public string[] CategoryType { get; set; }
        public string[] BasicCategories { get; set; }
        public Category[] Categories { get; set; }
        public PliCategory[] PliCategories { get; set; }
        public LinkDiscItem[] LinkDisc { get; set; }
        // TODO: title converter for multi-language support
    }
}
