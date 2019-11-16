#include "OP2Utility/include/OP2Utility.h"

#ifndef EXPORT
#define EXPORT __declspec(dllexport)
#endif

std::string JoinStringVector(std::vector<std::string> vStrings, std::string delimiter)
{
	std::string result;

	for (std::vector<std::string>::const_iterator i = vStrings.begin(); i != vStrings.end(); ++i)
	{
		if (i == vStrings.begin())
			result += *i;
		else
			result += delimiter + *i;
	}

	return result;
}

extern "C"
{
	extern EXPORT ResourceManager* __stdcall ResourceManager_Create(const char* archiveDirectory)		{ return new ResourceManager(archiveDirectory);			}
	extern EXPORT void __stdcall ResourceManager_Release(ResourceManager* resourceManager)				{ delete resourceManager;								}

	extern EXPORT const char* __stdcall ResourceManager_GetAllFilenames(ResourceManager* resourceManager, const char* filenameRegexStr, bool accessArchives)
	{
		std::vector<std::string> vFilenames = resourceManager->GetAllFilenames(filenameRegexStr, accessArchives);

		// Filenames are compiled into pipe delimited string
		return JoinStringVector(vFilenames, "|").c_str();
	}

	extern EXPORT const char* __stdcall ResourceManager_GetAllFilenamesOfType(ResourceManager* resourceManager, const char* extension, bool accessArchives)
	{
		std::vector<std::string> vFilenames = resourceManager->GetAllFilenamesOfType(extension, accessArchives);

		// Filenames are compiled into pipe delimited string
		return JoinStringVector(vFilenames, "|").c_str();
	}

	// Returns an empty string if file is not located in an archive file in the ResourceManager's working directory.
	extern EXPORT const char* __stdcall ResourceManager_FindContainingArchivePath(ResourceManager* resourceManager, const char* filename)
	{
		return resourceManager->FindContainingArchivePath(filename).c_str();
	}

	// Returns a list of all loaded archives
	extern EXPORT const char* __stdcall ResourceManager_GetArchiveFilenames(ResourceManager* resourceManager)
	{
		std::vector<std::string> vFilenames = resourceManager->GetArchiveFilenames();

		// Filenames are compiled into pipe delimited string
		return JoinStringVector(vFilenames, "|").c_str();
	}

	extern EXPORT unsigned __int64 __stdcall ResourceManager_GetResourceSize(ResourceManager* resourceManager, const char* filename, bool accessArchives)
	{
		std::unique_ptr<Stream::BidirectionalReader> stream = resourceManager->GetResourceStream(filename, accessArchives);

		return stream->Length();
	}

	extern EXPORT void __stdcall ResourceManager_GetResource(ResourceManager* resourceManager, const char* filename, bool accessArchives, char* buffer)
	{
		std::unique_ptr<Stream::BidirectionalReader> stream = resourceManager->GetResourceStream(filename, accessArchives);

		stream->Read(buffer, stream->Length());
	}
}
