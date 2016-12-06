using System;
using System.Collections.Generic;

namespace MandarinLearner.ViewModel.Filter
{
    public class SelectableItemFilter<T> : IItemFilter<SelectableItem<T>>
    {
        private readonly bool filterSelected;
        private readonly ItemFilter<SelectableItem<T>> itemFilter;

        public SelectableItemFilter(bool filterSelected, Func<SelectableItem<T>, bool> filterBySearchTerms)
        {
            this.filterSelected = filterSelected;
            itemFilter = new ItemFilter<SelectableItem<T>>(filterBySearchTerms);
        }

        public void Filter(IEnumerable<SelectableItem<T>> fullCollection, ICollection<SelectableItem<T>> collectionToFilter)
        {
            collectionToFilter.Clear();

            IEnumerable<SelectableItem<T>> filteredBySelected = FilterBySelectedItem(fullCollection);

            IEnumerable<SelectableItem<T>> finalFilter = itemFilter.FilterItems(filteredBySelected);

            foreach (SelectableItem<T> selectableItem in finalFilter)
            {
                collectionToFilter.Add(selectableItem);
            }
        }

        private IEnumerable<SelectableItem<T>> FilterBySelectedItem(IEnumerable<SelectableItem<T>> startingColection)
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