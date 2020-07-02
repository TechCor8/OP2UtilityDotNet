using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.Sprite;

namespace UnitTest.src.Sprite
{
	[TestClass]
	public class ArtReader_Test
	{
		[TestMethod]
		public void ReadMissingFile()
		{
			Assert.ThrowsException<FileNotFoundException>(() => ArtFile.Read("MissingFile.prt"));

			// Check if filename is an empty string
			Assert.ThrowsException<ArgumentException>(() => ArtFile.Read(""));
		}

		[TestMethod]
		public void ReadEmptyFile()
		{
			Assert.ThrowsException<EndOfStreamException>(() => ArtFile.Read("src/Sprite/data/Empty.prt"));
		}

		[TestMethod]
		public void ReadBinary()
		{
			// We want a simple valid source to load from, so we will create one by first writing it
			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				new ArtFile().Write(writer);

				// Read from stream as BinaryReader
				writer.BaseStream.Seek(0, SeekOrigin.Begin);
				using (BinaryReader reader = new BinaryReader(writer.BaseStream))
				{
					ArtFile artFile = ArtFile.Read(reader);
				}
			}
		}

		[TestMethod]
		public void ReadStream()
		{
			// We want a simple valid source to load from, so we will create one by first writing it
			using (MemoryStream stream = new MemoryStream())
			{
				new ArtFile().Write(stream);

				// Read from stream
				stream.Seek(0, SeekOrigin.Begin);
				ArtFile artFile = ArtFile.Read(stream);
			}
		}
	}
}
