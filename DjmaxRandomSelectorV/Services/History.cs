using System.Collections.Generic;

namespace DjmaxRandomSelectorV.Services
{
    public class History : Queue<int>, IHistory, IEnumerable<int>, IReadOnlyCollection<int>
    {
        private int _limit = 5;
        public int Limit
        {
            get { return _limit; }
            set
            {
                _limit = value;
                while (Count > _limit)
                {
                    Dequeue();
                }
            }
        }

        public History(IEnumerable<int> recents) : base(recents) { }

        public new void Enqueue(int item)
        {
            base.Enqueue(item);
            while (Count > _limit)
            {
                Dequeue();
            }
        }
    }
}
