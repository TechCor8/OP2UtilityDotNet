using System;
using System.Runtime.InteropServices;

namespace OP2UtilityDotNet
{
	class OP2BmpLoader : IDisposable
	{
		protected IntPtr m_BmpLoaderPtr;

		public OP2BmpLoader(string bmpFilename, string artFilename)		{ m_BmpLoaderPtr = OP2BmpLoader_Create(bmpFilename, artFilename);		}
		public void Dispose()											{ OP2BmpLoader_Release(m_BmpLoaderPtr);									}

		public void ExtractImage(ulong index, string filenameOut)		{ OP2BmpLoader_ExtractImage(m_BmpLoaderPtr, index, filenameOut);		}


		[DllImport("OP2UtilityForC.dll")] private static extern IntPtr OP2BmpLoader_Create(string bmpFilename, string artFilename);
		[DllImport("OP2UtilityForC.dll")] private static extern void OP2BmpLoader_Release(IntPtr bmpLoader);

		[DllImport("OP2UtilityForC.dll")] private static extern void OP2BmpLoader_ExtractImage(IntPtr bmpLoader, ulong index, string filenameOut);
	}
}
