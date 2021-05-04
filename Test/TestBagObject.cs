using NUnit.Framework;
using Bedrock;
using System;

namespace BedrockTest
{
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
                .Put("float", 100.5f)
                .Put("double", 100.5)
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

            Assert.AreEqual(bagObject.GetString("hello"), "world");
            Assert.AreEqual(bagObject.GetString("jump"), null);
            Assert.AreEqual(bagObject.GetString("quick"), null);

            Assert.AreEqual(bagObject.GetString("integer"), "100");
            Assert.AreEqual(bagObject.Get<Int32>("integer", () => 0), 100);

            Assert.AreEqual(bagObject.GetString("float"), "100.5");
            Assert.AreEqual(bagObject.Get<Single>("float", () => 0), 100.5f);

            Assert.AreEqual(bagObject.GetString("enum"), "Happy");
            Assert.AreEqual(bagObject.Get<TestEnum>("enum", () => TestEnum.Sad), TestEnum.Happy);
        }
    }
}