using System;
using System.Runtime.InteropServices;

namespace OP2UtilityDotNet
{
	// Represents compression flags used by Outpost 2 .VOL files to compress files
	// See FileFormat Archive Vol.txt for more details
	public enum CompressionType
	{
		Uncompressed = 0x100,
		RLE = 0x101,  // Run Length Encoded. Currently not supported.
		LZ = 0x102,   // Lempel - Ziv, named after the two creators. Currently not supported.
		LZH = 0x103,  // Lempel - Ziv, with adaptive Huffman encoding.
	}

	public class VolFile : Archive
	{
		public VolFile(string filename)							{ m_ArchivePtr = Archive_CreateVolFile(filename);								}
		public override void Dispose()							{ Archive_ReleaseVolFile(m_ArchivePtr);											}

		public CompressionType GetCompressionCode(ulong index)	{ return (CompressionType)Archive_GetCompressionCode(m_ArchivePtr, index);		}

		public static void WriteVolFile(string volumeFilename, string[] filesToPack)
		{
			string files = string.Join("|", filesToPack);
			Archive_WriteVolFile(volumeFilename, files);
		}

		[DllImport("OP2UtilityForC.dll")] private static extern IntPtr Archive_CreateVolFile(string filename);
		[DllImport("OP2UtilityForC.dll")] private static extern void Archive_ReleaseVolFile(IntPtr volFile);

		[DllImport("OP2UtilityForC.dll")] private static extern int Archive_GetCompressionCode(IntPtr archive, ulong index);

		[DllImport("OP2UtilityForC.dll")] private static extern void Archive_WriteVolFile(string volumeFilename, string filesToPack);	// Pipe delimited
	}
}
