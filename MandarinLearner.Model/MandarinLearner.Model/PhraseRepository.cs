using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace MandarinLearner.Model
{
    public class PhraseRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PhraseRepository));

        public static Phrase GetRandomPhrase()
        {
            using (var context = new LanguageLearningModel())
            {
                return context.Phrases.OrderBy(phrase => Guid.NewGuid()).First();
            }
        }

        public static async Task<Phrase> GetRandomHskPhraseByLevel(int hskLevel)
        {
            return await Task.Run(() =>
            {
                using (var context = new LanguageLearningModel())
                {
                    Log.DebugFormat("Loading random HSK Phrases found for level {0}.", hskLevel);
                    HskPhrase randomHskPhrase = context.HskPhrases.Include(x => x.MeasureWords).OrderBy(phrase => Guid.NewGuid()).First(x => x.HskLevel == hskLevel);
                    Log.DebugFormat("Completed loading random HSK Phrases found for level {0}.", hskLevel);
                    return randomHskPhrase;
                }
            });
        }

        public static async Task<IEnumerable<Phrase>> GetAllPhrasesFromLevelAsync(int hskLevel)
        {
            return await Task.Run(() =>
            {
                using (var context = new LanguageLearningModel())
                {
                    Log.DebugFormat("Loading all HSK Phrases found for level {0} and under.", hskLevel);
                    List<HskPhrase> results = context.HskPhrases.Include(x => x.MeasureWords).Where(x => x.HskLevel <= hskLevel).ToList();
                    Log.DebugFormat("Completed loading all HSK Phrases found for level {0} and under.", hskLevel);
                    return results;
                }
            });
        }
    }
}