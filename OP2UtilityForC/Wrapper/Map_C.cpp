#include "../../OP2Utility/include/OP2Utility.h"

#include "../Marshalling.h"

#if defined(_MSC_VER)
// Windows
#ifndef EXPORT
#define EXPORT __declspec(dllexport)
#endif
#elif defined(__GNUC__)
//  GCC
#define EXPORT __attribute__((visibility("default")))
#endif

std::size_t GetTileIndex(std::size_t x, std::size_t y, uint32_t heightInTiles)
{
	auto lowerX = x & 0x1F; // ... 0001 1111
	auto upperX = x >> 5;   // ... 1110 0000
	return (upperX * heightInTiles + y) * 32 + lowerX;
}

extern "C"
{
	extern EXPORT Map* Map_Create()																			{ return new Map();								}
	extern EXPORT void Map_Release(Map* map)																{ delete map;									}

	extern EXPORT Map* Map_ReadMap(const char* filename)
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
	extern EXPORT Map* Map_ReadMapStream(void* buffer, uint64_t size)
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

	extern EXPORT Map* Map_ReadSavedGame(const char* filename)
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
	extern EXPORT Map* Map_ReadSavedGameStream(const void* buffer, uint64_t size)
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
	extern EXPORT uint64_t Map_GetTileCount(Map* map)															{ return map->tiles.size();						}
	extern EXPORT int Map_GetTile(Map* map, uint64_t index)														{ return *(int*)&(map->tiles[index]);			}
	extern EXPORT void Map_SetTile(Map* map, uint64_t index, int tile)											{ map->tiles[index] = *(Tile*)&tile;			}
	extern EXPORT void Map_AddTile(Map* map, int tile)															{ map->tiles.push_back(*(Tile*)&tile);			}
	extern EXPORT void Map_RemoveTile(Map* map, uint64_t index)													{ map->tiles.erase(map->tiles.begin()+index);	}

	extern EXPORT int Map_GetClipRectX1(Map* map)																{ return map->clipRect.x1;						}
	extern EXPORT int Map_GetClipRectX2(Map* map)																{ return map->clipRect.x2;						}
	extern EXPORT int Map_GetClipRectY1(Map* map)																{ return map->clipRect.y1;						}
	extern EXPORT int Map_GetClipRectY2(Map* map)																{ return map->clipRect.y2;						}

	extern EXPORT void Map_SetClipRectX1(Map* map, int value)													{ map->clipRect.x1 = value;						}
	extern EXPORT void Map_SetClipRectX2(Map* map, int value)													{ map->clipRect.x2 = value;						}
	extern EXPORT void Map_SetClipRectY1(Map* map, int value)													{ map->clipRect.y1 = value;						}
	extern EXPORT void Map_SetClipRectY2(Map* map, int value)													{ map->clipRect.y2 = value;						}

	// Listing of all tile set sources associated with the map.
	extern EXPORT uint64_t Map_GetTilesetSourceCount(Map* map)													{ return map->tilesetSources.size();			}
	extern EXPORT const char* Map_GetTilesetSourceFilename(Map* map, uint64_t index)							{ return GetCStrFromString(map->tilesetSources[index].tilesetFilename);	}
	extern EXPORT unsigned int Map_GetTilesetSourceNumTiles(Map* map, uint64_t index)							{ return map->tilesetSources[index].numTiles;	}
	extern EXPORT void Map_SetTilesetSourceFilename(Map* map, uint64_t index, const char* tilesetFilename)		{ map->tilesetSources[index].tilesetFilename = tilesetFilename;	}
	extern EXPORT void Map_SetTilesetSourceNumTiles(Map* map, uint64_t index, int numTiles)						{ map->tilesetSources[index].numTiles = numTiles;				}
	extern EXPORT void Map_AddTilesetSource(Map* map, const char* tilesetFilename, int numTiles)
	{
		TilesetSource src;
		src.tilesetFilename = tilesetFilename;
		src.numTiles = numTiles;
		map->tilesetSources.push_back(src);
	}
	extern EXPORT void Map_RemoveTilesetSource(Map* map, uint64_t index)										{ map->tilesetSources.erase(map->tilesetSources.begin()+index);	}

	// Metadata about each available tile from the tile set sources.
	extern EXPORT uint64_t Map_GetTileMappingCount(Map* map)													{ return map->tileMappings.size();							}
	extern EXPORT uint64_t Map_GetTileMapping(Map* map, uint64_t index)
	{
		TileMapping mapping = map->tileMappings[index];
		uint64_t val;
		val = (int64_t)mapping.tilesetIndex << 48;
		val |= (int64_t)mapping.tileGraphicIndex << 32;
		val |= mapping.animationCount << 16;
		val |= mapping.animationDelay;

		return val;
	}
	extern EXPORT void Map_SetTileMapping(Map* map, uint64_t index, uint64_t tileMapping)
	{
		map->tileMappings[index].tilesetIndex = (tileMapping >> 48);
		map->tileMappings[index].tileGraphicIndex = (uint16_t)(tileMapping >> 32);
		map->tileMappings[index].animationCount = (uint16_t)(tileMapping >> 16);
		map->tileMappings[index].animationDelay = (uint16_t)tileMapping;
	}
	extern EXPORT void Map_AddTileMapping(Map* map, uint64_t tileMapping)
	{
		TileMapping mapping;
		mapping.tilesetIndex = (tileMapping >> 48);
		mapping.tileGraphicIndex = (uint16_t)(tileMapping >> 32);
		mapping.animationCount = (uint16_t)(tileMapping >> 16);
		mapping.animationDelay = (uint16_t)tileMapping;

		map->tileMappings.push_back(mapping);
	}
	extern EXPORT void Map_RemoveTileMapping(Map* map, uint64_t index)											{ map->tileMappings.erase(map->tileMappings.begin()+index);	}

	// Listing of properties grouped by terrain type. Properties apply to a given range of tiles.
	extern EXPORT uint64_t Map_GetTerrainTypeCount(Map* map)													{ return map->terrainTypes.size();							}
	extern EXPORT TerrainType* Map_GetTerrainType(Map* map, uint64_t index)										{ return &map->terrainTypes[index];							}
	extern EXPORT void Map_SetTerrainType(Map* map, uint64_t index, TerrainType* terrainType)					{ map->terrainTypes[index] = *terrainType;					}
	extern EXPORT void Map_AddTerrainType(Map* map, TerrainType* terrainType)									{ map->terrainTypes.push_back(*terrainType);				}
	extern EXPORT void Map_RemoveTerrainType(Map* map, uint64_t index)											{ map->terrainTypes.erase(map->terrainTypes.begin()+index);	}
	
	//std::vector<TileGroup> tileGroups;
	extern EXPORT uint64_t Map_GetTileGroupCount(Map* map)														{ return map->tileGroups.size();								}
	extern EXPORT const char* Map_GetTileGroupName(Map* map, uint64_t index)									{ return GetCStrFromString(map->tileGroups[index].name);		}
	extern EXPORT unsigned int Map_GetTileGroupTileWidth(Map* map, uint64_t index)								{ return map->tileGroups[index].tileWidth;						}
	extern EXPORT unsigned int Map_GetTileGroupTileHeight(Map* map, uint64_t index)								{ return map->tileGroups[index].tileHeight;						}

	extern EXPORT uint64_t Map_GetTileGroupMappingIndexCount(Map* map, uint64_t tileGroupIndex)														{ return map->tileGroups[tileGroupIndex].mappingIndices.size();			}
	extern EXPORT unsigned int Map_GetTileGroupMappingIndex(Map* map, uint64_t tileGroupIndex, uint64_t mappingIndex)								{ return map->tileGroups[tileGroupIndex].mappingIndices[mappingIndex];	}
	extern EXPORT void Map_SetTileGroupMappingIndex(Map* map, uint64_t tileGroupIndex, uint64_t mappingIndex, unsigned int value)					{ map->tileGroups[tileGroupIndex].mappingIndices[mappingIndex] = value;	}
	extern EXPORT void Map_AddTileGroupMappingIndex(Map* map, uint64_t tileGroupIndex, unsigned int value)											{ map->tileGroups[tileGroupIndex].mappingIndices.push_back(value);		}
	extern EXPORT void Map_RemoveTileGroupMappingIndex(Map* map, uint64_t tileGroupIndex, uint64_t mappingIndex)
	{
		map->tileGroups[tileGroupIndex].mappingIndices.erase( map->tileGroups[tileGroupIndex].mappingIndices.begin()+mappingIndex);
	}

	extern EXPORT void Map_SetTileGroupName(Map* map, uint64_t index, const char* groupName)					{ map->tileGroups[index].name = groupName;						}
	extern EXPORT void Map_SetTileGroupTileWidth(Map* map, uint64_t index, unsigned int tileWidth)				{ map->tileGroups[index].tileWidth = tileWidth;					}
	extern EXPORT void Map_SetTileGroupTileHeight(Map* map, uint64_t index, unsigned int tileHeight)			{ map->tileGroups[index].tileHeight = tileHeight;				}
	extern EXPORT uint64_t Map_AddTileGroup(Map* map)
	{
		TileGroup group;
		map->tileGroups.push_back(group);
		return map->tileGroups.size()-1;
	}
	extern EXPORT void Map_RemoveTileGroup(Map* map, uint64_t index)											{ map->tileGroups.erase(map->tileGroups.begin()+index);			}

	extern EXPORT void Map_Write(Map* map, const char* filename)												{ return map->Write(filename);					}
	//void Write(Stream::Writer& streamWriter) const;

	extern EXPORT void Map_SetVersionTag(Map* map, unsigned int versionTag)										{ return map->SetVersionTag(versionTag);		}
	extern EXPORT unsigned int Map_GetVersionTag(Map* map)														{ return map->GetVersionTag();					}
	extern EXPORT bool Map_IsSavedGame(Map* map)																{ return map->IsSavedGame();					}
	extern EXPORT unsigned int Map_GetWidthInTiles(Map* map)													{ return map->WidthInTiles();					}
	extern EXPORT unsigned int Map_GetHeightInTiles(Map* map)													{ return map->HeightInTiles();					}

	extern EXPORT uint64_t Map_GetTileMappingIndex(Map* map, uint64_t x, uint64_t y)							{ return map->GetTileMappingIndex(x, y);		}
	extern EXPORT int Map_GetCellType(Map* map, uint64_t x, uint64_t y)											{ return (int)map->GetCellType(x, y);			}
	extern EXPORT bool Map_GetLavaPossible(Map* map, uint64_t x, uint64_t y)									{ return map->GetLavaPossible(x, y);			}
	extern EXPORT uint64_t Map_GetTilesetIndex(Map* map, uint64_t x, uint64_t y)								{ return map->GetTilesetIndex(x, y);			}
	extern EXPORT uint64_t Map_GetImageIndex(Map* map, uint64_t x, uint64_t y)									{ return map->GetImageIndex(x, y);				}

	extern EXPORT void Map_SetTileMappingIndex(Map* map, uint64_t x, uint64_t y, uint64_t mappingIndex)			{ map->tiles[GetTileIndex(x, y, map->HeightInTiles())].tileMappingIndex = mappingIndex;		}
	extern EXPORT void Map_SetCellType(Map* map, uint64_t x, uint64_t y, int cellType)							{ map->tiles[GetTileIndex(x, y, map->HeightInTiles())].cellType = (CellType)cellType;		}
	extern EXPORT void Map_SetLavaPossible(Map* map, uint64_t x, uint64_t y, bool lavaPossible)					{ map->tiles[GetTileIndex(x, y, map->HeightInTiles())].bLavaPossible = lavaPossible;		}

	extern EXPORT void Map_CheckMinVersionTag(Map* map, unsigned int versionTag)								{ return map->CheckMinVersionTag(versionTag);	}

	extern EXPORT void Map_TrimTilesetSources(Map* map)															{ return map->TrimTilesetSources();				}
}
