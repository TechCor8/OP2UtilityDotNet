
namespace OP2UtilityDotNet.OP2Map
{
	// Facilitates finding the source BMP file (well00XX.bmp) for a tile set.
	// Tile set names must be exactly 8 chars long not including file extension.
	public class TilesetSource
	{
		// Fixed length string. Map and save files require tile set filenames to be exactly 8
		// characters long. User must handle terminating the string.
		public string tilesetFilename;

		// Number of Tiles in set (represented on BMP).
		public uint numTiles;


		public TilesetSource() { }

		public TilesetSource(string tilesetFilename, uint numTiles)
		{
			this.tilesetFilename = tilesetFilename;
			this.numTiles = numTiles;
		}

		public override bool Equals(object obj)
		{
			TilesetSource rhs = obj as TilesetSource;

			return this == rhs;
		}

		public override int GetHashCode()
		{
			return numTiles.GetHashCode() + tilesetFilename.GetHashCode();
		}

		public static bool operator ==(TilesetSource lhs, TilesetSource rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;

			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;

			return (lhs.numTiles == rhs.numTiles) && (lhs.tilesetFilename == rhs.tilesetFilename);
		}
		public static bool operator !=(TilesetSource lhs, TilesetSource rhs)
		{
			return !(lhs == rhs);
		}

		public bool IsEmpty()
		{
			return (numTiles == 0) || string.IsNullOrEmpty(tilesetFilename);
		}
	}
}
