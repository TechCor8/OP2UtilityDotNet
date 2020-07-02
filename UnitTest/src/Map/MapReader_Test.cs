using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.OP2Map;

namespace UnitTest.src.OP2Map
{
	[TestClass]
	public class MapReader_Test
	{
		[TestMethod]
		public void MissingFile()
		{
			Assert.ThrowsException<FileNotFoundException>(() => Map.ReadMap("MissingFile.map"));
			Assert.ThrowsException<FileNotFoundException>(() => Map.ReadSavedGame("MissingFile.op2"));

			// Check if filename is an empty string
			Assert.ThrowsException<ArgumentException>(() => Map.ReadMap(""));
			Assert.ThrowsException<ArgumentException>(() => Map.ReadSavedGame(""));
		}

		[TestMethod]
		public void EmptyFile()
		{
			Assert.ThrowsException<EndOfStreamException>(() => Map.ReadMap("src/Map/data/EmptyMap.map"));
			Assert.ThrowsException<EndOfStreamException>(() => Map.ReadSavedGame("src/Map/data/EmptySave.OP2"));
		}

		[TestMethod]
		public void ReadMap_Binary()
		{
			// Write as Binary Writer
			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				new Map().Write(writer);

				// Read from stream as Binary Reader
				writer.BaseStream.Seek(0, SeekOrigin.Begin);
				using (BinaryReader reader = new BinaryReader(writer.BaseStream))
				{
					Map mapFile = Map.ReadMap(reader);

					// Throw error if attempting to read a saved game from a map
					writer.BaseStream.Seek(0, SeekOrigin.Begin);
					Assert.ThrowsException<EndOfStreamException>(() => Map.ReadSavedGame(reader));
				}
			}
		}

		[TestMethod]
		public void ReadMap_Stream()
		{
			// Write as Memory Stream
			using (MemoryStream stream = new MemoryStream())
			{
				new Map().Write(stream);

				// Read as Memory Stream
				stream.Seek(0, SeekOrigin.Begin);
				Map mapFile = Map.ReadMap(stream);

				// Throw error if attempting to read a saved game from a map
				stream.Seek(0, SeekOrigin.Begin);
				Assert.ThrowsException<EndOfStreamException>(() => Map.ReadSavedGame(stream));
			}
		}
	}
}
