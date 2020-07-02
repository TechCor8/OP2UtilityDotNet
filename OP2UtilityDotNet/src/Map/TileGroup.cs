using System.Collections.Generic;

namespace OP2UtilityDotNet.OP2Map
{
	// A set of tiles that group together to form a feature in Outpost 2, such as a large rock or cliff.
	public class TileGroup
	{
		public string name;

		// Width of tile group in tiles
		public uint tileWidth;

		// Height of tile group in tiles
		public uint tileHeight;

		// Tiles used in TileGroup listed in 1D (all of row 0 tiles first, then row 1, etc).
		public List<uint> mappingIndices = new List<uint>();
	}
}
