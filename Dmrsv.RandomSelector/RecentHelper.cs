namespace Dmrsv.RandomSelector
{
    public class RecentHelper<T> : IRecent<T>
    {
        private readonly Queue<T> _recent;
        private int _capacity;

        public int Capacity
        {
            get => _capacity;
            set
            {
                _capacity = value;
                ResolveOverflow();
            }
        }
        public int Count => _recent.Count;

        public RecentHelper(IEnumerable<T>? recent = null, int capacity = 5)
        {
            _recent = new Queue<T>(recent ?? Array.Empty<T>());
            Capacity = capacity;
            ResolveOverflow();
        }

        public void Enqueue(T item)
        {
            _recent.Enqueue(item);
            if (_recent.Count > Capacity)
            {
                _recent.Dequeue();
            }
        }

        public T Dequeue()
        {
            return _recent.Dequeue();
        }

        public bool Contains(T item)
        {
            return _recent.Contains(item);
        }

        public void Clear()
        {
            _recent.Clear();
        }

        public IEnumerable<T> GetItems()
        {
            return _recent.Select(x => x);
        }

        private void ResolveOverflow()
        {
            int over = _recent.Count - _capacity;
            for (int i = 0; i < over; i++)
            {
                _recent.Dequeue();
            }
        }
    }
}
