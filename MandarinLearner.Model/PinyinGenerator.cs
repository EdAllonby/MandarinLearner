using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MandarinLearner.Model.Properties;

namespace MandarinLearner.Model
{
    public sealed class PinyinGenerator
    {
        private readonly Dictionary<char, IEnumerable<string>> pinyinIndexedByUnicode = new Dictionary<char, IEnumerable<string>>();

        public PinyinGenerator()
        {
            string lookup = Resources.UnicodePinyinLookup;

            foreach (string item in lookup.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }

                string[] pinyinComponents = item.Split('\t');
                int num = int.Parse(pinyinComponents[0], NumberStyles.AllowHexSpecifier);
                char key = Convert.ToChar(num);
                pinyinIndexedByUnicode.Add(key, pinyinComponents.Skip(1));
            }
        }

        public string GetPinyinFromHanzi(string hanzi)
        {
            var pinyin = new StringBuilder();

            foreach (char hanziCharacter in hanzi)
            {
                IEnumerable<string> possiblePinyinCharacters;

                if (!pinyinIndexedByUnicode.TryGetValue(hanziCharacter, out possiblePinyinCharacters))
                {
                    continue;
                }

                pinyin.Append($" {possiblePinyinCharacters.First()}");
            }

            return pinyin.ToString().Trim();
        }
    }
}