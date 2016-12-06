using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MandarinLearner.ViewModel.Filter
{
    /// <summary>
    /// An observable collection which can be filtered.
    /// </summary>
    /// <typeparam name="TItem">The item to observe and filter.</typeparam>
    public sealed class SearcheableObservableCollection<TItem> : ViewModel
    {
        private readonly IEnumerable<TItem> availableItems;
        private ObservableCollection<TItem> displayableItems;

        public SearcheableObservableCollection(IEnumerable<TItem> items)
        {
            availableItems = items;
            DisplayableItems = new ObservableCollection<TItem>(availableItems);
        }

        public ObservableCollection<TItem> DisplayableItems
        {
            get { return displayableItems; }
            set
            {
                displayableItems = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<TItem> FindAvailable(Func<TItem, bool> predicate)
        {
            return availableItems.Where(predicate);
        }

        public bool AnyAvailable(Func<TItem, bool> predicate)
        {
            return availableItems.Any(predicate);
        }

        public void ApplyToAll(Action<TItem> action)
        {
            foreach (TItem availablePhrase in availableItems)
            {
                action(availablePhrase);
            }
        }

        public void Filter(IItemFilter<TItem> filter)
        {
            filter.Filter(availableItems, DisplayableItems);
        }
    }
}