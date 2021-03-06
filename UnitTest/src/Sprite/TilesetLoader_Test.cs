using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet;
using OP2UtilityDotNet.Bitmap;
using OP2UtilityDotNet.Sprite;

namespace UnitTest.src.Sprite
{
	[TestClass]
	public class TilesetLoader_Test
	{
		[TestMethod]
		public void PeekIsCustomTileset()
		{
			// Ensure bool return is correct and that peek does not affect stream position

			MemoryStream reader1 = new MemoryStream(TilesetLoader.TagFileSignature.GetBytes());
			Assert.IsTrue(TilesetLoader.PeekIsCustomTileset(reader1));
			Assert.AreEqual(0u, reader1.Position);

			Tag wrongFileSignature = new Tag("TEST");
			MemoryStream reader2 = new MemoryStream(wrongFileSignature.GetBytes());
			Assert.IsFalse(TilesetLoader.PeekIsCustomTileset(reader2));
			Assert.AreEqual(0u, reader2.Position);
		}

		[TestMethod]
		public void WriteCustomTileset()
		{
			MemoryStream writer = new MemoryStream();

			BitmapFile tileset1 = new BitmapFile(8, 32, -32, new Color[] { DiscreteColor.Red });
			TilesetLoader.WriteCustomTileset(writer, tileset1);
			writer.Position = 0;

			// Read just written tileset to ensure it was well formed
			BitmapFile tileset2 = TilesetLoader.ReadTileset(writer);

			Assert.AreEqual(tileset1, tileset2);
		}

		[TestMethod]
		public void WriteCustomTilesetError()
		{
			MemoryStream writer = new MemoryStream();

			// Use incorrect pixel width - Ensure error is thrown
			BitmapFile tileset = new BitmapFile(8, 20, 32, new Color[] { DiscreteColor.Red });
			Assert.ThrowsException<Exception>(() => TilesetLoader.WriteCustomTileset(writer, tileset));
		}

		[TestMethod]
		public void ReadTileset()
		{	
			// Well formed standard bitmap
			BitmapFile tileset = new BitmapFile(8, 32, 32, new Color[] { DiscreteColor.Red });
			MemoryStream writer = new MemoryStream();
			tileset.Serialize(writer);

			writer.Position = 0;
			BitmapFile newTileset = TilesetLoader.ReadTileset(writer);

			Assert.AreEqual(tileset, newTileset);

			// Well formed standard bitmap - Wrong width for a tileset
			tileset = new BitmapFile(8, 20, 32, new Color[] { DiscreteColor.Red });
			tileset.Serialize(writer);
			Assert.ThrowsException<Exception>(() => TilesetLoader.ReadTileset(writer));
		}

		[TestMethod]
		public void ValidateTileset()
		{
			TilesetLoader.ValidateTileset(new BitmapFile(8, 32, 32));

			// Improper bit depth
			Assert.ThrowsException<Exception>(() => TilesetLoader.ValidateTileset(new BitmapFile(1, 32, 32)));

			// Improper width
			Assert.ThrowsException<Exception>(() => TilesetLoader.ValidateTileset(new BitmapFile(1, 64, 32)));

			// Improper Height
			Assert.ThrowsException<Exception>(() => TilesetLoader.ValidateTileset(new BitmapFile(1, 32, 70)));
		}
	}
}
