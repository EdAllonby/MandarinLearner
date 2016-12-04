namespace MandarinLearner.ViewModel
{
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