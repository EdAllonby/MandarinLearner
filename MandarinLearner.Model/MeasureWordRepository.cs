using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace MandarinLearner.Model
{
    public class MeasureWordRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MeasureWordRepository));

        public static async Task<IEnumerable<MeasureWord>> GetAllMeasureWordsAsync()
        {
            return await Task.Run(() =>
            {
                using (var context = new LanguageLearningModel())
                {
                    Log.Debug("Loading all Measure Words");
                    List<MeasureWord> results = context.MeasureWords.ToList();
                    Log.DebugFormat("Completed loading {0} Measure Words.", results.Count);
                    return results;
                }
            });
        }
    }
}