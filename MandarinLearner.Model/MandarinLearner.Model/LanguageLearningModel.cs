using System.Data.Entity;

namespace MandarinLearner.Model
{
    public class LanguageLearningModel : DbContext
    {
        public virtual DbSet<Phrase> Phrases { get; set; }
        public virtual DbSet<HskPhrase> HskPhrases { get; set; }

        public virtual DbSet<MeasureWord> MeasureWords { get; set; }

        public virtual DbSet<Tag> Tags { get; set; }

        public virtual DbSet<Sentence> Sentences { get; set; }
    }
}