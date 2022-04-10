namespace DjmaxRandomSelectorV.DataTypes
{
    public class InputCommand
    {
        public char Initial { get; set; }
        public int VerticalInputCount { get; set; }
        public char ButtonTune { get; set; }
        public int RightInputCount { get; set; }
        public bool IsForward { get; set; }
        public bool IsAlphabet { get; set; }
        public int Delay { get; set; }
        public bool Starts { get; set; }
    }
}
