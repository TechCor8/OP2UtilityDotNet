﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace OP2UtilityDotNet
{
	public class ResourceManager : IDisposable
	{
		protected IntPtr m_ResourceManagerPtr;

		public ResourceManager(string archiveDirectory)
		{
			if (Directory.Exists(archiveDirectory))
				m_ResourceManagerPtr = ResourceManager_Create(archiveDirectory);
		}

		public void Dispose()
		{
			if (m_ResourceManagerPtr != null)
				ResourceManager_Release(m_ResourceManagerPtr);
		}

		public string[] GetAllFilenames(string filenameRegexStr, bool accessArchives)
		{
			if (m_ResourceManagerPtr == null)
				return new string[0];

			return Marshalling.GetString(ResourceManager_GetAllFilenames(m_ResourceManagerPtr, filenameRegexStr, accessArchives)).Split('|');
		}

		public string[] GetAllFilenamesOfType(string filenameRegexStr, bool accessArchives)
		{
			if (m_ResourceManagerPtr == null)
				return new string[0];

			return Marshalling.GetString(ResourceManager_GetAllFilenames(m_ResourceManagerPtr, filenameRegexStr, accessArchives)).Split('|');
		}

		/// <summary>
		/// Returns an empty string if file is not located in an archive file in the ResourceManager's working directory.
		/// </summary>
		public string FindContainingArchivePath(string filename)
		{
			if (m_ResourceManagerPtr == null)
				return null;

			return Marshalling.GetString(ResourceManager_FindContainingArchivePath(m_ResourceManagerPtr, filename));
		}

		/// <summary>
		/// Returns a list of all loaded archives
		/// </summary>
		public string[] GetArchiveFilenames()
		{
			if (m_ResourceManagerPtr == null)
				return new string[0];

			return Marshalling.GetString(ResourceManager_GetArchiveFilenames(m_ResourceManagerPtr)).Split('|');
		}

		public ulong GetResourceSize(string filename, bool accessArchives)
			{
			if (m_ResourceManagerPtr == null)
				return 0;

			return ResourceManager_GetResourceSize(m_ResourceManagerPtr, filename, accessArchives);
			}

		public byte[] GetResource(string filename, bool accessArchives)
		{
			if (m_ResourceManagerPtr == null)
				return null;

			int size = (int)ResourceManager_GetResourceSize(m_ResourceManagerPtr, filename, accessArchives);
			if (size == 0) return null;

			IntPtr buffer = Marshal.AllocHGlobal(size);

			if (!ResourceManager_GetResource(m_ResourceManagerPtr, filename, accessArchives, buffer))
			{
				Marshal.FreeHGlobal(buffer);
				return null;
			}

			byte[] file = new byte[size];
			Marshal.Copy(buffer, file, 0, size);

			Marshal.FreeHGlobal(buffer);

			return file;
		}


		[DllImport(Platform.DLLPath, CallingConvention=CallingConvention.Cdecl)] private static extern IntPtr ResourceManager_Create(string archiveDirectory);
		[DllImport(Platform.DLLPath, CallingConvention=CallingConvention.Cdecl)] private static extern void ResourceManager_Release(IntPtr resourceManager);

		[DllImport(Platform.DLLPath, CallingConvention=CallingConvention.Cdecl)] private static extern IntPtr ResourceManager_GetAllFilenames(IntPtr resourceManager, string filenameRegexStr, bool accessArchives);
		[DllImport(Platform.DLLPath, CallingConvention=CallingConvention.Cdecl)] private static extern IntPtr ResourceManager_GetAllFilenamesOfType(IntPtr resourceManager, string extension, bool accessArchives);

		[DllImport(Platform.DLLPath, CallingConvention=CallingConvention.Cdecl)] private static extern IntPtr ResourceManager_FindContainingArchivePath(IntPtr resourceManager, string filename);

		[DllImport(Platform.DLLPath, CallingConvention=CallingConvention.Cdecl)] private static extern IntPtr ResourceManager_GetArchiveFilenames(IntPtr resourceManager);
		[DllImport(Platform.DLLPath, CallingConvention=CallingConvention.Cdecl)] private static extern ulong ResourceManager_GetResourceSize(IntPtr resourceManager, string filename, bool accessArchives);
		[DllImport(Platform.DLLPath, CallingConvention=CallingConvention.Cdecl)] private static extern bool ResourceManager_GetResource(IntPtr resourceManager, string filename, bool accessArchives, IntPtr buffer);
	}
}
