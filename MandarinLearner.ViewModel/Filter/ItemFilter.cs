using System;
using System.Collections.Generic;
using System.Linq;

namespace MandarinLearner.ViewModel.Filter
{
    public class ItemFilter<T> : IItemFilter<T>
    {
        private readonly Func<T, bool> filterBySearchTerms;

        public ItemFilter(Func<T, bool> filterBySearchTerms)
        {
            this.filterBySearchTerms = filterBySearchTerms;
        }

        public void Filter(IEnumerable<T> startingCollection, ICollection<T> collectionToFilter)
        {
            collectionToFilter.Clear();

            IEnumerable<T> finalFilter = FilterItems(startingCollection);

            foreach (T selectableItem in finalFilter)
            {
                collectionToFilter.Add(selectableItem);
            }
        }

        public IEnumerable<T> FilterItems(IEnumerable<T> phrasesToFilter)
        {
            return phrasesToFilter.Where(filterBySearchTerms);
        }
    }
}