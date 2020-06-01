#include "../../OP2Utility/include/OP2Utility.h"

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
	extern EXPORT OP2BmpLoader* OP2BmpLoader_Create(const char* bmpFilename, const char* artFilename)		{ return new OP2BmpLoader(bmpFilename, artFilename);	}
	extern EXPORT void OP2BmpLoader_Release(OP2BmpLoader* bmpLoader)										{ delete bmpLoader;										}

	extern EXPORT void OP2BmpLoader_ExtractImage(OP2BmpLoader* bmpLoader, uint64_t index, const char* filenameOut)	{ bmpLoader->ExtractImage(index, filenameOut);	}
}
