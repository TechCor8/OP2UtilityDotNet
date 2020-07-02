
using System.IO;

namespace OP2UtilityDotNet.OP2Map
{
	// Metadata that applies to all tiles on a map with the same Tileset and TileIndex.
	public struct TileMapping
	{
		// The tile set the tile comes from.
		public ushort tilesetIndex;

		// The tile index within the tile set.
		public ushort tileGraphicIndex;

		// The number of tiles following this index that may be used to represent the tile
		// for an animation sequence.
		public ushort animationCount;

		// The number of cycles elapsed before cycling to the next tile in an animation.
		public ushort animationDelay;


		public void Serialize(BinaryWriter writer)
		{
			writer.Write(tilesetIndex);
			writer.Write(tileGraphicIndex);
			writer.Write(animationCount);
			writer.Write(animationDelay);
		}

		public TileMapping(BinaryReader reader)
		{
			tilesetIndex = reader.ReadUInt16();
			tileGraphicIndex = reader.ReadUInt16();
			animationCount = reader.ReadUInt16();
			animationDelay = reader.ReadUInt16();
		}
	}

	//static_assert(8 == sizeof(TileMapping), "TileMapping is an unexpected size");
}
