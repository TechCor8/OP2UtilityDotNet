using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.Bitmap;
using OP2UtilityDotNet.Sprite;

namespace UnitTest.src.Sprite
{
	[TestClass]
	public class ArtWriter_Test
	{
		[TestMethod]
		public void Empty()
		{
			// Write to File
			string testFilename = "test.prt";
			new ArtFile().Write(testFilename);
			File.Delete(testFilename);

			// Write to Memory
			using (MemoryStream stream = new MemoryStream())
			{
				new ArtFile().Write(stream);
			}
		}

		[TestMethod]
		public void BlankFilename()
		{
			Assert.ThrowsException<ArgumentException>(() => new ArtFile().Write(""));
		}

		private ArtFile GetTestArtFile()
		{
			ArtFile artFile = new ArtFile();
			artFile.palettes.Add(new Palette());

			ImageMeta imageMeta = new ImageMeta();
			imageMeta.width = 10;
			imageMeta.scanLineByteWidth = 12;
			imageMeta.paletteIndex = 0;
			artFile.imageMetas.Add(imageMeta);

			return artFile;
		}

		[TestMethod]
		public void Write_ScanLineByteWidth()
		{
			ArtFile artFile = GetTestArtFile();

			// Check no throw if scanLine next 4 byte aligned
			using (MemoryStream stream = new MemoryStream())
			{
				artFile.Write(stream);
			}

			// Check throw if scanLine > width && < 4 byte aligned
			using (MemoryStream stream = new MemoryStream())
			{
				artFile.imageMetas[0].scanLineByteWidth = 11;
				Assert.ThrowsException<Exception>(() => artFile.Write(stream));
			}

			// Check throw if scanLine > first 4 byte align
			using (MemoryStream stream = new MemoryStream())
			{
				artFile.imageMetas[0].scanLineByteWidth = 16;
				Assert.ThrowsException<Exception>(() => artFile.Write(stream));
			}

			// Check throw if scanLine < width but still 4 byte aligned
			using (MemoryStream stream = new MemoryStream())
			{
				artFile.imageMetas[0].scanLineByteWidth = 8;
				Assert.ThrowsException<Exception>(() => artFile.Write(stream));
			}
		}

		[TestMethod]
		public void Write_PaletteIndexRange()
		{
			ArtFile artFile = GetTestArtFile();

			// Check for no throw when ImageMeta.paletteIndex is within palette container's range
			using (MemoryStream stream = new MemoryStream())
			{
				artFile.Write(stream);
			}

			artFile.palettes.Clear();

			// Check for throw due to ImageMeta.paletteIndex outside of palette container's range
			using (MemoryStream stream = new MemoryStream())
			{
				Assert.ThrowsException<Exception>(() => artFile.Write(stream));
			}
		}

		[TestMethod]
		public void Write_PaletteColors()
		{
			ArtFile artFile = GetTestArtFile();

			byte red = 255;
			byte blue = 0;
			artFile.palettes[0].colors[0] = new Color( red, 0, blue, 0 );

			using (MemoryStream stream = new MemoryStream())
			{
				artFile.Write(stream);

				// Check ArtFile palette remains unchanged after write
				Assert.AreEqual(red, artFile.palettes[0].colors[0].red);
				Assert.AreEqual(blue, artFile.palettes[0].colors[0].blue);

				// Check ArtFile palette written to disk properly
				stream.Seek(0, SeekOrigin.Begin);
				artFile = ArtFile.Read(stream);
				Assert.AreEqual(red, artFile.palettes[0].colors[0].red);
				Assert.AreEqual(blue, artFile.palettes[0].colors[0].blue);
			}
		}
	}
}
