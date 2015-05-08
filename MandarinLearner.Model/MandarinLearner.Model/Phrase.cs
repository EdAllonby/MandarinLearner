namespace MandarinLearner.Model
{
    /// <summary>
    /// Holds the english, pinyin and simplified chinese counterparts of a phrase.
    /// </summary>
    public sealed class Phrase
    {
        private readonly string englishPhrase;
        private readonly string pinyinPhrase;
        private readonly string simplifiedChinesePhrase;

        public Phrase(PhraseMapper phraseMapper)
        {
            englishPhrase = phraseMapper.English;
            pinyinPhrase = phraseMapper.Pinyin;
            simplifiedChinesePhrase = phraseMapper.Chinese;
        }

        /// <summary>
        /// The readonly english translation
        /// </summary>
        public string EnglishPhrase
        {
            get { return englishPhrase; }
        }

        /// <summary>
        /// The readonly Pinyin representation
        /// </summary>
        public string PinyinPhrase
        {
            get { return pinyinPhrase; }
        }

        /// <summary>
        /// The readonly simplified Chinese phrase
        /// </summary>
        public string SimplifiedChinesePhrase
        {
            get { return simplifiedChinesePhrase; }
        }
    }
}