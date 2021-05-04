using NUnit.Framework;
using Bedrock;

namespace BedrockTest
{
    public class TestBagArray
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var bagArray = BagArray
                .Open(5)
                .Add (TestEnum.Happy);
            Assert.AreEqual(bagArray.Count, 2);
        }
    }
}