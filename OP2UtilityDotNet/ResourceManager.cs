using System;
using System.Runtime.InteropServices;

namespace OP2UtilityDotNet
{
	public class ResourceManager : IDisposable
	{
		protected IntPtr m_ResourceManagerPtr;

		public ResourceManager(string archiveDirectory)									{ m_ResourceManagerPtr = ResourceManager_Create(archiveDirectory);							}
		public void Dispose()															{ ResourceManager_Release(m_ResourceManagerPtr);											}

		public string[] GetAllFilenames(string filenameRegexStr, bool accessArchives)
		{
			return Marshal.PtrToStringAnsi(ResourceManager_GetAllFilenames(m_ResourceManagerPtr, filenameRegexStr, accessArchives)).Split('|');
		}

		public string[] GetAllFilenamesOfType(string filenameRegexStr, bool accessArchives)
		{
			return Marshal.PtrToStringAnsi(ResourceManager_GetAllFilenames(m_ResourceManagerPtr, filenameRegexStr, accessArchives)).Split('|');
		}

		/// <summary>
		/// Returns an empty string if file is not located in an archive file in the ResourceManager's working directory.
		/// </summary>
		public string FindContainingArchivePath(string filename)
		{
			return Marshal.PtrToStringAnsi(ResourceManager_FindContainingArchivePath(m_ResourceManagerPtr, filename));
		}

		/// <summary>
		/// Returns a list of all loaded archives
		/// </summary>
		public string[] GetArchiveFilenames()
		{
			return Marshal.PtrToStringAnsi(ResourceManager_GetArchiveFilenames(m_ResourceManagerPtr)).Split('|');
		}

		public ulong GetResourceSize(string filename, bool accessArchives)				{ return ResourceManager_GetResourceSize(m_ResourceManagerPtr, filename, accessArchives);	}

		public byte[] GetResource(string filename, bool accessArchives)
		{
			int size = (int)ResourceManager_GetResourceSize(m_ResourceManagerPtr, filename, accessArchives);
			IntPtr buffer = Marshal.AllocHGlobal(size);

			ResourceManager_GetResource(m_ResourceManagerPtr, filename, accessArchives, buffer);
			byte[] file = new byte[size];
			Marshal.Copy(buffer, file, 0, size);

			Marshal.FreeHGlobal(buffer);

			return file;
		}


		[DllImport(Platform.DLLPath)] private static extern IntPtr ResourceManager_Create(string archiveDirectory);
		[DllImport(Platform.DLLPath)] private static extern void ResourceManager_Release(IntPtr resourceManager);

		[DllImport(Platform.DLLPath)] private static extern IntPtr ResourceManager_GetAllFilenames(IntPtr resourceManager, string filenameRegexStr, bool accessArchives);
		[DllImport(Platform.DLLPath)] private static extern IntPtr ResourceManager_GetAllFilenamesOfType(IntPtr resourceManager, string extension, bool accessArchives);

		[DllImport(Platform.DLLPath)] private static extern IntPtr ResourceManager_FindContainingArchivePath(IntPtr resourceManager, string filename);

		[DllImport(Platform.DLLPath)] private static extern IntPtr ResourceManager_GetArchiveFilenames(IntPtr resourceManager);
		[DllImport(Platform.DLLPath)] private static extern ulong ResourceManager_GetResourceSize(IntPtr resourceManager, string filename, bool accessArchives);
		[DllImport(Platform.DLLPath)] private static extern void ResourceManager_GetResource(IntPtr resourceManager, string filename, bool accessArchives, IntPtr buffer);
	}
}
