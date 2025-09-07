using System;
using System.Linq;

namespace DjmaxRandomSelectorV.Conditions
{
    public static class Condition
    {
        public static NullCondition Null => NullCondition.Instance;
        
        /// <summary>
        /// Returns a negation of the specified <see cref="ICondition"/> element.
        /// </summary>
        /// <param name="condition">An <see cref="ICondition"/> element.</param>
        /// <returns>
        /// A <see cref="NotCondition"/> if the specified element is not a <see cref="NotCondition"/>;
        /// otherwise, an inner <see cref="ICondition"/> element of the specified element.
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        public static ICondition Not(ICondition condition)
        {
            ArgumentNullException.ThrowIfNull(condition);
            if (condition is NotCondition complement)
            {
                return complement.Condition;
            }
            return new NotCondition(condition);
        }

        /// <summary>
        /// Returns a conjunction of the specified <see cref="ICondition"/> elements.
        /// </summary>
        /// <param name="conditions"><see cref="ICondition"/> elements.</param>
        /// <returns>
        /// <see langword="null"/> if there is no specified element.<br/>
        /// An <see cref="ICondition"/> element if there is only one specified element.<br/>
        /// A <see cref="NullCondition"/> if at least one of the specified elements is a <see cref="NullCondition"/>.<br/>
        /// Otherwise, an <see cref="AndCondition"/> that contains the specified <see cref="ICondition"/> elements.
        /// </returns>
        /// <remarks>
        /// All of the <see langword="null"/> in the <paramref name="conditions"/> will be excluded.
        /// </remarks>
        public static ICondition And(params ICondition[] conditions)
        {
            var entries = conditions.Where(cond => cond is not null);
            if (!entries.Any())
            {
                return null;
            }
            if (entries.Take(2).Count() == 1)
            {
                return entries.Single();
            }
            if (entries.Contains(Null))
            {
                return Null;
            }
            return new AndCondition(entries);
        }

        /// <summary>
        /// Returns a disjunction of the specified <see cref="ICondition"/> elements.
        /// </summary>
        /// <param name="conditions"><see cref="ICondition"/> elements.</param>
        /// <returns>
        /// A <see cref="NullCondition"/> if there is no specified element which is not a <see cref="NullCondition"/>.<br/>
        /// An <see cref="ICondition"/> element if there is only one specified element.<br/>
        /// Otherwise, an <see cref="OrCondition"/> that contains the specified <see cref="ICondition"/> elements.
        /// </returns>
        /// <remarks>
        /// All of the <see langword="null"/> in the <paramref name="conditions"/> will be excluded.
        /// </remarks>
        public static ICondition Or(params ICondition[] conditions)
        {
            var entries = conditions.Where(cond => cond is not null);
            if (!entries.Any(cond => cond != Null))
            {
                return Null;
            }
            if (entries.Take(2).Count() == 1)
            {
                return entries.Single();
            }
            return new OrCondition(entries);
        }
    }
}
