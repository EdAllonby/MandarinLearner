using System.Threading.Tasks;

namespace MandarinLearner.Model
{
    public interface IPhraseImporter
    {
        Task ImportPhrasesAsync(string csvFile);
    }
}