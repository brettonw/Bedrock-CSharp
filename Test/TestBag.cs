using NUnit.Framework;
using Bedrock;
using System;

namespace BedrockTest
{
    public class TestBag : Bag
    {
        public override object GetObject(string key)
        {
            throw new System.NotImplementedException();
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestObjectify()
        {
            Assert.IsTrue(Objectify("string") is String);
            Assert.IsTrue(Objectify('X') is String);
            Assert.IsTrue(Objectify(TestEnum.Happy) is String);
            Assert.IsTrue(Objectify((Int64)100) is String);
            Assert.IsTrue(Objectify((UInt64)100) is String);
            Assert.IsTrue(Objectify((Int32)100) is String);
            Assert.IsTrue(Objectify((UInt32)100) is String);
            Assert.IsTrue(Objectify((Int16)100) is String);
            Assert.IsTrue(Objectify((UInt16)100) is String);
            Assert.IsTrue(Objectify((SByte)100) is String);
            Assert.IsTrue(Objectify((Byte)100) is String);
            Assert.IsTrue(Objectify(100.0f) is String);
            Assert.IsTrue(Objectify(100.0) is String);
            Assert.IsTrue(Objectify(true) is String);
        }
    }
}