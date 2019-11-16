#include "OP2Utility/include/OP2Utility.h"

#include "OP2Utility/src/Sprite/OP2BmpLoader.h"

#ifndef EXPORT
#define EXPORT __declspec(dllexport)
#endif


extern "C"
{
	extern EXPORT OP2BmpLoader* __stdcall OP2BmpLoader_Create(const char* bmpFilename, const char* artFilename)		{ return new OP2BmpLoader(bmpFilename, artFilename);	}
	extern EXPORT void __stdcall OP2BmpLoader_Release(OP2BmpLoader* bmpLoader)										{ delete bmpLoader;										}

	extern EXPORT void __stdcall OP2BmpLoader_ExtractImage(OP2BmpLoader* bmpLoader, unsigned __int64 index, const char* filenameOut)	{ bmpLoader->ExtractImage(index, filenameOut);	}
}
