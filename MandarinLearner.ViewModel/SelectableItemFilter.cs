using System;
using System.Collections.Generic;

namespace MandarinLearner.ViewModel
{
    public static class SelectableItemFilter<T>
    {
        public static void Filter(IEnumerable<SelectableItem<T>> startingCollection, ICollection<SelectableItem<T>> collectionToFilter, bool filterSelected, Func<SelectableItem<T>, bool> filterBySearchTerms)
        {
            collectionToFilter.Clear();

            IEnumerable<SelectableItem<T>> filteredBySelected = FilterBySelectedItem(startingCollection, filterSelected);

            IEnumerable<SelectableItem<T>> finalFilter = ItemFilter<SelectableItem<T>>.FilteredItems(filteredBySelected, filterBySearchTerms);

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
    }
}