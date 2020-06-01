using System;
using System.Runtime.InteropServices;

namespace OP2UtilityDotNet
{
	public class ClmFile : Archive
	{
		public ClmFile(string filename)				{ m_ArchivePtr = Archive_CreateClmFile(filename);				}
		public override void Dispose()				{ Archive_ReleaseClmFile(m_ArchivePtr);							}

		public static void WriteClmFile(string archiveFilename, string[] filesToPack)
		{
			string files = string.Join("|", filesToPack);
			Archive_WriteClmFile(archiveFilename, files);
		}

		[DllImport(Platform.DLLPath, CallingConvention=CallingConvention.Cdecl)] private static extern IntPtr Archive_CreateClmFile(string filename);
		[DllImport(Platform.DLLPath, CallingConvention=CallingConvention.Cdecl)] private static extern void Archive_ReleaseClmFile(IntPtr clmFile);

		[DllImport(Platform.DLLPath, CallingConvention=CallingConvention.Cdecl)] private static extern void Archive_WriteClmFile(string archiveFilename, string filesToPack);	// Pipe delimited
	}
}
