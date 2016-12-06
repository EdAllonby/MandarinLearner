namespace MandarinLearner.ViewModel
{
    /// <summary>
    /// Makes an item selectable in a view model.
    /// </summary>
    /// <typeparam name="TItem">The item to take selectable.</typeparam>
    public class SelectableItem<TItem> : ViewModel
    {
        private bool isSelected;

        public SelectableItem(TItem item)
        {
            Item = item;
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public TItem Item { get; }
    }
}