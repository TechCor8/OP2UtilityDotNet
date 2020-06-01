#pragma once

#include <cstring>
#include <vector>
#include <sstream>

const char* GetCStrFromString(std::string s);

std::string JoinStringVector(std::vector<std::string> vStrings, std::string delimiter);

std::vector<std::string> SplitString(std::string s, char delimiter);