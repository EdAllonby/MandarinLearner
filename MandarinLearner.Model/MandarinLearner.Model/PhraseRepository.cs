using System;
using System.Collections.Generic;
using System.Linq;

namespace MandarinLearner.Model
{
    public class PhraseRepository
    {
        private readonly Dictionary<string, Phrase> phrasesIndexedByEnglish = new Dictionary<string, Phrase>();
        private readonly Random random = new Random();

        public void LoadAll()
        {
            IEnumerable<Phrase> phrases = PhraseParser.ParseAll();

            foreach (Phrase phrase in phrases)
            {
                phrasesIndexedByEnglish.Add(phrase.EnglishPhrase, phrase);
            }
        }

        public Phrase GetPhraseByEnglish(string english)
        {
            return phrasesIndexedByEnglish.ContainsKey(english) ? phrasesIndexedByEnglish[english] : null;
        }

        public Phrase GetRandomPhrase()
        {
            return phrasesIndexedByEnglish.ElementAt(random.Next(0, phrasesIndexedByEnglish.Count)).Value;
        }
    }
}