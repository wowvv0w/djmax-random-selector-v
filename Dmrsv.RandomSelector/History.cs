namespace Dmrsv.RandomSelector
{
    public class History<T> : IHistory<T>
    {
        private readonly Queue<T> _history;
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
        public int Count => _history.Count;

        public History(IEnumerable<T>? recent = null, int capacity = 5)
        {
            _history = new Queue<T>(recent ?? Array.Empty<T>());
            Capacity = capacity;
            ResolveOverflow();
        }

        public void Enqueue(T item)
        {
            _history.Enqueue(item);
            if (_history.Count > Capacity)
            {
                _history.Dequeue();
            }
        }

        public T Dequeue()
        {
            return _history.Dequeue();
        }

        public bool Contains(T item)
        {
            return _history.Contains(item);
        }

        public void Clear()
        {
            _history.Clear();
        }

        public IEnumerable<T> GetItems()
        {
            return _history.Select(x => x);
        }

        private void ResolveOverflow()
        {
            int over = _history.Count - _capacity;
            for (int i = 0; i < over; i++)
            {
                _history.Dequeue();
            }
        }
    }
}
