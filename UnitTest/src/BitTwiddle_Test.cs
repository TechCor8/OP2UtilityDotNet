using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet;

namespace UnitTest.src
{
	[TestClass]
	public class BitTwiddle_Test
	{
		[TestMethod]
		public void IsPowerOf2()
		{
			Assert.IsTrue(BitTwiddle.IsPowerOf2(1));
			Assert.IsTrue(BitTwiddle.IsPowerOf2(2));
			Assert.IsTrue(BitTwiddle.IsPowerOf2(4));
			Assert.IsTrue(BitTwiddle.IsPowerOf2(8));
			Assert.IsTrue(BitTwiddle.IsPowerOf2(16));
			Assert.IsTrue(BitTwiddle.IsPowerOf2(32));
			Assert.IsTrue(BitTwiddle.IsPowerOf2(64));
			Assert.IsTrue(BitTwiddle.IsPowerOf2(128));
			Assert.IsTrue(BitTwiddle.IsPowerOf2(256));
			Assert.IsTrue(BitTwiddle.IsPowerOf2(512));

			Assert.IsTrue(BitTwiddle.IsPowerOf2(1u << 31));

			Assert.IsFalse(BitTwiddle.IsPowerOf2(0));
			Assert.IsFalse(BitTwiddle.IsPowerOf2(3));
			Assert.IsFalse(BitTwiddle.IsPowerOf2(5));
			Assert.IsFalse(BitTwiddle.IsPowerOf2(6));
			Assert.IsFalse(BitTwiddle.IsPowerOf2(7));
		}

		[TestMethod]
		public void Log2OfPowerOf2()
		{
			Assert.AreEqual(0u, BitTwiddle.Log2OfPowerOf2(1));
			Assert.AreEqual(1u, BitTwiddle.Log2OfPowerOf2(2));
			Assert.AreEqual(2u, BitTwiddle.Log2OfPowerOf2(4));
			Assert.AreEqual(3u, BitTwiddle.Log2OfPowerOf2(8));
			Assert.AreEqual(4u, BitTwiddle.Log2OfPowerOf2(16));
			Assert.AreEqual(5u, BitTwiddle.Log2OfPowerOf2(32));
			Assert.AreEqual(6u, BitTwiddle.Log2OfPowerOf2(64));
			Assert.AreEqual(7u, BitTwiddle.Log2OfPowerOf2(128));
			Assert.AreEqual(8u, BitTwiddle.Log2OfPowerOf2(256));
			Assert.AreEqual(9u, BitTwiddle.Log2OfPowerOf2(512));

			Assert.AreEqual(31u, BitTwiddle.Log2OfPowerOf2(1u << 31));
		}
	}
}
