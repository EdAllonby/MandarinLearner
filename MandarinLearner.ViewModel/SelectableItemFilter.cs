using System;
using System.Collections.Generic;
using System.Linq;

namespace MandarinLearner.ViewModel
{
    public static class SelectableItemFilter<T>
    {
        public static void Filter(IEnumerable<SelectableItem<T>> startingCollection, ICollection<SelectableItem<T>> collectionToFilter, bool filterSelected, Func<T, bool> filterBySearchTerms)
        {
            collectionToFilter.Clear();

            IEnumerable<SelectableItem<T>> filteredBySelected = FilterBySelectedItem(startingCollection, filterSelected);

            IEnumerable<SelectableItem<T>> finalFilter = FilterByItem(filteredBySelected, filterBySearchTerms);

            foreach (SelectableItem<T> selectableItem in finalFilter)
            {
                collectionToFilter.Add(selectableItem);
            }
        }

        private static IEnumerable<SelectableItem<T>> FilterBySelectedItem(IEnumerable<SelectableItem<T>> startingColection, bool filterSelected)
        {
            foreach (SelectableItem<T> availablePhrase in startingColection)
            {
                if (!filterSelected)
                {
                    yield return availablePhrase;
                }
                else if (availablePhrase.IsSelected)
                {
                    yield return availablePhrase;
                }
            }
        }

        private static IEnumerable<SelectableItem<T>> FilterByItem(IEnumerable<SelectableItem<T>> phrasesToFilter, Func<T, bool> filterBySearchTerms)
        {
            return phrasesToFilter.Where(availablePhrase => filterBySearchTerms(availablePhrase.Item));
        }
    }
}