namespace Dmrsv.RandomSelector
{
    public interface IRecent<T>
    {
        int Capacity { get; set; }
        int Count { get; }
        void Enqueue(T item);
        T Dequeue();
        bool Contains(T item);
        void Clear();
        IEnumerable<T> GetItems();
    }
}
