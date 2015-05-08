using System.Collections.Generic;
using System.Linq;
using LinqToExcel;

namespace MandarinLearner.Model
{
    public static class PhraseParser
    {
        public static IEnumerable<Phrase> ParseAll()
        {
            const string FilePath = @"Resources\database.xls";

            var query = new ExcelQueryFactory(FilePath);

            return query.Worksheet<PhraseMapper>().Select(phrase => new Phrase(phrase));
        }
    }
}