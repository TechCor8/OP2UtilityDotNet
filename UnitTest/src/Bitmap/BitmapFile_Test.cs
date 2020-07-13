using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.Bitmap;

namespace UnitTest.src.Bitmap
{
	[TestClass]
	public class BitmapFile_Test
	{
		void RoundTripSub(ushort bitCount, uint width, uint height)
		{
			const string filename = "BitmapTest.bmp";

			BitmapFile bitmapFile = BitmapFile.CreateDefaultIndexed(bitCount, width, height);
			BitmapFile bitmapFile2;

			bitmapFile.Serialize(filename);
			bitmapFile2 = BitmapFile.ReadIndexed(filename);

			Assert.AreEqual(bitmapFile, bitmapFile2);

			File.Delete(filename);
		}

		[TestMethod]
		public void RoundTripWriteAndRead()
		{
			/* Test cases:
				 * Width below pitch
				 * Greater than 1 height
				 * Width equal to pitch
				 * Width above pitch
			*/
			{
				//SCOPED_TRACE("Monochrome");
				RoundTripSub(1, 1, 1);
				RoundTripSub(1, 1, 2);
				RoundTripSub(1, 32, 1);
				RoundTripSub(1, 33, 1);
			}
			{
				//SCOPED_TRACE("4 Bit");
				RoundTripSub(4, 1, 1);
				RoundTripSub(4, 1, 2);
				RoundTripSub(4, 8, 1);
				RoundTripSub(4, 9, 1);
			}
			{
				//SCOPED_TRACE("8 Bit");
				RoundTripSub(8, 1, 1);
				RoundTripSub(8, 1, 2);
				RoundTripSub(8, 2, 1);
			}
		}

		[TestMethod]
		public void VerifyIndexedPaletteSizeDoesNotExceedBitCount()
		{
			BitmapFile.VerifyIndexedPaletteSizeDoesNotExceedBitCount(1, 1);
			BitmapFile.VerifyIndexedPaletteSizeDoesNotExceedBitCount(1, 2);
			Assert.ThrowsException<Exception>(() => BitmapFile.VerifyIndexedPaletteSizeDoesNotExceedBitCount(1, 3));

			// Test non-static version of function
			BitmapFile bitmapFile = BitmapFile.CreateDefaultIndexed(1, 1, 1);
			bitmapFile.VerifyIndexedPaletteSizeDoesNotExceedBitCount();
			bitmapFile.palette = new Color[3];
			Assert.ThrowsException<Exception>(() => bitmapFile.VerifyIndexedPaletteSizeDoesNotExceedBitCount());
		}

		[TestMethod]
		public void VerifyPixelSizeMatchesImageDimensionsWithPitch()
		{
			BitmapFile.VerifyPixelSizeMatchesImageDimensionsWithPitch(1, 1, 1, 4);
			Assert.ThrowsException<Exception>(() => BitmapFile.VerifyPixelSizeMatchesImageDimensionsWithPitch(1, 1, 1, 1));

			// Test non-static version of function
			BitmapFile bitmapFile = BitmapFile.CreateDefaultIndexed(1, 1, 1);;
			bitmapFile.VerifyPixelSizeMatchesImageDimensionsWithPitch();
			bitmapFile.pixels = new byte[1];
			Assert.ThrowsException<Exception>(() => bitmapFile.VerifyPixelSizeMatchesImageDimensionsWithPitch());
		}

		[TestMethod]
		public void Equality()
		{
			BitmapFile bitmapFile1 = BitmapFile.CreateDefaultIndexed(1, 1, 1);
			BitmapFile bitmapFile2 = BitmapFile.CreateDefaultIndexed(1, 1, 1);

			Assert.IsTrue(bitmapFile1 == bitmapFile2);
			Assert.IsFalse(bitmapFile1 != bitmapFile2);

			bitmapFile1.pixels[0] = 1;
			Assert.IsFalse(bitmapFile1 == bitmapFile2);
			Assert.IsTrue(bitmapFile1 != bitmapFile2);
		}
	}
}
