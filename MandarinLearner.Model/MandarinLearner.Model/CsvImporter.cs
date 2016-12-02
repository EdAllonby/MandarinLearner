using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using log4net;

namespace MandarinLearner.Model
{
    public sealed class CsvImporter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CsvImporter));

        public void ImportHskFormat(string hskCsvFile)
        {
            using (var sr = new StreamReader(hskCsvFile))
            using (var csv = new CsvReader(sr))
            {
                while (csv.Read())
                {
                    var chineseWord = csv.GetField<string>("Word");
                    var pinyin = csv.GetField<string>("Pronunciation");
                    var englishDefinition = csv.GetField<string>("Definition");
                    int hskLevel = ParseHskLevel(csv);
                    string[] splitEnglish = englishDefinition.Split(new[] { "CL:" }, StringSplitOptions.None);
                    string english = splitEnglish[0].Trim();
                    var phrase = new HskPhrase { SimplifiedChinesePhrase = chineseWord, PinyinPhrase = pinyin, EnglishPhrase = english, HskLevel = hskLevel };

                    List<MeasureWord> measureWords = FindMeasureWords(splitEnglish).ToList();
                    var tag = new Tag { Name = $"HSK{hskLevel}" };
                    phrase.Tags = new List<Tag> { tag };
                    phrase.MeasureWords = new List<MeasureWord>();

                    using (var context = new LanguageLearningModel())
                    {
                        Log.DebugFormat("Adding phrase [{0}]", phrase.PinyinPhrase);

                        context.Phrases.Add(phrase);

                        foreach (MeasureWord measureWord in measureWords)
                        {
                            MeasureWord measureWordToAddToPhrase;

                            MeasureWord foundMeasureWord = context.MeasureWords.FirstOrDefault(mw => mw.SimplifiedChinese == measureWord.SimplifiedChinese);
                            if (foundMeasureWord != null)
                            {
                                Log.InfoFormat("Not adding Measure Word [{0}], already added.", foundMeasureWord.Pinyin);
                                measureWordToAddToPhrase = foundMeasureWord;
                            }
                            else
                            {
                                measureWordToAddToPhrase = measureWord;

                                Log.DebugFormat("Adding measure word [{0}, {1}]", measureWord.SimplifiedChinese, measureWord.Pinyin);

                                context.MeasureWords.Add(measureWord);
                            }

                            phrase.MeasureWords.Add(measureWordToAddToPhrase);
                            context.SaveChanges();
                        }

                        context.SaveChanges();
                    }
                }
            }
        }

        private static IEnumerable<MeasureWord> FindMeasureWords(string[] splitEnglish)
        {
            if (splitEnglish.Length <= 1)
            {
                yield break;
            }

            string includesMeasureWordPart = splitEnglish[1].Split(';')[0];


            string[] measureWordsFound = includesMeasureWordPart.Split(',');
            foreach (string measureWordComponents in measureWordsFound)
            {
                string simplifiedChineseWord = measureWordComponents.Split('[')[0];
                string[] pinyinMeasureWord = measureWordComponents.Split('[', ']');

                if (pinyinMeasureWord.Length > 1)
                {
                    yield return new MeasureWord { Pinyin = pinyinMeasureWord[1], SimplifiedChinese = simplifiedChineseWord };
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