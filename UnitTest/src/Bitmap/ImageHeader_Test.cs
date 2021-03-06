using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.Bitmap;

namespace UnitTest.src.Bitmap
{
	[TestClass]
	public class ImageHeader_Test
	{
		[TestMethod]
		public void Create()
		{
			int width = 10;
			int height = 10;
			ushort bitCount = 1;

			// Improper Bit Count throws
			Assert.ThrowsException<Exception>(() => ImageHeader.Create(width, height, 3));

			ImageHeader imageHeader;
			imageHeader = ImageHeader.Create(width, height, bitCount);

			Assert.AreEqual(ImageHeader.SizeInBytes, (int)imageHeader.headerSize);
			Assert.AreEqual(width, imageHeader.width);
			Assert.AreEqual(height, imageHeader.height);
			Assert.AreEqual(ImageHeader.DefaultPlanes, imageHeader.planes);
			Assert.AreEqual(bitCount, imageHeader.bitCount);
			Assert.AreEqual(BmpCompression.Uncompressed, imageHeader.compression);
			Assert.AreEqual(ImageHeader.DefaultImageSize, imageHeader.imageSize);
			Assert.AreEqual(ImageHeader.DefaultXResolution, imageHeader.xResolution);
			Assert.AreEqual(ImageHeader.DefaultYResolution, imageHeader.yResolution);
			Assert.AreEqual(ImageHeader.DefaultUsedColorMapEntries, imageHeader.usedColorMapEntries);
			Assert.AreEqual(ImageHeader.DefaultImportantColorCount, imageHeader.importantColorCount);
		}

		[TestMethod]
		public void IsValidBitCount()
		{
			foreach (var bitCount in ImageHeader.ValidBitCounts) {
				Assert.IsTrue(ImageHeader.IsValidBitCount(bitCount));
			}

			Assert.IsFalse(ImageHeader.IsValidBitCount(3));

			// Test non-static version of function
			ImageHeader imageHeader = new ImageHeader();
			imageHeader.bitCount = 1;
			Assert.IsTrue(imageHeader.IsValidBitCount());
			imageHeader.bitCount = 3;
			Assert.IsFalse(imageHeader.IsValidBitCount());
		}

		[TestMethod]
		public void IsIndexedImage()
		{
			Assert.IsTrue(ImageHeader.IsIndexedImage(1));
			Assert.IsTrue(ImageHeader.IsIndexedImage(8));
			Assert.IsFalse(ImageHeader.IsIndexedImage(16));

			// Test non-static version of function
			ImageHeader imageHeader = new ImageHeader();
			imageHeader.bitCount = 1;
			Assert.IsTrue(imageHeader.IsIndexedImage());
			imageHeader.bitCount = 16;
			Assert.IsFalse(imageHeader.IsIndexedImage());
		}

		[TestMethod]
		public void VerifyValidBitCount()
		{
			foreach (var bitCount in ImageHeader.ValidBitCounts) {
				ImageHeader.VerifyValidBitCount(bitCount);
			}
	
			Assert.ThrowsException<Exception>(() => ImageHeader.VerifyValidBitCount(0));
			Assert.ThrowsException<Exception>(() => ImageHeader.VerifyValidBitCount(3));

			// Test non-static version of function
			ImageHeader imageHeader = new ImageHeader();
			imageHeader.bitCount = 1;
			imageHeader.VerifyValidBitCount();
			imageHeader.bitCount = 3;
			Assert.ThrowsException<Exception>(() => imageHeader.VerifyValidBitCount());
		}

		[TestMethod]
		public void CalculatePitch()
		{
			Assert.AreEqual(4, ImageHeader.CalculatePitch(1, 1));
			Assert.AreEqual(4, ImageHeader.CalculatePitch(1, 32));
			Assert.AreEqual(8, ImageHeader.CalculatePitch(1, 33));

			Assert.AreEqual(4, ImageHeader.CalculatePitch(4, 1));
			Assert.AreEqual(4, ImageHeader.CalculatePitch(4, 8));
			Assert.AreEqual(8, ImageHeader.CalculatePitch(4, 9));

			Assert.AreEqual(4, ImageHeader.CalculatePitch(8, 1));
			Assert.AreEqual(4, ImageHeader.CalculatePitch(8, 4));
			Assert.AreEqual(8, ImageHeader.CalculatePitch(8, 5));

			// Test non-static version of function
			ImageHeader imageHeader = new ImageHeader();
			imageHeader.bitCount = 1;
			imageHeader.width = 1;
			Assert.AreEqual(4, imageHeader.CalculatePitch());
		}

		[TestMethod]
		public void CalcByteWidth()
		{
			Assert.AreEqual(1, ImageHeader.CalcPixelByteWidth(1, 1));
			Assert.AreEqual(1, ImageHeader.CalcPixelByteWidth(1, 8));
			Assert.AreEqual(2, ImageHeader.CalcPixelByteWidth(1, 9));

			Assert.AreEqual(1, ImageHeader.CalcPixelByteWidth(4, 1));
			Assert.AreEqual(1, ImageHeader.CalcPixelByteWidth(4, 2));
			Assert.AreEqual(2, ImageHeader.CalcPixelByteWidth(4, 3));

			Assert.AreEqual(1, ImageHeader.CalcPixelByteWidth(8, 1));
			Assert.AreEqual(2, ImageHeader.CalcPixelByteWidth(8, 2));

			// Test non-static version of function
			ImageHeader imageHeader = new ImageHeader();
			imageHeader.bitCount = 1;
			imageHeader.width = 1;
			Assert.AreEqual(1, imageHeader.CalcPixelByteWidth());
		}

		[TestMethod]
		public void Validate()
		{
			ImageHeader imageHeader = ImageHeader.Create(1, 1, 1);

			imageHeader.Validate();

			imageHeader.headerSize = ImageHeaderV4.SizeInBytes;
			TestInvalidImageHeaderSize(imageHeader, "version 4");

			imageHeader.headerSize = ImageHeaderV5.SizeInBytes;
			TestInvalidImageHeaderSize(imageHeader, "version 5");
	
			imageHeader.headerSize = 0;
			TestInvalidImageHeaderSize(imageHeader, "Unknown");

			imageHeader.headerSize = ImageHeader.SizeInBytes;

			//imageHeader.headerSize = 0;
			//Assert.ThrowsException<Exception>(() => imageHeader.Validate());
			//imageHeader.headerSize = ImageHeader.SizeInBytes;

			imageHeader.planes = 0;
			Assert.ThrowsException<Exception>(() => imageHeader.Validate());
			imageHeader.planes = ImageHeader.DefaultPlanes;

			imageHeader.bitCount = 3;
			Assert.ThrowsException<Exception>(() => imageHeader.Validate());
			imageHeader.bitCount = 1;

			imageHeader.usedColorMapEntries = 3;
			Assert.ThrowsException<Exception>(() => imageHeader.Validate());
			imageHeader.usedColorMapEntries = ImageHeader.DefaultUsedColorMapEntries;

			imageHeader.importantColorCount = 3;
			Assert.ThrowsException<Exception>(() => imageHeader.Validate());
			imageHeader.importantColorCount = ImageHeader.DefaultImportantColorCount;
		}

		void TestInvalidImageHeaderSize(ImageHeader imageHeader, string exceptionSubstring)
		{
			try
			{
				try
				{
					imageHeader.Validate();
				}
				catch (System.Exception e)
				{
					Assert.IsTrue(e.ToString().Contains(exceptionSubstring));
					return;
				}
			}
			catch (System.Exception)
			{
				throw new System.Exception("An ImageHeader containing the size of " + exceptionSubstring + " threw the wrong type of exception");
			}

			throw new System.Exception("An ImageHeader containing the size of " + exceptionSubstring + " should have thrown an exception, but did not");
		}

		[TestMethod]
		public void Equality()
		{
			ImageHeader imageHeader1 = ImageHeader.Create(1, 1, 1);
			ImageHeader imageHeader2 = ImageHeader.Create(1, 1, 1);

			Assert.IsTrue(imageHeader1 == imageHeader2);
			Assert.IsFalse(imageHeader1 != imageHeader2);

			imageHeader2.bitCount = 8;
			Assert.IsFalse(imageHeader1 == imageHeader2);
			Assert.IsTrue(imageHeader1 != imageHeader2);
		}
	}
}
