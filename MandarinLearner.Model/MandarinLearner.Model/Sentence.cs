using System.Collections.Generic;

namespace MandarinLearner.Model
{
    public class Sentence
    {
        public int Id { get; set; }

        public string SimplifiedChinese { get; set; }
        public string Pinyin { get; set; }
        public string English { get; set; }

        public virtual ICollection<Phrase> RelatedPhrases { get; set; }

        public virtual ICollection<MeasureWord> RelatedMeasureWords { get; set; }
    }
}