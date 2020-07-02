using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.OP2Map;

namespace UnitTest.src.OP2Map
{
	[TestClass]
	public class MapWriter_Test
	{
		[TestMethod]
		public void EmptyMap()
		{
			// Write to File
			string testFilename = "test.map";
			new Map().Write(testFilename);
			File.Delete(testFilename);

			// Write to Memory
			using (MemoryStream stream = new MemoryStream())
			{
				new Map().Write(stream);
			}

			// Write to Binary Writer
			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				new Map().Write(writer);
			}
		}

		[TestMethod]
		public void BlankFilename()
		{
			Assert.ThrowsException<ArgumentException>(() => new Map().Write(""));
		}

		[TestMethod]
		public void AllowInvalidVersionTag()
		{
			using (MemoryStream stream = new MemoryStream())
			{
				Map map = new Map();

				map.SetVersionTag(MapHeader.MinMapVersion - 1);
				new Map().Write(stream);
			}
		}
	}
}
