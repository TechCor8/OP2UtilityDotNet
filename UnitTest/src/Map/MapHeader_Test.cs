using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.OP2Map;

namespace UnitTest.src.OP2Map
{
	[TestClass]
	public class MapHeader_Test
	{
		[TestMethod]
		public void VersionTagValid()
		{
			MapHeader mapHeader = new MapHeader();

			Assert.IsTrue(mapHeader.VersionTagValid());

			mapHeader.versionTag = MapHeader.MinMapVersion + 1;
			Assert.IsTrue(mapHeader.VersionTagValid());

			mapHeader.versionTag = MapHeader.MinMapVersion - 1;
			Assert.IsFalse(mapHeader.VersionTagValid());
		}

		[TestMethod]
		public void WidthInTiles()
		{
			MapHeader mapHeader = new MapHeader();
			mapHeader.lgWidthInTiles = 5;
			mapHeader.heightInTiles = 32;

			Assert.AreEqual(32u, mapHeader.WidthInTiles());
		}

		[TestMethod]
		public void TileCount()
		{
			MapHeader mapHeader = new MapHeader();
			mapHeader.lgWidthInTiles = 5;
			mapHeader.heightInTiles = 32;

			Assert.AreEqual(32u * 32u, mapHeader.TileCount());
		}
	}
}
