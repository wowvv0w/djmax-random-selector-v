using System;
using System.Linq;

namespace DjmaxRandomSelectorV.Conditions
{
    public static class Condition
    {
        public static NullCondition Null => NullCondition.Instance;

        public static ICondition ComplementOf(ICondition condition)
        {
            if (condition is ComplementCondition complement)
            {
                return complement.Condition;
            }
            return new ComplementCondition(condition);
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
