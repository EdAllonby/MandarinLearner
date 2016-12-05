using System;
using System.Collections.Generic;
using System.Linq;

namespace MandarinLearner.ViewModel
{
    public static class ItemFilter<T>
    {
        public static void Filter(IEnumerable<T> startingCollection, ICollection<T> collectionToFilter, Func<T, bool> filterBySearchTerms)
        {
            collectionToFilter.Clear();

            IEnumerable<T> finalFilter = FilteredItems(startingCollection, filterBySearchTerms);

            foreach (T selectableItem in finalFilter)
            {
                collectionToFilter.Add(selectableItem);
            }
        }

        public static IEnumerable<T> FilteredItems(IEnumerable<T> phrasesToFilter, Func<T, bool> filterBySearchTerms)
        {
            return phrasesToFilter.Where(filterBySearchTerms);
        }
    }
}