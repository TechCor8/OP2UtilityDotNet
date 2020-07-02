using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.OP2Map;

namespace UnitTest.src.OP2Map
{
	[TestClass]
	public class Map_Test
	{
		[TestMethod]
		public void TrimTilesetSources()
		{
			Map map = new Map();

			map.tilesetSources = new List<TilesetSource>(new TilesetSource[]
			{
				new TilesetSource("well0000.bmp", 1),
				new TilesetSource("somename.bmp", 0),
				new TilesetSource("", 1),
				new TilesetSource("well0001.bmp", 200)
			});
			map.TrimTilesetSources();

			CollectionAssert.AreEqual(
				new TilesetSource[]
				{
					new TilesetSource ("well0000.bmp", 1),
					new TilesetSource ("well0001.bmp", 200)
				},
				map.tilesetSources
			);
		}

		[TestMethod]
		public void CheckCellType()
		{
			Map map = new Map();
			map.tiles.Add(new Tile());

			map.SetCellType(CellType.FastPassible2, 0, 0);
			Assert.AreEqual(CellType.FastPassible2, map.GetCellType(0, 0));

			// Pass a non-defined cell type and receive an error
			Assert.ThrowsException<Exception>(() => map.SetCellType((CellType)9999, 0, 0));
			// Ensure no change in original cell type after exception raised
			Assert.AreEqual(CellType.FastPassible2, map.GetCellType(0, 0));
		}

		[TestMethod]
		public void CheckLavaPossible()
		{
			Map map = new Map();
			map.tiles.Add(new Tile());

			map.SetLavaPossible(true, 0, 0);
			Assert.IsTrue(map.GetLavaPossible(0, 0));

			map.SetLavaPossible(false, 0, 0);
			Assert.IsFalse(map.GetLavaPossible(0, 0));
		}
	}
}
