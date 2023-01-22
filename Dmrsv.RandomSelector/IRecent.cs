namespace Dmrsv.RandomSelector
{
    public interface IRecent<T>
    {
        int Capacity { get; set; }
        void Add(T item);
        bool Contains(T item);
        void Clear();
        IEnumerable<T> GetItems();
    }
}
