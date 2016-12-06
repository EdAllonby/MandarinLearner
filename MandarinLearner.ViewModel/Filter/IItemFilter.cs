using System.Collections.Generic;

namespace MandarinLearner.ViewModel.Filter
{
    public interface IItemFilter<T>
    {
        void Filter(IEnumerable<T> fullCollection, ICollection<T> collectionToFilter);
    }
}