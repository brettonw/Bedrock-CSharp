using NUnit.Framework;
using Bedrock;


namespace BedrockTest
{
    enum TestEnum { Happy, Sad }

    public class TestBagObject
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var bagObject = BagObject
                .Open("hello", "world")
                .Put("jump", BagObject.Open("black", "queen"))
                .Put("integer", 100)
                .Put("float", 100.0f)
                .Put("double", 100.0)
                .Put("long", 100L)
                .Put("bool", true)
                .Put("char", 'X')
                .Put("byte", (byte)25)
                .Put("unsigned", 100U)
                .Put("short", (short)16000)
                .Put("ushort", (ushort)16000)
                .Put("ulong", 16000UL)
                .Put("uint", 6000U)
                .Put("sbyte", (sbyte)60)
                .Put("enum", TestEnum.Happy);
            Assert.AreEqual(bagObject.Count, 16);
        }
    }
}