using NUnit.Framework;
using Bedrock;
using System;

namespace BedrockTest
{
    public class TestJson
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestToJson()
        {
            var bagObject = BagObject.Open("hello", "world");
            var bagObjectAsString = bagObject.ToString();
            Assert.AreEqual(bagObjectAsString, "{\"hello\":\"world\"}");
        }

        [Test]
        public void TestFromJson()
        {
            var srcString = "{\"hello\":\"world\"}";
            var bagObject = new FormatReaderJson(srcString).ReadBagObject();
            Assert.NotNull(bagObject);
            Assert.AreEqual(bagObject.GetString("hello"), "world");
        }
    }
}
