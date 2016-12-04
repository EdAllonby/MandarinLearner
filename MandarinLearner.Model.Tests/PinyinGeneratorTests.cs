using NUnit.Framework;

namespace MandarinLearner.Model.Tests
{
    [TestFixture]
    internal class PinyinGeneratorTests
    {
        [TestCase("你", ExpectedResult = "ni3")]
        [TestCase("名字", ExpectedResult = "ming2 zi4")]
        [TestCase("我会说一点中文", ExpectedResult = "wo3 hui4 shuo1 yi1 dian3 zhong1 wen2")]
        public string HanziReturnsCorrectPinyin(string hanzi)
        {
            var generator = new PinyinGenerator();
            return generator.GetPinyinFromHanzi(hanzi);
        }
    }
}