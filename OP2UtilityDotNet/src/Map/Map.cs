using System;
using System.Collections.Generic;
using System.IO;

namespace OP2UtilityDotNet.Map
{
	// FILE FORMAT DOCUMENTATION:
	//     Outpost2SVN\OllyDbg\InternalData\FileFormat SavedGame and Map.txt.

	// ALT IMPLEMENTATION (with COM support)
	//     Outpost2SVN\MapEditor\OP2Editor.

	//An Outpost 2 map file.
	public class Map
	{
		private static readonly byte[] tilesetHeader = new byte[10] { (byte)'T', (byte)'I', (byte)'L', (byte)'E', (byte)' ', (byte)'S', (byte)'E', (byte)'T', (byte)'\x1a', (byte)'\0' };

		// 1D listing of all tiles on the associated map. See MapHeader data for height and width of map.
		public List<Tile> tiles = new List<Tile>();

		/**
		 * \brief	Represents the visible areas of the map.
		 *
		 * \note	Maps designated 'around the world' allow for continuous
		 *			scrolling on the X axis and so will populate X1 with -1
		 *			and X2 with \c INT_MAX.
		 */
		public Rect clipRect;

		// Listing of all tile set sources associated with the map.
		public List<TilesetSource> tilesetSources = new List<TilesetSource>();

		// Metadata about each available tile from the tile set sources.
		public List<TileMapping> tileMappings = new List<TileMapping>();

		// Listing of properties grouped by terrain type. Properties apply to a given range of tiles.
		public List<TerrainType> terrainTypes = new List<TerrainType>();

		public List<TileGroup> tileGroups = new List<TileGroup>();


		public Map()
		{
			versionTag = MapHeader.MinMapVersion;
			isSavedGame = false;
			widthInTiles = 0;
			heightInTiles = 0;
		}

		public static Map ReadMap(string filename)
		{
			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (BinaryReader mapStream = new BinaryReader(fs))
			{
				return ReadMap(mapStream);
			}
		}
		public static Map ReadMap(Stream mapStream)
		{
			if (mapStream == null)
			{
				throw new ArgumentNullException(nameof(mapStream));
			}

			using (BinaryReader reader = new BinaryReader(mapStream, System.Text.Encoding.ASCII, true))
			{
				return ReadMap(reader);
			}
		}
		public static Map ReadMap(BinaryReader mapStream)
		{
			Map map = ReadMapBeginning(mapStream);

			ReadVersionTag(mapStream, map.versionTag);
			ReadVersionTag(mapStream, map.versionTag);

			ReadTileGroups(mapStream, map);

			return map;
		}

		public static Map ReadSavedGame(string filename)
		{
			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (BinaryReader savedGameStream = new BinaryReader(fs))
			{
				return ReadSavedGame(savedGameStream);
			}
		}

		public static Map ReadSavedGame(BinaryReader savedGameStream)
		{
			SkipSaveGameHeader(savedGameStream);

			Map map = ReadMapBeginning(savedGameStream);
	
			ReadVersionTag(savedGameStream, map.versionTag);

			ReadSavedGameUnits(savedGameStream);
	
			ReadVersionTag(savedGameStream, map.versionTag);

			// TODO: Read data after final version tag.

			return map;
		}

		public void Write(string filename)
		{
			using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
			using (BinaryWriter mapStream = new BinaryWriter(fs))
			{
				Write(mapStream);
			}
		}

		public void Write(BinaryWriter mapStream)
		{
			Map map = this;
			MapHeader mapHeader = map.CreateHeader();

			mapHeader.Serialize(mapStream);
			for (int i=0; i < map.tiles.Count; ++i)
				mapStream.Write(map.tiles[i].backingField);
			map.clipRect.Serialize(mapStream);
			WriteTilesetSources(mapStream, map.tilesetSources);
			mapStream.Write(tilesetHeader);
			mapStream.Write((uint)map.tileMappings.Count);
			for (int i=0; i < map.tileMappings.Count; ++i)
				map.tileMappings[i].Serialize(mapStream);
			mapStream.Write((uint)map.terrainTypes.Count);
			for (int i=0; i < map.terrainTypes.Count; ++i)
				map.terrainTypes[i].Serialize(mapStream);

			mapStream.Write(mapHeader.versionTag);
			mapStream.Write(mapHeader.versionTag);

			WriteTileGroups(mapStream, map.tileGroups);
		}

		public void SetVersionTag(uint versionTag) { this.versionTag = versionTag; }
		public uint GetVersionTag() { return versionTag; }
		public bool IsSavedGame() { return isSavedGame; }
		public uint WidthInTiles() { return widthInTiles; }
		public uint HeightInTiles() { return heightInTiles; }

		// Total number of tiles on map.
		public int TileCount()
		{
			return tiles.Count;
		}

		public int GetTileMappingIndex(int x, int y)
		{
			return (int)tiles[GetTileIndex(x, y)].tileMappingIndex;
		}

		public void SetTileMappingIndex(int x, int y, uint mappingIndex)
		{
			tiles[GetTileIndex(x, y)].tileMappingIndex = mappingIndex;
		}

		public CellType GetCellType(int x, int y)
		{
			return tiles[GetTileIndex(x, y)].cellType;
		}

		public void SetCellType(CellType cellType, int x, int y)
		{
			// Tube5 has the largest CellType index
			if (cellType > CellType.Tube5) {
				throw new System.Exception("Improper cell type provided : CellType index = " + cellType);
			}

			tiles[GetTileIndex(x, y)].cellType = cellType;
		}

		public bool GetLavaPossible(int x, int y)
		{
			return tiles[GetTileIndex(x, y)].bLavaPossible != 0;
		}

		public void SetLavaPossible(bool lavaPossible, int x, int y)
		{
			tiles[GetTileIndex(x, y)].bLavaPossible = lavaPossible ? 1 : 0;
		}

		public int GetTilesetIndex(int x, int y)
		{
			return tileMappings[GetTileMappingIndex(x, y)].tilesetIndex;
		}

		public int GetImageIndex(int x, int y)
		{
			return tileMappings[GetTileMappingIndex(x, y)].tileGraphicIndex;
		}

		public static void CheckMinVersionTag(uint versionTag)
		{
			if (versionTag < MapHeader.MinMapVersion)
			{
				throw new System.Exception(
					"All instances of version tag in .map and .op2 files should be greater than " +
					MapHeader.MinMapVersion + ".\n" +
					"Found version tag is " + versionTag + "."
				);
			}
		}

		public void TrimTilesetSources()
		{
			tilesetSources.RemoveAll((source) => source.IsEmpty());
		}

		private uint versionTag;
		private bool isSavedGame;
		private uint widthInTiles;
		private uint heightInTiles;

		private int GetTileIndex(int x, int y)
		{
			int lowerX = x & 0x1F; // ... 0001 1111
			int upperX = x >> 5;   // ... 1110 0000
			return (int)((upperX * heightInTiles + y) * 32 + lowerX);
		}

		// Write
		private MapHeader CreateHeader()
		{
			MapHeader mapHeader = new MapHeader();

			mapHeader.versionTag = versionTag;
			mapHeader.bSavedGame = isSavedGame ? 1 : 0;
			mapHeader.lgWidthInTiles = GetWidthInTilesLog2(widthInTiles);
			mapHeader.heightInTiles = heightInTiles;

			//if (tilesetSources.Count > uint.MaxValue) {
			//	throw new System.Exception("Too many tilesets contained in map");
			//}

			mapHeader.tilesetCount = (uint)tilesetSources.Count;

			return mapHeader;
		}

		private uint GetWidthInTilesLog2(uint widthInTiles)
		{
			//if (!IsPowerOf2(widthInTiles)) {
			if (widthInTiles != 0 && !BitTwiddle.IsPowerOf2(widthInTiles)) {
				throw new System.Exception("Map width in tiles must be a power of 2");
			}
			return BitTwiddle.Log2OfPowerOf2(widthInTiles);
		}

		private static void WriteTilesetSources(BinaryWriter stream, List<TilesetSource> tilesetSources)
		{
			foreach (TilesetSource tilesetSource in tilesetSources)
			{
				stream.Write(tilesetSource.tilesetFilename.Length);
				stream.Write(System.Text.Encoding.ASCII.GetBytes(tilesetSource.tilesetFilename));

				// Only include the number of tiles if the tileset contains a filename.
				if (tilesetSource.tilesetFilename.Length > 0)
				{
					stream.Write(tilesetSource.numTiles);
				}
			}
		}
		private static void WriteTileGroups(BinaryWriter stream, List<TileGroup> tileGroups)
		{
			WriteContainerSize(stream, tileGroups.Count);
			// tileGroups.size is checked to ensure it is below UINT32_MAX by previous call to WriteContainerSize.
			// Write unknown field with best guess as to what value it should hold
			uint unknown = tileGroups.Count > 0 ? (uint)tileGroups.Count - 1 : 0;
			stream.Write(unknown);

			foreach (TileGroup tileGroup in tileGroups)
			{
				stream.Write(tileGroup.tileWidth);
				stream.Write(tileGroup.tileHeight);

				for (int i=0; i < tileGroup.mappingIndices.Count; ++i)
					stream.Write(tileGroup.mappingIndices[i]);

				stream.Write(tileGroup.name.Length);
				byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(tileGroup.name);
				stream.Write(nameBytes);
			}
		}

		// Outpost 2 map files represent container sizes as 4 byte values
		private static void WriteContainerSize(BinaryWriter stream, int size)
		{
			//if (size > uint.MaxValue) {
			//	throw new System.Exception("Container size is too large for writing into an Outpost 2 map");
			//}

			stream.Write((uint)size);
		}

		// Read
		private static Map ReadMapBeginning(BinaryReader stream)
		{
			MapHeader mapHeader = new MapHeader(stream);
			CheckMinVersionTag(mapHeader.versionTag);

			Map map = new Map();
			map.versionTag = mapHeader.versionTag;
			map.isSavedGame = mapHeader.bSavedGame != 0;
			map.widthInTiles = mapHeader.WidthInTiles();
			map.heightInTiles = mapHeader.heightInTiles;

			map.tiles = new List<Tile>((int)mapHeader.TileCount());
			for (int i=0; i < mapHeader.TileCount(); ++i)
			{
				Tile tile = new Tile();
				tile.backingField = stream.ReadInt32();
				map.tiles.Add(tile);
			}
			
			map.clipRect = new Rect(stream);
			ReadTilesetSources(stream, map, (int)mapHeader.tilesetCount);
			ReadTilesetHeader(stream);

			int tileMappingsCount = (int)stream.ReadUInt32();
			map.tileMappings = new List<TileMapping>(tileMappingsCount);
			for (int i=0; i < tileMappingsCount; ++i)
				map.tileMappings.Add(new TileMapping(stream));

			int terrainTypesCount = (int)stream.ReadUInt32();
			map.terrainTypes = new List<TerrainType>(terrainTypesCount);
			for (int i=0; i < terrainTypesCount; ++i)
				map.terrainTypes.Add(new TerrainType(stream));

			return map;
		}
		private static void SkipSaveGameHeader(BinaryReader stream)
		{
			stream.BaseStream.Seek(0x1E025, SeekOrigin.Current);
		}
		private static void ReadTilesetSources(BinaryReader stream, Map map, int tilesetCount)
		{
			map.tilesetSources = new List<TilesetSource>(tilesetCount);

			for (int i=0; i < tilesetCount; ++i)
			{
				TilesetSource tilesetSource = new TilesetSource();

				int tilesetFilenameLength = (int)stream.ReadUInt32();
				byte[] tilesetFilenameBytes = stream.ReadBytes(tilesetFilenameLength);
				tilesetSource.tilesetFilename = System.Text.Encoding.ASCII.GetString(tilesetFilenameBytes);

				if (tilesetSource.tilesetFilename.Length > 8) {
					throw new System.Exception("Tileset name may not be greater than 8 characters in length.");
				}

				if (tilesetSource.tilesetFilename.Length > 0) {
					tilesetSource.numTiles = stream.ReadUInt32();
				}

				map.tilesetSources.Add(tilesetSource);
			}
		}
		private static void ReadTilesetHeader(BinaryReader stream)
		{
			byte[] buffer = stream.ReadBytes(10);

			if (buffer.Length != tilesetHeader.Length)
			{
				throw new System.Exception("'TILE SET' string not found.");
			}

			for (int i = 0; i < buffer.Length; ++i)
			{
				if (buffer[i] != tilesetHeader[i])
				{
					throw new System.Exception("'TILE SET' string not found.");
				}
			}
		}
		private static void ReadVersionTag(BinaryReader stream, uint lastVersionTag)
		{
			uint nextVersionTag = stream.ReadUInt32();

			CheckMinVersionTag(nextVersionTag);

			if (nextVersionTag != lastVersionTag) {
				throw new System.Exception("Mismatched version tags detected. Version tag 1: " + 
					lastVersionTag + ". Version tag 2: " + nextVersionTag);
			}
		}
		private static void ReadSavedGameUnits(BinaryReader stream)
		{
			SavedGameUnits savedGameUnits = new SavedGameUnits();

			savedGameUnits.unitCount = stream.ReadUInt32();
			savedGameUnits.lastUsedUnitIndex = stream.ReadUInt32();
			savedGameUnits.nextFreeUnitSlotIndex = stream.ReadUInt32();
			savedGameUnits.firstFreeUnitSlotIndex = stream.ReadUInt32();
			savedGameUnits.sizeOfUnit = stream.ReadUInt32();
			
			savedGameUnits.CheckSizeOfUnit();

			savedGameUnits.objectCount1 = stream.ReadUInt32();
			savedGameUnits.objectCount2 = stream.ReadUInt32();
			
			savedGameUnits.objects1 = new List<ObjectType1>((int)savedGameUnits.objectCount1);
			for (int i=0; i < savedGameUnits.objectCount1; ++i)
				savedGameUnits.objects1.Add(new ObjectType1(stream));
			savedGameUnits.objects2 = new List<uint>((int)savedGameUnits.objectCount2);
			for (int i=0; i < savedGameUnits.objectCount2; ++i)
				savedGameUnits.objects2.Add(stream.ReadUInt32());
			
			savedGameUnits.nextUnitIndex = stream.ReadUInt32();
			savedGameUnits.prevUnitIndex = stream.ReadUInt32();
			
			for (int i=0; i < savedGameUnits.units.Length; ++i)
				savedGameUnits.units[i] = new UnitRecord(stream);
			
			if (savedGameUnits.firstFreeUnitSlotIndex != savedGameUnits.nextFreeUnitSlotIndex)
			{
				for (int i=0; i < savedGameUnits.freeUnits.Length; ++i)
					savedGameUnits.freeUnits[i] = stream.ReadUInt32();
			}
		}
		private static void ReadTileGroups(BinaryReader stream, Map map)
		{
			uint numTileGroups = stream.ReadUInt32();
			/*uint unknown = */stream.ReadUInt32(); // Read unknown/unused field (skip past it)

			for (uint i = 0; i < numTileGroups; ++i)
			{
				map.tileGroups.Add(ReadTileGroup(stream));
			}
		}
		private static TileGroup ReadTileGroup(BinaryReader stream)
		{
			TileGroup tileGroup = new TileGroup();

			tileGroup.tileWidth = stream.ReadUInt32();
			tileGroup.tileHeight = stream.ReadUInt32();

			int mappingIndicesCount = (int)(tileGroup.tileWidth * tileGroup.tileHeight);
			tileGroup.mappingIndices = new List<uint>(mappingIndicesCount);

			for (int i=0; i < mappingIndicesCount; ++i)
				tileGroup.mappingIndices.Add(stream.ReadUInt32());

			int nameLength = (int)stream.ReadUInt32();
			byte[] nameBytes = stream.ReadBytes(nameLength);
			tileGroup.name = System.Text.Encoding.ASCII.GetString(nameBytes);

			return tileGroup;
		}
	}
}
