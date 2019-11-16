using System.Runtime.InteropServices;

namespace OP2UtilityDotNet
{
	// CellTypes returned and set by the GameMap class
	public enum CellType
	{
		FastPassible1 = 0,	// Rock vegetation
		Impassible2,		// Meteor craters, cracks/crevases
		SlowPassible1,		// Lava rock (dark)
		SlowPassible2,		// Rippled dirt/Lava rock bumps
		MediumPassible1,	// Dirt
		MediumPassible2,	// Lava rock
		Impassible1,		// Dirt/Rock/Lava rock mound/ice/volcano
		FastPassible2,		// Rock
		NorthCliffs,
		CliffsHighSide,
		CliffsLowSide,
		VentsAndFumaroles,	// Fumaroles (passable by GeoCons)
		zPad12,
		zPad13,
		zPad14,
		zPad15,
		zPad16,
		zPad17,
		zPad18,
		zPad19,
		zPad20,
		DozedArea,
		Rubble,
		NormalWall,
		MicrobeWall,
		LavaWall,
		Tube0,				// Used for tubes and areas under buildings
		Tube1,				// Note: Tube values 1-5 don't appear to be used
		Tube2,
		Tube3,
		Tube4,
		Tube5,
	}

	// Outpost 2 Tile metadata. Implemented as a Bitfield structure (32 bits total)
	public class Tile
	{
		public int _tile;

		private int GetBits(int offset, int count)
		{
			return (_tile >> (32-(offset+count))) & ((1 << count)-1);
		}

		private void SetBits(int offset, int count, int value)
		{
			int mask = ((1 << count)-1) << (32-(offset+count));
			_tile &= ~mask;
			_tile |= value << (32-(offset+count));
		}

		public Tile(int tile)
		{
			_tile = tile;
		}

		// Determines movement speed of tile, or if tile is bulldozed, walled, tubed, or has rubble.
		public CellType cellType			{ get { return (CellType)GetBits(0,5);			} set { SetBits(0,5, (int)value);			}	}

		// TileMapping lists graphics and animation properties associated with a tile.
		public uint tileMappingIndex		{ get { return (uint)GetBits(5,11);				} set { SetBits(5,11, (int)value);			}	}

		// Index of the unit that occupies the tile.
		public int unitIndex				{ get { return GetBits(16,11);					} set { SetBits(16,11, value);				}	}
		
		// True if lava is present on tile.
		public bool lava					{ get { return GetBits(27,1) != 0;				} set { SetBits(27,1, value ? 1 : 0);		}	}

		// True if it is possible for lava to enter the tile.
		public bool lavaPossible			{ get { return GetBits(28,1) != 0;				} set { SetBits(28,1, value ? 1 : 0);		}	}

		// Used in controlling Lava and Microbe spread. exact workings UNKNOWN.
		public bool expansion				{ get { return GetBits(29,1) != 0;				} set { SetBits(29,1, value ? 1 : 0);		}	}

		// True if the Blight is present on tile.
		public bool microbe					{ get { return GetBits(30,1) != 0;				} set { SetBits(30,1, value ? 1 : 0);		}	}
		
		// True if a wall or a building has been built on the tile.
		public bool wallOrBuilding			{ get { return GetBits(31,1) != 0;				} set { SetBits(31,1, value ? 1 : 0);		}	}
	}

	// Metadata that applies to all tiles on a map with the same Tileset and TileIndex.
	[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi, Pack=1)]
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
	}

	[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi, Pack=1)]
	public struct Range16
	{
		ushort start;
		ushort end;
	}

	// The properties associated with a range of tiles.
	[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi, Pack=1)]
	public struct TerrainType
	{
		// Range of tile mappings the TerrainType applies to.
		Range16 tileMappingRange;

		// The first tile mapping index that represents bulldozed versions of the terrain type.
		// The rest of the bulldozed tiles appear consecutively after the first index.
		ushort bulldozedTileMappingIndex;

		// The first tile mapping index of rubble for the Terrain Type range.
		// Common rubble will be the first 4 consecutive tile mappings after index.
		// Rare rubble will be the next 4 consecutive tile mappings.
		ushort rubbleTileMappingIndex;

		// Mapping index of tube tiles. UNKNOWN why the data is repeated.
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		ushort[] tubeTileMappings;

		// 5 groups of 16 tiles. Each group represents a different wall type.
		// Lava, Microbe, Full Strength Regular, Damaged Regular, and Heavily Damaged Regular.
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5*16)]
		ushort[] wallTileMappingIndexes;

		// First mapping index for lava tiles in Terrain Type.
		ushort lavaTileMappingIndex;

		// UNKNOWN
		ushort flat1;

		// UNKNOWN
		ushort flat2;

		// UNKNOWN
		ushort flat3;

		// Tube tile mapping indexes associated with Terrain Type.
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		ushort[] tubeTileMappingIndexes;

		// Index for scorched tile within Terrain Type
		// Scorch comes from meteor impact or vehicle destruction. Not all tiles may be scorched.
		ushort scorchedTileMappingIndex;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		Range16[] scorchedRange;

		// UNKNOWN
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
		ushort[] unknown;
	}
}
