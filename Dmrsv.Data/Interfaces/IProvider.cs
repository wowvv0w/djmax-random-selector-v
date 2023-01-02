namespace Dmrsv.Data
{
    public interface IProvider
    {
        void Provide(Music selectedMusic, List<Track> tracks, int delay);
    }
}
