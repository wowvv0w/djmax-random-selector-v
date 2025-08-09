using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Services
{
    public interface IHistory : IEnumerable<int>, IReadOnlyCollection<int>
    {
        int Limit { get; set; }
        void Enqueue(int item);
        int Dequeue();
        void Clear();
    }
}
