using System.Collections.Generic;

namespace MandarinLearner.Model
{
    public class MeasureWord
    {
        public int Id { get; set; }

        public virtual ICollection<Phrase> Noun { get; set; }

        public string SimplifiedChinese { get; set; }

        public string Pinyin { get; set; }
    }
}