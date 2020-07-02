﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.Bitmap;

namespace UnitTest.src.Bitmap
{
	[TestClass]
	public class IndexBmpWriter_Test
	{
		[TestMethod]
		public void InvalidBitCountThrows()
		{
			Color[] palette = new Color[8];
			byte[] pixels = new byte[4];
			string filename = "MonochromeTest.bmp";

			Assert.ThrowsException<Exception>(() => BitmapFile.WriteIndexed(filename, 3, 1, 1, palette, pixels));
			Assert.ThrowsException<Exception>(() => BitmapFile.WriteIndexed(filename, 32, 1, 1, palette, pixels));

			File.Delete(filename);
		}

		[TestMethod]
		public void TooManyPaletteEntriesThrows()
		{
			Color[] palette = new Color[3];
			byte[] pixels = new byte[4];
			string filename = "PaletteRangeTest.bmp";

			Assert.ThrowsException<Exception>(() => BitmapFile.WriteIndexed(filename, 1, 1, 1, palette, pixels));

			File.Delete(filename);
		}

		[TestMethod]
		public void WritePartiallyFilledPalette()
		{
			Color[] palette = new Color[1];
			byte[] pixels = new byte[4];
			string filename = "PaletteRangeTest.bmp";

			BitmapFile.WriteIndexed(filename, 1, 1, 1, palette, pixels);

			File.Delete(filename);
		}

		[TestMethod]
		public void IncorrectPixelPaddingThrows()
		{
			Color[] palette = new Color[2];
			string filename = "IncorrectPixelPaddingTest.bmp";

			byte[] pixels = new byte[3];
			Assert.ThrowsException<Exception>(() => BitmapFile.WriteIndexed(filename, 1, 1, 1, palette, pixels));

			pixels = new byte[5];
			Assert.ThrowsException<Exception>(() => BitmapFile.WriteIndexed(filename, 1, 1, 1, palette, pixels));

			File.Delete(filename);
		}
	}
}
