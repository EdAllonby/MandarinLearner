using System.Collections.Generic;

namespace MandarinLearner.Model
{
    /// <summary>
    /// Holds the english, pinyin and simplified chinese counterparts of a phrase.
    /// </summary>
    public class Phrase
    {
        // The unique identity of the Phrase.
        public int Id { get; set; }

        /// <summary>
        /// The readonly english translation
        /// </summary>
        public string English { get; set; }

        /// <summary>
        /// The readonly Pinyin representation of the Hanzi
        /// </summary>
        public string Pinyin { get; set; }

        /// <summary>
        /// The readonly Hanzi (Simplified Chinese) phrase
        /// </summary>
        public string Hanzi { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public virtual ICollection<MeasureWord> MeasureWords { get; set; }

        public virtual ICollection<Sentence> SentenceUsages { get; set; }

        public string DisplayableMeasureWords
        {
            get
            {
                if (MeasureWords.IsNullOrEmpty())
                {
                    return string.Empty;
                }

                var measureWords = "MW: ";
                const string seperator = ", ";

                foreach (MeasureWord measureWord in MeasureWords)
                {
                    measureWords += $"{measureWord.Hanzi} {measureWord.Pinyin}";
                    measureWords += seperator;
                }

                return measureWords.Remove(measureWords.Length - seperator.Length);
            }
        }

        public override string ToString()
        {
            return Pinyin;
        }
    }
}