#include "OP2Utility/include/OP2Utility.h"

#include "Marshalling.h"

#ifndef EXPORT
#define EXPORT __declspec(dllexport)
#endif

std::size_t GetTileIndex(std::size_t x, std::size_t y, uint32_t heightInTiles)
{
	auto lowerX = x & 0x1F; // ... 0001 1111
	auto upperX = x >> 5;   // ... 1110 0000
	return (upperX * heightInTiles + y) * 32 + lowerX;
}

extern "C"
{
	extern EXPORT Map* __stdcall Map_Create()																			{ return new Map();								}
	extern EXPORT void __stdcall Map_Release(Map* map)																	{ delete map;									}

	extern EXPORT Map* __stdcall Map_ReadMap(const char* filename)
	{
		try
		{
			return new Map(Map::ReadMap(filename));
		}
		catch (...)
		{
			return 0;
		}
	}
	extern EXPORT Map* __stdcall Map_ReadMapStream(void* buffer, unsigned __int64 size)
	{
		try
		{
			Stream::MemoryReader reader(buffer, size);
			return new Map(Map::ReadMap(reader));
		}
		catch (...)
		{
			return 0;
		}
	}

	extern EXPORT Map* __stdcall Map_ReadSavedGame(const char* filename)
	{
		try
		{
			return new Map(Map::ReadSavedGame(filename));
		}
		catch (...)
		{
			return 0;
		}
	}
	extern EXPORT Map* __stdcall Map_ReadSavedGameStream(const void* buffer, unsigned __int64 size)
	{
		try
		{
			Stream::MemoryReader reader(buffer, size);
			return new Map(Map::ReadSavedGame(reader));
		}
		catch (...)
		{
			return 0;
		}
	}

	// 1D listing of all tiles on the associated map. See MapHeader data for height and width of map.
	extern EXPORT unsigned __int64 __stdcall Map_GetTileCount(Map* map)													{ return map->tiles.size();						}
	extern EXPORT int __stdcall Map_GetTile(Map* map, unsigned __int64 index)											{ return *(int*)&(map->tiles[index]);			}
	extern EXPORT void __stdcall Map_SetTile(Map* map, unsigned __int64 index, int tile)								{ map->tiles[index] = *(Tile*)&tile;			}
	extern EXPORT void __stdcall Map_AddTile(Map* map, int tile)														{ map->tiles.push_back(*(Tile*)&tile);			}
	extern EXPORT void __stdcall Map_RemoveTile(Map* map, unsigned __int64 index)										{ map->tiles.erase(map->tiles.begin()+index);	}

	extern EXPORT int __stdcall Map_GetClipRectX1(Map* map)																{ return map->clipRect.x1;						}
	extern EXPORT int __stdcall Map_GetClipRectX2(Map* map)																{ return map->clipRect.x2;						}
	extern EXPORT int __stdcall Map_GetClipRectY1(Map* map)																{ return map->clipRect.y1;						}
	extern EXPORT int __stdcall Map_GetClipRectY2(Map* map)																{ return map->clipRect.y2;						}

	extern EXPORT void __stdcall Map_SetClipRectX1(Map* map, int value)													{ map->clipRect.x1 = value;						}
	extern EXPORT void __stdcall Map_SetClipRectX2(Map* map, int value)													{ map->clipRect.x2 = value;						}
	extern EXPORT void __stdcall Map_SetClipRectY1(Map* map, int value)													{ map->clipRect.y1 = value;						}
	extern EXPORT void __stdcall Map_SetClipRectY2(Map* map, int value)													{ map->clipRect.y2 = value;						}

	// Listing of all tile set sources associated with the map.
	extern EXPORT unsigned __int64 __stdcall Map_GetTilesetSourceCount(Map* map)										{ return map->tilesetSources.size();			}
	extern EXPORT const char* __stdcall Map_GetTilesetSourceFilename(Map* map, unsigned __int64 index)					{ return GetCStrFromString(map->tilesetSources[index].tilesetFilename);	}
	extern EXPORT unsigned int __stdcall Map_GetTilesetSourceNumTiles(Map* map, unsigned __int64 index)					{ return map->tilesetSources[index].numTiles;	}
	extern EXPORT void __stdcall Map_SetTilesetSourceFilename(Map* map, unsigned __int64 index, const char* tilesetFilename){ map->tilesetSources[index].tilesetFilename = tilesetFilename;	}
	extern EXPORT void __stdcall Map_SetTilesetSourceNumTiles(Map* map, unsigned __int64 index, int numTiles)			{ map->tilesetSources[index].numTiles = numTiles;				}
	extern EXPORT void __stdcall Map_AddTilesetSource(Map* map, const char* tilesetFilename, int numTiles)
	{
		TilesetSource src;
		src.tilesetFilename = tilesetFilename;
		src.numTiles = numTiles;
		map->tilesetSources.push_back(src);
	}
	extern EXPORT void __stdcall Map_RemoveTilesetSource(Map* map, unsigned __int64 index)								{ map->tilesetSources.erase(map->tilesetSources.begin()+index);	}

	// Metadata about each available tile from the tile set sources.
	extern EXPORT unsigned __int64 __stdcall Map_GetTileMappingCount(Map* map)											{ return map->tileMappings.size();							}
	extern EXPORT unsigned __int64 __stdcall Map_GetTileMapping(Map* map, unsigned __int64 index)
	{
		TileMapping mapping = map->tileMappings[index];
		unsigned __int64 val;
		val = (__int64)mapping.tilesetIndex << 48;
		val |= (__int64)mapping.tileGraphicIndex << 32;
		val |= mapping.animationCount << 16;
		val |= mapping.animationDelay;

		return val;
	}
	extern EXPORT void __stdcall Map_SetTileMapping(Map* map, unsigned __int64 index, unsigned __int64 tileMapping)
	{
		map->tileMappings[index].tilesetIndex = (tileMapping >> 48);
		map->tileMappings[index].tileGraphicIndex = (uint16_t)(tileMapping >> 32);
		map->tileMappings[index].animationCount = (uint16_t)(tileMapping >> 16);
		map->tileMappings[index].animationDelay = (uint16_t)tileMapping;
	}
	extern EXPORT void __stdcall Map_AddTileMapping(Map* map, unsigned __int64 tileMapping)
	{
		TileMapping mapping;
		mapping.tilesetIndex = (tileMapping >> 48);
		mapping.tileGraphicIndex = (uint16_t)(tileMapping >> 32);
		mapping.animationCount = (uint16_t)(tileMapping >> 16);
		mapping.animationDelay = (uint16_t)tileMapping;

		map->tileMappings.push_back(mapping);
	}
	extern EXPORT void __stdcall Map_RemoveTileMapping(Map* map, unsigned __int64 index)								{ map->tileMappings.erase(map->tileMappings.begin()+index);	}

	// Listing of properties grouped by terrain type. Properties apply to a given range of tiles.
	extern EXPORT unsigned __int64 __stdcall Map_GetTerrainTypeCount(Map* map)											{ return map->terrainTypes.size();							}
	extern EXPORT TerrainType* __stdcall Map_GetTerrainType(Map* map, unsigned __int64 index)							{ return &map->terrainTypes[index];							}
	extern EXPORT void __stdcall Map_SetTerrainType(Map* map, unsigned __int64 index, TerrainType* terrainType)			{ map->terrainTypes[index] = *terrainType;					}
	extern EXPORT void __stdcall Map_AddTerrainType(Map* map, TerrainType* terrainType)									{ map->terrainTypes.push_back(*terrainType);				}
	extern EXPORT void __stdcall Map_RemoveTerrainType(Map* map, unsigned __int64 index)								{ map->terrainTypes.erase(map->terrainTypes.begin()+index);	}
	
	//std::vector<TileGroup> tileGroups;
	extern EXPORT unsigned __int64 __stdcall Map_GetTileGroupCount(Map* map)											{ return map->tileGroups.size();								}
	extern EXPORT const char* __stdcall Map_GetTileGroupName(Map* map, unsigned __int64 index)							{ return GetCStrFromString(map->tileGroups[index].name);		}
	extern EXPORT unsigned int __stdcall Map_GetTileGroupTileWidth(Map* map, unsigned __int64 index)					{ return map->tileGroups[index].tileWidth;						}
	extern EXPORT unsigned int __stdcall Map_GetTileGroupTileHeight(Map* map, unsigned __int64 index)					{ return map->tileGroups[index].tileHeight;						}

	extern EXPORT unsigned __int64 __stdcall Map_GetTileGroupMappingIndexCount(Map* map, unsigned __int64 tileGroupIndex)									{ return map->tileGroups[tileGroupIndex].mappingIndices.size();			}
	extern EXPORT unsigned int __stdcall Map_GetTileGroupMappingIndex(Map* map, unsigned __int64 tileGroupIndex, unsigned __int64 mappingIndex)				{ return map->tileGroups[tileGroupIndex].mappingIndices[mappingIndex];	}
	extern EXPORT void __stdcall Map_SetTileGroupMappingIndex(Map* map, unsigned __int64 tileGroupIndex, unsigned __int64 mappingIndex, unsigned int value)	{ map->tileGroups[tileGroupIndex].mappingIndices[mappingIndex] = value;	}
	extern EXPORT void __stdcall Map_AddTileGroupMappingIndex(Map* map, unsigned __int64 tileGroupIndex, unsigned int value)								{ map->tileGroups[tileGroupIndex].mappingIndices.push_back(value);		}
	extern EXPORT void __stdcall Map_RemoveTileGroupMappingIndex(Map* map, unsigned __int64 tileGroupIndex, unsigned __int64 mappingIndex)
	{
		map->tileGroups[tileGroupIndex].mappingIndices.erase( map->tileGroups[tileGroupIndex].mappingIndices.begin()+mappingIndex);
	}

	extern EXPORT void __stdcall Map_SetTileGroupName(Map* map, unsigned __int64 index, const char* groupName)			{ map->tileGroups[index].name = groupName;						}
	extern EXPORT void __stdcall Map_SetTileGroupTileWidth(Map* map, unsigned __int64 index, unsigned int tileWidth)	{ map->tileGroups[index].tileWidth = tileWidth;					}
	extern EXPORT void __stdcall Map_SetTileGroupTileHeight(Map* map, unsigned __int64 index, unsigned int tileHeight)	{ map->tileGroups[index].tileHeight = tileHeight;				}
	extern EXPORT unsigned __int64 __stdcall Map_AddTileGroup(Map* map)
	{
		TileGroup group;
		map->tileGroups.push_back(group);
		return map->tileGroups.size()-1;
	}
	extern EXPORT void __stdcall Map_RemoveTileGroup(Map* map, unsigned __int64 index)									{ map->tileGroups.erase(map->tileGroups.begin()+index);			}

	extern EXPORT void __stdcall Map_Write(Map* map, const char* filename)												{ return map->Write(filename);					}
	//void Write(Stream::Writer& streamWriter) const;

	extern EXPORT void __stdcall Map_SetVersionTag(Map* map, unsigned int versionTag)									{ return map->SetVersionTag(versionTag);		}
	extern EXPORT unsigned int __stdcall Map_GetVersionTag(Map* map)													{ return map->GetVersionTag();					}
	extern EXPORT bool __stdcall Map_IsSavedGame(Map* map)																{ return map->IsSavedGame();					}
	extern EXPORT unsigned int __stdcall Map_GetWidthInTiles(Map* map)													{ return map->WidthInTiles();					}
	extern EXPORT unsigned int __stdcall Map_GetHeightInTiles(Map* map)													{ return map->HeightInTiles();					}

	extern EXPORT unsigned __int64 __stdcall Map_GetTileMappingIndex(Map* map, unsigned __int64 x, unsigned __int64 y)	{ return map->GetTileMappingIndex(x, y);		}
	extern EXPORT int __stdcall Map_GetCellType(Map* map, unsigned __int64 x, unsigned __int64 y)						{ return (int)map->GetCellType(x, y);			}
	extern EXPORT bool __stdcall Map_GetLavaPossible(Map* map, unsigned __int64 x, unsigned __int64 y)					{ return map->GetLavaPossible(x, y);			}
	extern EXPORT unsigned __int64 __stdcall Map_GetTilesetIndex(Map* map, unsigned __int64 x, unsigned __int64 y)		{ return map->GetTilesetIndex(x, y);			}
	extern EXPORT unsigned __int64 __stdcall Map_GetImageIndex(Map* map, unsigned __int64 x, unsigned __int64 y)		{ return map->GetImageIndex(x, y);				}

	extern EXPORT void __stdcall Map_SetTileMappingIndex(Map* map, unsigned __int64 x, unsigned __int64 y, unsigned __int64 mappingIndex)	{ map->tiles[GetTileIndex(x, y, map->HeightInTiles())].tileMappingIndex = mappingIndex;		}
	extern EXPORT void __stdcall Map_SetCellType(Map* map, unsigned __int64 x, unsigned __int64 y, int cellType)							{ map->tiles[GetTileIndex(x, y, map->HeightInTiles())].cellType = (CellType)cellType;		}
	extern EXPORT void __stdcall Map_SetLavaPossible(Map* map, unsigned __int64 x, unsigned __int64 y, bool lavaPossible)					{ map->tiles[GetTileIndex(x, y, map->HeightInTiles())].bLavaPossible = lavaPossible;		}

	extern EXPORT void __stdcall Map_CheckMinVersionTag(Map* map, unsigned int versionTag)								{ return map->CheckMinVersionTag(versionTag);	}

	extern EXPORT void __stdcall Map_TrimTilesetSources(Map* map)														{ return map->TrimTilesetSources();				}
}
