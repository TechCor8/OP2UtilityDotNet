using System;
using System.Runtime.InteropServices;

namespace OP2UtilityDotNet
{
	public class Map : IDisposable
	{
		private IntPtr m_MapPtr;

		public Map()											{ m_MapPtr = Map_Create();								}
		private Map(IntPtr mapPtr)								{ m_MapPtr = mapPtr;									}
		public void Dispose()									{ Map_Release(m_MapPtr);								}

		public static Map ReadMap(string filename)				{ return new Map(Map_ReadMap(filename));				}
		public static Map ReadMap(byte[] mapData)
		{
			IntPtr buffer = Marshal.AllocHGlobal(mapData.Length);
			Marshal.Copy(mapData, 0, buffer, mapData.Length);

			IntPtr mapPtr = Map_ReadMapStream(buffer, (ulong)mapData.Length);

			Marshal.FreeHGlobal(buffer);

			return new Map(mapPtr);
		}

		public static Map ReadSavedGame(string filename)		{ return new Map(Map_ReadSavedGame(filename));			}
		public static Map ReadSavedGame(byte[] mapData)
		{
			IntPtr buffer = Marshal.AllocHGlobal(mapData.Length);
			Marshal.Copy(mapData, 0, buffer, mapData.Length);

			IntPtr mapPtr = Map_ReadSavedGameStream(buffer, (ulong)mapData.Length);

			Marshal.FreeHGlobal(buffer);

			return new Map(mapPtr);
		}

		// 1D listing of all tiles on the associated map. See MapHeader data for height and width of map.
		public ulong GetTileCount()						{ return Map_GetTileCount(m_MapPtr);											}
		public Tile GetTile(int index)					{ return new Tile(Map_GetTile(m_MapPtr, index));								}
		public void SetTile(int index, Tile tile)		{ Map_SetTile(m_MapPtr, index, tile._tile);										}
		public void AddTile(Tile tile)					{ Map_AddTile(m_MapPtr, tile._tile);											}
		public void RemoveTile(int index)				{ Map_RemoveTile(m_MapPtr, index);												}

		/**
		 * \brief	Represents the visible areas of the map.
		 *
		 * \note	Maps designated 'around the world' allow for continuous
		 *			scrolling on the X axis and so will populate X1 with -1
		 *			and X2 with \c INT_MAX.
		 */
		public int clipRectX1			{ get { return Map_GetClipRectX1(m_MapPtr);		} set { Map_SetClipRectX1(m_MapPtr, value);		}	}
		public int clipRectX2			{ get { return Map_GetClipRectX2(m_MapPtr);		} set { Map_SetClipRectX2(m_MapPtr, value);		}	}
		public int clipRectY1			{ get { return Map_GetClipRectY1(m_MapPtr);		} set { Map_SetClipRectY1(m_MapPtr, value);		}	}
		public int clipRectY2			{ get { return Map_GetClipRectY2(m_MapPtr);		} set { Map_SetClipRectY2(m_MapPtr, value);		}	}

		// Listing of all tile set sources associated with the map.
		public ulong GetTilesetSourceCount()											{ return Map_GetTilesetSourceCount(m_MapPtr);										}
		public string GetTilesetSourceFilename(int index)								{ return Marshalling.GetString(Map_GetTilesetSourceFilename(m_MapPtr, index));		}
		public uint GetTilesetSourceNumTiles(int index)									{ return Map_GetTilesetSourceNumTiles(m_MapPtr, index);								}
		public void SetTilesetSourceFilename(int index, string tilesetFilename)			{ Map_SetTilesetSourceFilename(m_MapPtr, index, tilesetFilename);					}
		public void SetTilesetSourceNumTiles(int index, int numTiles)					{ Map_SetTilesetSourceNumTiles(m_MapPtr, index, numTiles);							}
		public void AddTilesetSource(string tilesetFilename, int numTiles)				{ Map_AddTilesetSource(m_MapPtr, tilesetFilename, numTiles);						}
		public void RemoveTilesetSource(int index)										{ Map_RemoveTilesetSource(m_MapPtr, index);											}

		// Metadata about each available tile from the tile set sources.
		public ulong GetTileMappingCount()												{ return Map_GetTileMappingCount(m_MapPtr);											}
		public TileMapping GetTileMapping(int index)
		{
			ulong val = Map_GetTileMapping(m_MapPtr, index);
			TileMapping mapping = new TileMapping();
			mapping.tilesetIndex = (ushort)(val >> 48);
			mapping.tileGraphicIndex = (ushort)(val >> 32);
			mapping.animationCount = (ushort)(val >> 16);
			mapping.animationDelay = (ushort)val;

			return mapping;
		}
		public void SetTileMapping(int index, TileMapping mapping)
		{
			ulong val;
			val = (ulong)mapping.tilesetIndex << 48;
			val |= (ulong)mapping.tileGraphicIndex << 32;
			val |= (ulong)mapping.animationCount << 16;
			val |= mapping.animationDelay;

			Map_SetTileMapping(m_MapPtr, index, val);
		}
		public void AddTileMapping(TileMapping mapping)
		{
			ulong val;
			val = (ulong)mapping.tilesetIndex << 48;
			val |= (ulong)mapping.tileGraphicIndex << 32;
			val |= (ulong)mapping.animationCount << 16;
			val |= mapping.animationDelay;

			Map_AddTileMapping(m_MapPtr, val);
		}
		public void RemoveTileMapping(int index)										{ Map_RemoveTileMapping(m_MapPtr, index);								}

		// Listing of properties grouped by terrain type. Properties apply to a given range of tiles.
		public ulong GetTerrainTypeCount()												{ return Map_GetTerrainTypeCount(m_MapPtr);								}
		public TerrainType GetTerrainType(int index)
		{
			IntPtr terrainTypePtr = Map_GetTerrainType(m_MapPtr, index);
			TerrainType terrainType = Marshal.PtrToStructure<TerrainType>(terrainTypePtr);
			return terrainType;
		}
		public void SetTerrainType(int index, TerrainType terrainType)
		{
			IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<TerrainType>());
			Marshal.StructureToPtr(terrainType, ptr, false);

			Map_SetTerrainType(m_MapPtr, index, ptr);

			Marshal.FreeHGlobal(ptr);
		}
		public void AddTerrainType(TerrainType terrainType)
		{
			IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<TerrainType>());
			Marshal.StructureToPtr(terrainType, ptr, false);

			Map_AddTerrainType(m_MapPtr, ptr);

			Marshal.FreeHGlobal(ptr);
		}
		public void RemoveTerrainType(int index)										{ Map_RemoveTerrainType(m_MapPtr, index);											}

		//std::vector<TileGroup> tileGroups;
		public ulong GetTileGroupCount()												{ return Map_GetTileGroupCount(m_MapPtr);											}
		public string GetTileGroupName(int index)										{ return Marshalling.GetString(Map_GetTileGroupName(m_MapPtr, index));				}
		public uint GetTileGroupTileWidth(int index)									{ return Map_GetTileGroupTileWidth(m_MapPtr, index);								}
		public uint GetTileGroupTileHeight(int index)									{ return Map_GetTileGroupTileHeight(m_MapPtr, index);								}

		public ulong GetTileGroupMappingIndexCount(int tileGroupIndex)					{ return Map_GetTileGroupMappingIndexCount(m_MapPtr, tileGroupIndex);				}
		public uint GetTileGroupMappingIndex(int tileGroupIndex, int mappingIndex)		{ return Map_GetTileGroupMappingIndex(m_MapPtr, tileGroupIndex, mappingIndex);		}
		public void SetTileGroupMappingIndex(int tileGroupIndex, int mappingIndex, uint val){ Map_SetTileGroupMappingIndex(m_MapPtr, tileGroupIndex, mappingIndex, val);	}
		public void AddTileGroupMappingIndex(int tileGroupIndex, uint val)				{ Map_AddTileGroupMappingIndex(m_MapPtr, tileGroupIndex, val);						}
		public void RemoveTileGroupMappingIndex(int tileGroupIndex, int mappingIndex)	{ Map_RemoveTileGroupMappingIndex(m_MapPtr, tileGroupIndex, mappingIndex);			}

		public void SetTileGroupName(int tileGroupIndex, string groupName)				{ Map_SetTileGroupName(m_MapPtr, tileGroupIndex, groupName);						}
		public void SetTileGroupTileWidth(int tileGroupIndex, uint tileWidth)			{ Map_SetTileGroupTileWidth(m_MapPtr, tileGroupIndex, tileWidth);					}
		public void SetTileGroupTileHeight(int tileGroupIndex, uint tileHeight)			{ Map_SetTileGroupTileHeight(m_MapPtr, tileGroupIndex, tileHeight);					}
		public ulong AddTileGroup(int tileGroupIndex)									{ return Map_AddTileGroup(m_MapPtr);												} // Ret: new index
		public void RemoveTileGroup(int tileGroupIndex)									{ Map_RemoveTileGroup(m_MapPtr, tileGroupIndex);									}

		public void Write(string filename)												{ Map_Write(m_MapPtr, filename);													}
		
		public void SetVersionTag(uint versionTag)										{ Map_SetVersionTag(m_MapPtr, versionTag);											}
		public uint GetVersionTag()														{ return Map_GetVersionTag(m_MapPtr);												}
		public bool IsSavedGame()														{ return Map_IsSavedGame(m_MapPtr);													}
		public uint GetWidthInTiles()													{ return Map_GetWidthInTiles(m_MapPtr);												}
		public uint GetHeightInTiles()													{ return Map_GetHeightInTiles(m_MapPtr);											}

		public ulong GetTileMappingIndex(ulong x, ulong y)								{ return Map_GetTileMappingIndex(m_MapPtr, x, y);									}
		public int GetCellType(ulong x, ulong y)										{ return Map_GetCellType(m_MapPtr, x, y);											}
		public bool GetLavaPossible(ulong x, ulong y)									{ return Map_GetLavaPossible(m_MapPtr, x, y);										}
		public ulong GetTilesetIndex(ulong x, ulong y)									{ return Map_GetTilesetIndex(m_MapPtr, x, y);										}
		public ulong GetImageIndex(ulong x, ulong y)									{ return Map_GetImageIndex(m_MapPtr, x, y);											}

		public void CheckMinVersionTag(uint versionTag)									{ Map_CheckMinVersionTag(m_MapPtr, versionTag);										}

		public void TrimTilesetSources()												{ Map_TrimTilesetSources(m_MapPtr);													}


		
		[DllImport(Platform.DLLPath)] private static extern IntPtr Map_Create();
		[DllImport(Platform.DLLPath)] private static extern void Map_Release(IntPtr map);

		[DllImport(Platform.DLLPath)] private static extern IntPtr Map_ReadMap(string filename);
		[DllImport(Platform.DLLPath)] private static extern IntPtr Map_ReadMapStream(IntPtr buffer, ulong size);

		[DllImport(Platform.DLLPath)] private static extern IntPtr Map_ReadSavedGame(string filename);
		[DllImport(Platform.DLLPath)] private static extern IntPtr Map_ReadSavedGameStream(IntPtr buffer, ulong size);

		// 1D listing of all tiles on the associated map. See MapHeader data for height and width of map.
		[DllImport(Platform.DLLPath)] private static extern ulong Map_GetTileCount(IntPtr map);
		[DllImport(Platform.DLLPath)] private static extern int Map_GetTile(IntPtr map, int index);
		[DllImport(Platform.DLLPath)] private static extern void Map_SetTile(IntPtr map, int index, int tile);
		[DllImport(Platform.DLLPath)] private static extern void Map_AddTile(IntPtr map, int tile);
		[DllImport(Platform.DLLPath)] private static extern void Map_RemoveTile(IntPtr map, int index);

		[DllImport(Platform.DLLPath)] private static extern int Map_GetClipRectX1(IntPtr map);
		[DllImport(Platform.DLLPath)] private static extern int Map_GetClipRectX2(IntPtr map);
		[DllImport(Platform.DLLPath)] private static extern int Map_GetClipRectY1(IntPtr map);
		[DllImport(Platform.DLLPath)] private static extern int Map_GetClipRectY2(IntPtr map);

		[DllImport(Platform.DLLPath)] private static extern void Map_SetClipRectX1(IntPtr map, int value);
		[DllImport(Platform.DLLPath)] private static extern void Map_SetClipRectX2(IntPtr map, int value);
		[DllImport(Platform.DLLPath)] private static extern void Map_SetClipRectY1(IntPtr map, int value);
		[DllImport(Platform.DLLPath)] private static extern void Map_SetClipRectY2(IntPtr map, int value);

		// Listing of all tile set sources associated with the map.
		[DllImport(Platform.DLLPath)] private static extern ulong Map_GetTilesetSourceCount(IntPtr map);
		[DllImport(Platform.DLLPath)] private static extern IntPtr Map_GetTilesetSourceFilename(IntPtr map, int index);
		[DllImport(Platform.DLLPath)] private static extern uint Map_GetTilesetSourceNumTiles(IntPtr map, int index);
		[DllImport(Platform.DLLPath)] private static extern void Map_SetTilesetSourceFilename(IntPtr map, int index, string tilesetFilename);
		[DllImport(Platform.DLLPath)] private static extern void Map_SetTilesetSourceNumTiles(IntPtr map, int index, int numTiles);
		[DllImport(Platform.DLLPath)] private static extern void Map_AddTilesetSource(IntPtr map, string tilesetFilename, int numTiles);
		[DllImport(Platform.DLLPath)] private static extern void Map_RemoveTilesetSource(IntPtr map, int index);

		// Metadata about each available tile from the tile set sources.
		[DllImport(Platform.DLLPath)] private static extern ulong Map_GetTileMappingCount(IntPtr map);
		[DllImport(Platform.DLLPath)] private static extern ulong Map_GetTileMapping(IntPtr map, int index);
		[DllImport(Platform.DLLPath)] private static extern void Map_SetTileMapping(IntPtr map, int index, ulong tileMapping);
		[DllImport(Platform.DLLPath)] private static extern void Map_AddTileMapping(IntPtr map, ulong tileMapping);
		[DllImport(Platform.DLLPath)] private static extern void Map_RemoveTileMapping(IntPtr map, int index);

		// Listing of properties grouped by terrain type. Properties apply to a given range of tiles.
		[DllImport(Platform.DLLPath)] private static extern ulong Map_GetTerrainTypeCount(IntPtr map);
		[DllImport(Platform.DLLPath)] private static extern IntPtr Map_GetTerrainType(IntPtr map, int index);
		[DllImport(Platform.DLLPath)] private static extern void Map_SetTerrainType(IntPtr map, int index, IntPtr terrainType);
		[DllImport(Platform.DLLPath)] private static extern void Map_AddTerrainType(IntPtr map, IntPtr terrainType);
		[DllImport(Platform.DLLPath)] private static extern void Map_RemoveTerrainType(IntPtr map, int index);

		//std::vector<TileGroup> tileGroups;
		[DllImport(Platform.DLLPath)] private static extern ulong Map_GetTileGroupCount(IntPtr map);
		[DllImport(Platform.DLLPath)] private static extern IntPtr Map_GetTileGroupName(IntPtr map, int index);
		[DllImport(Platform.DLLPath)] private static extern uint Map_GetTileGroupTileWidth(IntPtr map, int index);
		[DllImport(Platform.DLLPath)] private static extern uint Map_GetTileGroupTileHeight(IntPtr map, int index);

		[DllImport(Platform.DLLPath)] private static extern ulong Map_GetTileGroupMappingIndexCount(IntPtr map, int tileGroupIndex);
		[DllImport(Platform.DLLPath)] private static extern uint Map_GetTileGroupMappingIndex(IntPtr map, int tileGroupIndex, int mappingIndex);
		[DllImport(Platform.DLLPath)] private static extern void Map_SetTileGroupMappingIndex(IntPtr map, int tileGroupIndex, int mappingIndex, uint value);
		[DllImport(Platform.DLLPath)] private static extern void Map_AddTileGroupMappingIndex(IntPtr map, int tileGroupIndex, uint value);
		[DllImport(Platform.DLLPath)] private static extern void Map_RemoveTileGroupMappingIndex(IntPtr map, int tileGroupIndex, int mappingIndex);

		[DllImport(Platform.DLLPath)] private static extern void Map_SetTileGroupName(IntPtr map, int index, string groupName);
		[DllImport(Platform.DLLPath)] private static extern void Map_SetTileGroupTileWidth(IntPtr map, int index, uint tileWidth);
		[DllImport(Platform.DLLPath)] private static extern void Map_SetTileGroupTileHeight(IntPtr map, int index, uint tileHeight);
		[DllImport(Platform.DLLPath)] private static extern ulong Map_AddTileGroup(IntPtr map);
		[DllImport(Platform.DLLPath)] private static extern void Map_RemoveTileGroup(IntPtr map, int index);

		[DllImport(Platform.DLLPath)] private static extern void Map_Write(IntPtr map, string filename);
		
		[DllImport(Platform.DLLPath)] private static extern void Map_SetVersionTag(IntPtr map, uint versionTag);
		[DllImport(Platform.DLLPath)] private static extern uint Map_GetVersionTag(IntPtr map);
		[DllImport(Platform.DLLPath)] private static extern bool Map_IsSavedGame(IntPtr map);
		[DllImport(Platform.DLLPath)] private static extern uint Map_GetWidthInTiles(IntPtr map);
		[DllImport(Platform.DLLPath)] private static extern uint Map_GetHeightInTiles(IntPtr map);

		[DllImport(Platform.DLLPath)] private static extern ulong Map_GetTileMappingIndex(IntPtr map, ulong x, ulong y);
		[DllImport(Platform.DLLPath)] private static extern int Map_GetCellType(IntPtr map, ulong x, ulong y);
		[DllImport(Platform.DLLPath)] private static extern bool Map_GetLavaPossible(IntPtr map, ulong x, ulong y);
		[DllImport(Platform.DLLPath)] private static extern ulong Map_GetTilesetIndex(IntPtr map, ulong x, ulong y);
		[DllImport(Platform.DLLPath)] private static extern ulong Map_GetImageIndex(IntPtr map, ulong x, ulong y);

		[DllImport(Platform.DLLPath)] private static extern void Map_CheckMinVersionTag(IntPtr map, uint versionTag);

		[DllImport(Platform.DLLPath)] private static extern void Map_TrimTilesetSources(IntPtr map);
	}
}
