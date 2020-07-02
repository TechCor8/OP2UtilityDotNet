using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.OP2Map;

namespace UnitTest.src.OP2Map
{
	[TestClass]
	public class MapTilesetSource_Test
	{
		[TestMethod]
		public void ComparisonOperators()
		{
			TilesetSource blue = new TilesetSource("well0000.bmp", 1);
			TilesetSource well0 = new TilesetSource("well0000.bmp", 1);
			TilesetSource well1 = new TilesetSource("well0001.bmp", 200);

			// Note: Internally EXPECT_EQ uses `operator ==`
			Assert.AreEqual(blue, well0);
			Assert.AreEqual(well0, well0);
			Assert.AreEqual(well1, well1);

			// Note: Internally EXPECT_NE uses `operator !=`
			Assert.AreNotEqual(well0, well1);
			Assert.AreNotEqual(well1, well0);
		}

		[TestMethod]
		public void IsEmpty()
		{
			Assert.IsTrue(new TilesetSource("", 0).IsEmpty());
			Assert.IsTrue(new TilesetSource("", 1).IsEmpty());
			Assert.IsTrue(new TilesetSource("filename.bmp", 0).IsEmpty());
			Assert.IsFalse(new TilesetSource("well0000.bmp", 1).IsEmpty());
		}
	}
}
