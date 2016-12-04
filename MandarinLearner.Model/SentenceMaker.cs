using System.Collections.Generic;
using log4net;

namespace MandarinLearner.Model
{
    public sealed class SentenceMaker
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SentenceMaker));

        public static void AddSentence(string english, string pinyin, string chinese, List<Phrase> relatedPhrases, List<MeasureWord> relatedMeasureWords)
        {
            var sentence = new Sentence
            {
                Pinyin = pinyin,
                English = english,
                Hanzi = chinese,
                RelatedPhrases = relatedPhrases,
                RelatedMeasureWords = relatedMeasureWords
            };

            using (var context = new LanguageLearningModel())
            {
                // Phrases and measure words must already exist.
                foreach (Phrase relatedPhrase in relatedPhrases)
                {
                    context.Phrases.Attach(relatedPhrase);
                }
                foreach (MeasureWord relatedMeasureWord in relatedMeasureWords)
                {
                    context.MeasureWords.Attach(relatedMeasureWord);
                }

                Log.InfoFormat("Adding sentence [{0}] with {1} phrases and {2} measure words", sentence.Pinyin, relatedPhrases.Count, relatedMeasureWords.Count);

                context.Sentences.Add(sentence);
                context.SaveChanges();
                Log.InfoFormat("Sentence [{0}] added", sentence.Pinyin);
            }
        }
    }
}