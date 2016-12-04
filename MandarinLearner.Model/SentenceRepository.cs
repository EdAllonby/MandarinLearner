using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace MandarinLearner.Model
{
    public class SentenceRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SentenceRepository));

        public static async Task<IEnumerable<Sentence>> GetAllSentencesAsync()
        {
            return await Task.Run(() =>
            {
                using (var context = new LanguageLearningModel())
                {
                    Log.Debug("Loading all Sentences");
                    List<Sentence> results = context.Sentences.Include(x => x.RelatedMeasureWords).Include(x => x.RelatedPhrases).ToList();
                    Log.DebugFormat("Completed loading {0} Sentences.", results.Count);
                    return results;
                }
            });
        }
    }
}