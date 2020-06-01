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

extern "C"
{
	extern EXPORT Archive::VolFile* Archive_CreateVolFile(const char* filename)											{ return new Archive::VolFile(filename);					}
	extern EXPORT void Archive_ReleaseVolFile(Archive::VolFile* volFile)												{ delete volFile;											}

	extern EXPORT Archive::ClmFile* Archive_CreateClmFile(const char* filename)											{ return new Archive::ClmFile(filename);					}
	extern EXPORT void Archive_ReleaseClmFile(Archive::ClmFile* clmFile)												{ delete clmFile;											}

	extern EXPORT const char* Archive_GetArchiveFilename(Archive::ArchiveFile* archive)									{ return GetCStrFromString(archive->GetArchiveFilename());	}
	extern EXPORT uint64_t Archive_GetArchiveFileSize(Archive::ArchiveFile* archive)									{ return archive->GetArchiveFileSize();						}
	extern EXPORT uint64_t Archive_GetCount(Archive::ArchiveFile* archive)												{ return archive->GetCount();								}

	extern EXPORT bool Archive_Contains(Archive::ArchiveFile* archive, const char* name)								{ return archive->Contains(name);							}
	extern EXPORT void Archive_ExtractFileByName(Archive::ArchiveFile* archive, const char* name, const char* pathOut)	{ archive->ExtractFile(name, pathOut);						}

	extern EXPORT uint64_t Archive_GetIndex(Archive::ArchiveFile* archive, const char* name)							{ return archive->GetIndex(name);							}
	extern EXPORT const char* Archive_GetName(Archive::ArchiveFile* archive, uint64_t index)							{ return GetCStrFromString(archive->GetName(index));		}

	extern EXPORT unsigned int Archive_GetSize(Archive::ArchiveFile* archive, uint64_t index)							{ return archive->GetSize(index);							}
	extern EXPORT void Archive_ExtractFileByIndex(Archive::ArchiveFile* archive, uint64_t index, const char* pathOut)	{ archive->ExtractFile(index, pathOut);						}
	extern EXPORT void Archive_ExtractAllFiles(Archive::ArchiveFile* archive, const char* destDirectory)				{ archive->ExtractAllFiles(destDirectory);					}

	// Pipe delimited
	extern EXPORT void Archive_WriteVolFile(const char* volumeFilename, const char* filesToPack)
	{
		Archive::VolFile::CreateArchive(volumeFilename, SplitString(filesToPack, '|'));
	}

	// Pipe delimited
	extern EXPORT void Archive_WriteClmFile(const char* archiveFilename, const char* filesToPack)
	{
		Archive::ClmFile::CreateArchive(archiveFilename, SplitString(filesToPack, '|'));
	}

	extern EXPORT bool Archive_ReadFileByIndex(Archive::ArchiveFile* archive, uint64_t index, char* buffer)
	{
		std::unique_ptr<Stream::BidirectionalReader> stream = archive->OpenStream(index);

		if (stream == nullptr)
			return false;

		stream->Read(buffer, stream->Length());

		return true;
	}


	// VolFile only
	extern EXPORT int Archive_GetCompressionCode(Archive::VolFile* archive, uint64_t index)								{ return (int)archive->GetCompressionCode(index);			}
}
