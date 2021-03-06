﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using log4net;
using MandarinLearner.Model.Properties;

namespace MandarinLearner.Model
{
    public sealed class HskPhraseImporter : IPhraseImporter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HskPhraseImporter));

        public async Task ImportPhrasesAsync()
        {
            await Task.Run(() =>
            {
                using (var sr = new StringReader(Resources.Hsk6))
                using (var csv = new CsvReader(sr))
                {
                    while (csv.Read())
                    {
                        string chineseWord = GetColumn(csv, "Word");
                        string pinyin = GetColumn(csv, "Pronunciation");
                        string englishDefinition = GetColumn(csv, "Definition");
                        int hskLevel = ParseHskLevel(csv);

                        string[] splitEnglish = englishDefinition.Split(new[] { "CL:" }, StringSplitOptions.None);
                        string english = splitEnglish[0].Trim();
                        var phrase = new HskPhrase { Hanzi = chineseWord, Pinyin = pinyin, English = english, HskLevel = hskLevel };

                        List<MeasureWord> measureWords = FindMeasureWords(splitEnglish).ToList();

                        using (var context = new LanguageLearningModel())
                        {
                            phrase.Tags = new List<Tag>();
                            phrase.MeasureWords = new List<MeasureWord>();

                            Log.DebugFormat("Adding phrase [{0}]", phrase.Pinyin);

                            AddPhrase(context, phrase);

                            var tag = new Tag { Name = $"HSK{hskLevel}" };

                            AddElementToSet(context, tag, phrase.Tags, x => x.Name == tag.Name);

                            foreach (MeasureWord measureWord in measureWords)
                            {
                                AddElementToSet(context, measureWord, phrase.MeasureWords, mw => mw.Hanzi == measureWord.Hanzi);

                                context.SaveChanges();
                            }

                            context.SaveChanges();
                        }
                    }
                }
            });
        }

        private static string GetColumn(ICsvReaderRow csv, string columnName)
        {
            return csv.GetField<string>(columnName).Trim();
        }

        private static void AddElementToSet<T>(DbContext context, T element, ICollection<T> set, Func<T, bool> canAdd) where T : class
        {
            T elementToAddToPhrase;
            T foundTag = context.Set<T>().FirstOrDefault(canAdd);

            if (foundTag == null)
            {
                elementToAddToPhrase = element;
                Log.DebugFormat("Adding element [{0}]", elementToAddToPhrase);
                context.Set<T>().Add(element);
            }
            else
            {
                elementToAddToPhrase = foundTag;
            }

            if (!set.Any(canAdd))
            {
                set.Add(elementToAddToPhrase);
            }
        }

        private static void AddPhrase(LanguageLearningModel context, Phrase phrase)
        {
            if (!context.Phrases.Any(p => p.Hanzi == phrase.Hanzi))
            {
                context.Phrases.Add(phrase);
            }
            else
            {
                Log.WarnFormat("Phrase {0} already exists. Not adding.", phrase);
            }
        }

        private static IEnumerable<MeasureWord> FindMeasureWords(IReadOnlyList<string> splitEnglish)
        {
            if (splitEnglish.Count <= 1)
            {
                yield break;
            }

            string includesMeasureWordPart = splitEnglish[1].Split(';')[0];

            string[] measureWordsFound = includesMeasureWordPart.Split(',');
            foreach (string measureWordComponents in measureWordsFound)
            {
                string hanzi = measureWordComponents.Split('[')[0];
                string[] pinyinMeasureWord = measureWordComponents.Split('[', ']');

                if (pinyinMeasureWord.Length > 1)
                {
                    yield return new MeasureWord { Pinyin = pinyinMeasureWord[1], Hanzi = hanzi };
                }
                else
                {
                    Log.WarnFormat("Could not parse measure word from {0}", includesMeasureWordPart);
                }
            }
        }

        private static int ParseHskLevel(ICsvReaderRow csv)
        {
            int hskLevel;
            if (!csv.TryGetField("HSK Level", out hskLevel))
            {
                hskLevel = int.Parse(csv.GetField<string>("HSK Level-Order").Split('-')[0]);
            }
            return hskLevel;
        }
    }
}