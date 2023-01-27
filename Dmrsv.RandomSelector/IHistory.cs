namespace Dmrsv.RandomSelector
{
    public interface IHistory<T>
    {
        int Capacity { get; set; }
        int Count { get; }
        void Enqueue(T item);
        T Dequeue();
        bool Contains(T item);
        void Clear();
        List<T> GetItems();
    }
}
