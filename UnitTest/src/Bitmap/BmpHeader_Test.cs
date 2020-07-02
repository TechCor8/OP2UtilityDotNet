using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.Bitmap;

namespace UnitTest.src.Bitmap
{
	[TestClass]
	public class BmpHeader_Test
	{
		[TestMethod]
		public void Create()
		{
			const uint fileSize = 100;
			const uint pixelOffset = 50;

			BmpHeader bmpHeader;
			bmpHeader = BmpHeader.Create(fileSize, pixelOffset);

			CollectionAssert.AreEqual(BmpHeader.FileSignature, bmpHeader.fileSignature);
			Assert.AreEqual(fileSize, bmpHeader.size);
			Assert.AreEqual(BmpHeader.DefaultReserved1, bmpHeader.reserved1);
			Assert.AreEqual(BmpHeader.DefaultReserved2, bmpHeader.reserved2);
			Assert.AreEqual(pixelOffset, bmpHeader.pixelOffset);
		}

		[TestMethod]
		public void IsValidFileSignature() 
		{
			BmpHeader bmpHeader = new BmpHeader();

			bmpHeader.fileSignature = BmpHeader.FileSignature.ToArray();
			Assert.IsTrue(bmpHeader.IsValidFileSignature());

			bmpHeader.fileSignature[0] = (byte)'b';
			Assert.IsFalse(bmpHeader.IsValidFileSignature());
		}

		[TestMethod]
		public void VerifyFileSignature()
		{
			BmpHeader bmpHeader = new BmpHeader();

			bmpHeader.fileSignature = BmpHeader.FileSignature.ToArray();
			bmpHeader.VerifyFileSignature();

			bmpHeader.fileSignature[0] = (byte)'b';
			Assert.ThrowsException<Exception>(() => bmpHeader.VerifyFileSignature());
		}

		[TestMethod]
		public void Equality()
		{
			var bmpHeader1 = BmpHeader.Create(1, 1);
			var bmpHeader2 = BmpHeader.Create(1, 1);

			Assert.IsTrue(bmpHeader1 == bmpHeader2);
			Assert.IsFalse(bmpHeader1 != bmpHeader2);

			bmpHeader2.size = 2;
			Assert.IsFalse(bmpHeader1 == bmpHeader2);
			Assert.IsTrue(bmpHeader1 != bmpHeader2);
		}
	}
}
