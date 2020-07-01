
namespace OP2UtilityDotNet.Map
{
	// Outpost 2 Tile metadata. Implemented as a Bitfield structure (32 bits total)
	public class Tile
	{
		public int backingField;

		// Determines movement speed of tile, or if tile is bulldozed, walled, tubed, or has rubble.
		public CellType cellType // : 5;
		{
			get
			{
				return (CellType)GetBitValue(0, 5);
			}
			set
			{
				SetBitValue(0, 5, (int)value);
			}
		}

		// TileMapping lists graphics and animation properties associated with a tile.
		public uint tileMappingIndex // : 11;
		{
			get
			{
				return (uint)GetBitValue(5, 11);
			}
			set
			{
				SetBitValue(5, 11, (int)value);
			}
		}

		// Index of the unit that occupies the tile.
		public int unitIndex // : 11;
		{
			get
			{
				return GetBitValue(16, 11);
			}
			set
			{
				SetBitValue(16, 11, value);
			}
		}

		// True if lava is present on tile.
		public int bLava // : 1;
		{
			get
			{
				return GetBitValue(27, 1);
			}
			set
			{
				SetBitValue(27, 1, value);
			}
		}

		// True if it is possible for lava to enter the tile.
		public int bLavaPossible // : 1;
		{
			get
			{
				return GetBitValue(28, 1);
			}
			set
			{
				SetBitValue(28, 1, value);
			}
		}

		// Used in controlling Lava and Microbe spread. exact workings UNKNOWN.
		public int bExpansion // : 1;
		{
			get
			{
				return GetBitValue(29, 1);
			}
			set
			{
				SetBitValue(29, 1, value);
			}
		}

		// True if the Blight is present on tile.
		public int bMicrobe // : 1;
		{
			get
			{
				return GetBitValue(30, 1);
			}
			set
			{
				SetBitValue(30, 1, value);
			}
		}

		// True if a wall or a building has been built on the tile.
		public int bWallOrBuilding // : 1;
		{
			get
			{
				return GetBitValue(31, 1);
			}
			set
			{
				SetBitValue(31, 1, value);
			}
		}

		private int GetBitValue(int offset, int length)
		{
			int result = backingField >> offset;// Remove the offset from the backing field
			result &= ~(-1 << length);			// Creates a mask with "length" number of bits set and clear the bits
			
			return result;
		}

		private void SetBitValue(int offset, int length, int value)
		{
			value = value << offset;			// Move the value to set to the correct offset

			int mask = ~(-1 << length);			// Creates a mask with "length" number of bits set
			mask = mask << offset;				// Move masked bits to the correct offset

			backingField &= ~mask;				// Clear the masked bits in the backing field
			backingField |= value & mask;		// Set the value in the masked bits
		}
	}
}
