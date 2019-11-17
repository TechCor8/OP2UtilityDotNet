#include "OP2Utility/include/OP2Utility.h"

#include "Marshalling.h"

#ifndef EXPORT
#define EXPORT __declspec(dllexport)
#endif

extern "C"
{
	extern EXPORT Archive::VolFile* __stdcall Archive_CreateVolFile(const char* filename)										{ return new Archive::VolFile(filename);					}
	extern EXPORT void __stdcall Archive_ReleaseVolFile(Archive::VolFile* volFile)												{ delete volFile;											}

	extern EXPORT Archive::ClmFile* __stdcall Archive_CreateClmFile(const char* filename)										{ return new Archive::ClmFile(filename);					}
	extern EXPORT void __stdcall Archive_ReleaseClmFile(Archive::ClmFile* clmFile)												{ delete clmFile;											}

	extern EXPORT const char* __stdcall Archive_GetArchiveFilename(Archive::ArchiveFile* archive)								{ return GetCStrFromString(archive->GetArchiveFilename());	}
	extern EXPORT unsigned __int64 __stdcall Archive_GetArchiveFileSize(Archive::ArchiveFile* archive)							{ return archive->GetArchiveFileSize();						}
	extern EXPORT unsigned __int64 __stdcall Archive_GetCount(Archive::ArchiveFile* archive)									{ return archive->GetCount();								}

	extern EXPORT bool __stdcall Archive_Contains(Archive::ArchiveFile* archive, const char* name)								{ return archive->Contains(name);							}
	extern EXPORT void __stdcall Archive_ExtractFileByName(Archive::ArchiveFile* archive, const char* name, const char* pathOut){ archive->ExtractFile(name, pathOut);						}

	extern EXPORT unsigned __int64 __stdcall Archive_GetIndex(Archive::ArchiveFile* archive, const char* name)					{ return archive->GetIndex(name);							}
	extern EXPORT const char* __stdcall Archive_GetName(Archive::ArchiveFile* archive, unsigned __int64 index)					{ return GetCStrFromString(archive->GetName(index));		}

	extern EXPORT unsigned int __stdcall Archive_GetSize(Archive::ArchiveFile* archive, unsigned __int64 index)					{ return archive->GetSize(index);							}
	extern EXPORT void __stdcall Archive_ExtractFileByIndex(Archive::ArchiveFile* archive, unsigned __int64 index, const char* pathOut){ archive->ExtractFile(index, pathOut);				}
	extern EXPORT void __stdcall Archive_ExtractAllFiles(Archive::ArchiveFile* archive, const char* destDirectory)				{ archive->ExtractAllFiles(destDirectory);					}

	// Pipe delimited
	extern EXPORT void __stdcall Archive_WriteVolFile(const char* volumeFilename, const char* filesToPack)
	{
		Archive::VolFile::CreateArchive(volumeFilename, SplitString(filesToPack, '|'));
	}

	// Pipe delimited
	extern EXPORT void __stdcall Archive_WriteClmFile(const char* archiveFilename, const char* filesToPack)
	{
		Archive::ClmFile::CreateArchive(archiveFilename, SplitString(filesToPack, '|'));
	}

	extern EXPORT void __stdcall Archive_ReadFileByIndex(Archive::ArchiveFile* archive, unsigned __int64 index, char* buffer)
	{
		std::unique_ptr<Stream::BidirectionalReader> stream = archive->OpenStream(index);

		stream->Read(buffer, stream->Length());
	}


	// VolFile only
	extern EXPORT int __stdcall Archive_GetCompressionCode(Archive::VolFile* archive, unsigned __int64 index)					{ return (int)archive->GetCompressionCode(index);	}
}
