namespace DjmaxRandomSelectorV.SerializableObjects.VArchiveCompatible
{
    public class VArchiveBoardRoot
    {
        public bool? Success { get; set; }
        public string Board { get; set; }
        public string Button { get; set; }
        public VArchiveBoardFloor[] Floors { get; set; }
        public string Message { get; set; }
    }
}
