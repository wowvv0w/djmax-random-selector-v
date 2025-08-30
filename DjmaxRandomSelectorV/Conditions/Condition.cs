using System;
using System.Linq;

namespace DjmaxRandomSelectorV.Conditions
{
    public static class Condition
    {
        public static NullCondition Null => NullCondition.Instance;

        public static void ThrowIfNotCondition(Type type)
        {
            if (!type.IsAssignableTo(typeof(ICondition)))
            {
                throw new ArgumentException(
                    $"Type must be an implementation of interface {nameof(ICondition)}",
                    nameof(type));
            }
        }

        public static void ThrowIfNotCondition(object obj)
        {
            ThrowIfNotCondition(obj.GetType());
        }
        
        public static ICondition ComplementOf(ICondition condition)
        {
            if (condition is ComplementCondition complement)
            {
                return complement.Condition;
            }
            return new ComplementCondition(condition);
        }

        public static ICondition CreateInstance(Type type, params object[] args)
        {
            ThrowIfNotCondition(type);
            return (ICondition)Activator.CreateInstance(type, args);
        }

        public static ICondition CreateInstance(ConditionInfo entry)
        {
            return (ICondition)Activator.CreateInstance(entry.Type, entry.Args);
        }

        public static UnionCondition CreateUnion(params (bool IsEnabled, Func<ICondition> Generate)[] queries)
        {
            var result = queries.Where(query => query.IsEnabled)
                                .Select(query => Compress(query.Generate()))
                                .Where(cond => cond is not null);
            return result.Any() ? new UnionCondition(result) : null;
        }

        public static IntersectionCondition CreateIntersection(params (bool IsEnabled, Func<ICondition> Generate)[] queries)
        {
            var result = queries.Where(query => query.IsEnabled)
                                .Select(query => Compress(query.Generate()))
                                .Where(cond => cond is not null);
            return result.Any() ? new IntersectionCondition(result) : null;
        }

        private static ICondition Compress(ICondition condition)
        {
            if (condition is IMergedCondition merged && merged.Conditions.Take(2).Count() == 1)
            {
                return merged.Conditions.Single();
            }
            return condition;
        }
    }
}
