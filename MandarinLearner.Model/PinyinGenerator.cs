using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MandarinLearner.Model.Properties;

namespace MandarinLearner.Model
{
    public sealed class PinyinGenerator
    {
        private readonly Dictionary<char, IEnumerable<string>> pinyinIndexedByUnicode = new Dictionary<char, IEnumerable<string>>();

        public PinyinGenerator()
        {
            foreach (string item in SplitToLines(Resources.UnicodePinyinLookup))
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

            foreach (char character in hanzi)
            {
                if (IsEnglish(character))
                {
                    pinyin.Append(character);
                    continue;
                }

                switch (character)
                {
                    case '，':
                        pinyin.Append(',');
                        continue;
                    case '。':
                        pinyin.Append('.');
                        continue;
                    case '！':
                        pinyin.Append('!');
                        continue;
                    case '？':
                        pinyin.Append('?');
                        continue;
                }
                IEnumerable<string> possiblePinyinCharacters;

                if (!pinyinIndexedByUnicode.TryGetValue(character, out possiblePinyinCharacters))
                {
                    continue;
                }

                pinyin.Append($" {possiblePinyinCharacters.First()}");
            }

            return pinyin.ToString().Trim();
        }

        private static bool IsEnglish(char c)
        {
            // ASCII Range is up to 128
            return c < 128;
        }

        private static IEnumerable<string> SplitToLines(string input)
        {
            if (input == null)
            {
                yield break;
            }

            using (var reader = new StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}