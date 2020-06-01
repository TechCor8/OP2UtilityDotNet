#include "Marshalling.h"

using namespace std;

#if defined(_MSC_VER)
// Windows
#ifndef EXPORT
#define EXPORT __declspec(dllexport)
#endif
#elif defined(__GNUC__)
//  GCC
#define EXPORT __attribute__((visibility("default")))
#endif

const char* GetCStrFromString(std::string s)
{
	const char* _str = s.c_str();
	size_t length = strlen(_str);
	
	char* buffer = new char[length*2+1];
	strcpy(buffer/*, length*2+1*/, _str);

	return buffer;
}

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

std::vector<std::string> SplitString(std::string s, char delimiter)
{
	std::vector<std::string> vSplit;
	std::istringstream stream(s);
	std::string splitLine;

	while (std::getline(stream, splitLine, delimiter))
		vSplit.push_back(splitLine);

	return vSplit;
}

extern "C"
{
	extern EXPORT void FreeString(char* str)
	{
		delete [] str;
	}
}
