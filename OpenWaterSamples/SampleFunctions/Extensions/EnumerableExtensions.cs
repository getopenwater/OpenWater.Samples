using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleFunctions.Extensions
{
    public static class EnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> self, T value)
        {
            var comparer = EqualityComparer<T>.Default;
            var found = self.Select((el, index) => new { Element = el, Index = index }).FirstOrDefault(i => comparer.Equals(i.Element, value));
            return found?.Index ?? -1;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> self)
        {
            return self == null || !self.Any();
        }

        public static object Cast(this IEnumerable<object> self, Type type)
        {
            var castMethod = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(type);
            var castIterator = castMethod.Invoke(null, new object[] { self });

            var toArrayMethod = typeof(Enumerable).GetMethod("ToArray").MakeGenericMethod(type);
            return toArrayMethod.Invoke(null, new[] { castIterator });
        }

        public static IEnumerable<T> Reorder<T>(this IEnumerable<T> self, IEnumerable<T> correctOrder)
        {
            var result = new List<T>();
            var selfList = new List<T>(self);
            var correctOrderList = new List<T>(correctOrder);

            while (selfList.Count > 0)
            {
                if (!correctOrder.Contains(selfList.First()))
                {
                    result.Add(selfList.First());
                }
                else
                {
                    result.Add(correctOrderList.First());
                    correctOrderList.RemoveAt(0);
                }

                selfList.RemoveAt(0);
            }

            return result.ToArray();
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> self, int batchSize)
        {
            var nextBatch = new List<T>(batchSize);
            foreach (T item in self)
            {
                nextBatch.Add(item);
                if (nextBatch.Count == batchSize)
                {
                    yield return nextBatch;
                    nextBatch = new List<T>(batchSize);
                }
            }

            if (nextBatch.Count > 0)
                yield return nextBatch;
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, Comparer<TKey>.Default);
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentException("source");
            if (selector == null)
                throw new ArgumentException("selector");
            if (comparer == null)
                throw new ArgumentException("comparer");

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    throw new InvalidOperationException("Sequence was empty");

                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }

                return min;
            }
        }
    }
}
