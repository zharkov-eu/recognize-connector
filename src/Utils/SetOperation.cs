using System.Linq;
using System.Collections.Generic;

namespace ExpertSystem.Utils
{
    public static class SetOperation
    {
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(emptyProduct, (accumulator, sequence) =>
                from accseq in accumulator
                from item in sequence
                select accseq.Concat(new[] { item })
            );
        }
    }
}