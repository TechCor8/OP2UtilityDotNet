using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.Bitmap;

namespace UnitTest.src.Bitmap
{
	[TestClass]
	public class BitmapFile_Test
	{
		void RoundTripSub(ushort bitCount, uint width, int height)
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
		public void PeekIsBitmapFile()
		{
			{
				MemoryStream writer = new MemoryStream();
				new BitmapFile(1, 1, 1).Serialize(writer);
				writer.Position = 0;
				Assert.IsTrue(BitmapFile.PeekIsBitmap(writer));
			}
			{
				MemoryStream ms = new MemoryStream();
				BinaryWriter writer = new BinaryWriter(ms);
				writer.Write("test");
				Assert.IsFalse(BitmapFile.PeekIsBitmap(ms));
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
		public void CreateWithPalette()
		{
			BitmapFile bitmapFile = new BitmapFile(8, 2, 2, new Color[] { DiscreteColor.Green });

			Assert.AreEqual(DiscreteColor.Green, bitmapFile.palette[0]);

			// Ensure all pixels are set to palette index 0 (so they register as the initial color)
			foreach (byte pixel in bitmapFile.pixels)
			{
				Assert.AreEqual(0u, pixel);
			}

			// Proivde palette with more indices than bit count supports
			Color[] palette = new Color[] { DiscreteColor.Green, DiscreteColor.Red, DiscreteColor.Blue };
			Assert.ThrowsException<Exception>(() => new BitmapFile(1, 2, 2, palette));
		}

		[TestMethod]
		public void TestScanLineOrientation()
		{
			{ // Test Negative Height
				BitmapFile bitmap = new BitmapFile(1, 32, -32);
				Assert.AreEqual(ScanLineOrientation.TopDown, bitmap.GetScanLineOrientation());
			}
			{ // Test Positive Height
				BitmapFile bitmap = new BitmapFile(1, 32, 32);
				Assert.AreEqual(ScanLineOrientation.BottomUp, bitmap.GetScanLineOrientation());
			}
		}

		[TestMethod]
		public void AbsoluteHeight()
		{
			{ // Test Positive Height
				BitmapFile bitmap = new BitmapFile(1, 32, 32);
				Assert.AreEqual(32, bitmap.AbsoluteHeight());
			}
			{ // Test Negative Height
				BitmapFile bitmap = new BitmapFile(1, 32, -32);
				Assert.AreEqual(32, bitmap.AbsoluteHeight());
			}
		}

		[TestMethod]
		public void SwapRedAndBlue()
		{
			BitmapFile bitmapFile = new BitmapFile(8, 2, 2, new Color[] { DiscreteColor.Red });

			bitmapFile.SwapRedAndBlue();
			Assert.AreEqual(DiscreteColor.Blue, bitmapFile.palette[0]);
		}

		[TestMethod]
		public void InvertScanLines()
		{
			byte[] pixels = new byte[]
			{
				1, 1, 1, 1, 1, 1, 1, 1,
				0, 0, 0, 0, 0, 0, 0, 0
			};

			byte[] invertedPixels = new byte[]
			{
				0, 0, 0, 0, 0, 0, 0, 0,
				1, 1, 1, 1, 1, 1, 1, 1
			};

			BitmapFile bitmap = new BitmapFile(8, 8, 2, new Color[] { /* Empty Palette */ }, pixels);
			bitmap.InvertScanLines();

			Assert.AreEqual(ScanLineOrientation.TopDown, bitmap.GetScanLineOrientation());
			CollectionAssert.AreEqual(invertedPixels, bitmap.pixels);

			bitmap.InvertScanLines();

			Assert.AreEqual(ScanLineOrientation.BottomUp, bitmap.GetScanLineOrientation());
			CollectionAssert.AreEqual(pixels, bitmap.pixels);
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
