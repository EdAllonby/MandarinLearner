using System.Collections.Generic;

namespace MandarinLearner.Model
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Phrase> Phrases { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}