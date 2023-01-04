using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmrsv.RandomSelector
{
    public interface IHub
    {
        void Register(object sender, Type type, object obj, string? key);
        object? Request(Type type, string? key);
        public IEnumerable<object?> RequestAll();
    }

    public class Hub : IHub
    {
        private readonly List<Product> _products = new();

        public void Register(object sender, Type type, object obj, string? key = null)
        {
            var id = new Identifier(type, key);
            if (_products.Any(x => x.Id == id && x.Sender != sender))
                throw new ArgumentException("The identifier already exists.");

            var product = new Product { Id = id, Sender = sender, Content = obj };
            int index = _products.FindIndex(x => x.Id == id);
            if (index == -1)
                _products.Add(product);
            else
                _products[index] = product;
        }

        public object? Request(Type type, string? key = null)
        {
            var obj = _products.FirstOrDefault(x => x!.Id.Type == type && x.Id.Key == key, null);
            return obj;
        }

        public T? Request<T>(string? key = null)
        {
            return (T?)Request(typeof(T), key);
        }

        public IEnumerable<object?> RequestAll()
        {
            return _products.Select(x => x.Content);
        }

        private record Identifier(Type Type, string? Key);

        private class Product
        {
            public Identifier Id { get; init; } = default!;
            public object Sender { get; init; } = default!;
            public object Content { get; init; } = default!;
            public bool HasUpdate { get; set; } = true;
        }
    }
}
