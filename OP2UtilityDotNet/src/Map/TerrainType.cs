
using System.IO;

namespace OP2UtilityDotNet.OP2Map
{
	public struct Range16
	{
		public ushort start;
		public ushort end;
	}

	//static_assert(4 == sizeof(Range16), "Range16 is an unexpected size");

	// The properties associated with a range of tiles.
	public class TerrainType
	{
		public const int SizeInBytes = 264;

		// Range of tile mappings the TerrainType applies to.
		public Range16 tileMappingRange;

		// The first tile mapping index that represents bulldozed versions of the terrain type.
		// The rest of the bulldozed tiles appear consecutively after the first index.
		public ushort bulldozedTileMappingIndex;

		// The first tile mapping index of rubble for the Terrain Type range.
		// Common rubble will be the first 4 consecutive tile mappings after index.
		// Rare rubble will be the next 4 consecutive tile mappings.
		public ushort rubbleTileMappingIndex;

		// Mapping index of tube tiles. UNKNOWN why the data is repeated.
		public ushort[] tubeTileMappings = new ushort[6];

		// 5 groups of 16 tiles. Each group represents a different wall type.
		// Lava, Microbe, Full Strength Regular, Damaged Regular, and Heavily Damaged Regular.
		public ushort[,] wallTileMappingIndexes = new ushort[5,16];

		// First mapping index for lava tiles in Terrain Type.
		public ushort lavaTileMappingIndex;

		// UNKNOWN
		public ushort flat1;

		// UNKNOWN
		public ushort flat2;

		// UNKNOWN
		public ushort flat3;

		// Tube tile mapping indexes associated with Terrain Type.
		public ushort[] tubeTileMappingIndexes = new ushort[16];

		// Index for scorched tile within Terrain Type
		// Scorch comes from meteor impact or vehicle destruction. Not all tiles may be scorched.
		public ushort scorchedTileMappingIndex;

		public Range16[] scorchedRange = new Range16[3];

		// UNKNOWN
		public short[] unknown = new short[15];

		public TerrainType() { }

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(tileMappingRange.start);
			writer.Write(tileMappingRange.end);
			writer.Write(bulldozedTileMappingIndex);
			writer.Write(rubbleTileMappingIndex);

			for (int i=0; i < tubeTileMappings.Length; ++i)
				writer.Write(tubeTileMappings[i]);

			for (int i=0; i < wallTileMappingIndexes.GetLength(0); ++i)
			{
				for (int j=0; j < wallTileMappingIndexes.GetLength(1); ++j)
				{
					writer.Write(wallTileMappingIndexes[i,j]);
				}
			}

			writer.Write(lavaTileMappingIndex);
			writer.Write(flat1);
			writer.Write(flat2);
			writer.Write(flat3);

			for (int i=0; i < tubeTileMappingIndexes.Length; ++i)
				writer.Write(tubeTileMappingIndexes[i]);

			writer.Write(scorchedTileMappingIndex);

			for (int i=0; i < scorchedRange.Length; ++i)
			{
				writer.Write(scorchedRange[i].start);
				writer.Write(scorchedRange[i].end);
			}

			for (int i=0; i < unknown.Length; ++i)
				writer.Write(unknown[i]);
		}

		public TerrainType(BinaryReader reader)
		{
			tileMappingRange.start = reader.ReadUInt16();
			tileMappingRange.end = reader.ReadUInt16();
			bulldozedTileMappingIndex = reader.ReadUInt16();
			rubbleTileMappingIndex = reader.ReadUInt16();

			for (int i=0; i < tubeTileMappings.Length; ++i)
				tubeTileMappings[i] = reader.ReadUInt16();

			for (int i=0; i < wallTileMappingIndexes.GetLength(0); ++i)
			{
				for (int j=0; j < wallTileMappingIndexes.GetLength(1); ++j)
				{
					wallTileMappingIndexes[i,j] = reader.ReadUInt16();
				}
			}

			lavaTileMappingIndex = reader.ReadUInt16();
			flat1 = reader.ReadUInt16();
			flat2 = reader.ReadUInt16();
			flat3 = reader.ReadUInt16();

			for (int i=0; i < tubeTileMappingIndexes.Length; ++i)
				tubeTileMappingIndexes[i] = reader.ReadUInt16();

			scorchedTileMappingIndex = reader.ReadUInt16();

			for (int i=0; i < scorchedRange.Length; ++i)
			{
				scorchedRange[i].start = reader.ReadUInt16();
				scorchedRange[i].end = reader.ReadUInt16();
			}

			for (int i=0; i < unknown.Length; ++i)
				unknown[i] = reader.ReadInt16();
		}
	}

	//static_assert(264 == sizeof(TerrainType), "TerrainType is an unexpected size");
}
