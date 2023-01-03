using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmrsv.RandomSelector
{
    // 플레이스토어마냥
    public interface IHub
    {
        bool TryRegister(Type type, object obj);
        object? Request(Type type);
        public IEnumerable<object?> RequestAll();
    }

    public class Hub : IHub
    {
        private readonly Dictionary<Type, object?> _products = new();

        public bool HasUpdate { get; private set; }

        public bool TryRegister(Type type, object obj)
        {
            return _products.TryAdd(type, obj);
        }

        public void NotifyUpdate()
        {
            HasUpdate = true;
        }

        public object? Request(Type type)
        {
            var obj = _products.GetValueOrDefault(type, null);
            return obj;
        }

        public T? Request<T>()
        {
            return (T?)Request(typeof(T));
        }

        public IEnumerable<object?> RequestAll()
        {
            return _products.Values.Select(x => x);
        }
    }
}
