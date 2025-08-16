namespace DjmaxRandomSelectorV.SerializableObjects
{
    public class Dmrsv3AppData
    {
        public string[] CategoryType { get; set; }
        public string[] BasicCategories { get; set; }
        public Dmrsv3Category[] Categories { get; set; }
        public Dmrsv3LinkDisc[] LinkDisc { get; set; }
        // TODO: title converter for multi-language support
    }
}
