using OP2UtilityDotNet.Archive;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace OP2UtilityDotNet
{
	// Use to find files/resources either on disk or contained in an archive file (.vol|.clm).
	public class ResourceManager : System.IDisposable
	{
		public ResourceManager(string archiveDirectory)
		{
			resourceRootDir = archiveDirectory;

			if (!Directory.Exists(archiveDirectory)) {
				throw new System.Exception("Resource manager must be passed an archive directory.");
			}

			List<string> volFilenames = GetFilesFromDirectory("*.vol");

			foreach (string volFilename in volFilenames) {
				ArchiveFiles.Add(new VolFile(Path.Combine(archiveDirectory, volFilename)));
			}

			List<string> clmFilenames = GetFilesFromDirectory("*.clm");

			foreach (string clmFilename in clmFilenames) {
				ArchiveFiles.Add(new ClmFile(Path.Combine(archiveDirectory, clmFilename)));
			}
		}

		// First searches for resources loosely in provided directory.
		// Then, if accessArhives = true, searches the preloaded archives for the resource.
		public byte[] GetResource(string filename, bool accessArchives = true)
		{
			using (Stream stream = GetResourceStream(filename, accessArchives))
			using (BinaryReader reader = new BinaryReader(stream))
			{
				return reader.ReadBytes((int)stream.Length);
			}
		}

		// First searches for resources loosely in provided directory.
		// Then, if accessArhives = true, searches the preloaded archives for the resource.
		public Stream GetResourceStream(string filename, bool accessArchives = true)
		{
			// Only fully relative paths are supported
			if (Path.IsPathRooted(filename)) {
				throw new System.Exception("ResourceManager only accepts fully relative paths. Refusing: " + filename);
			}

			// Relative paths are relative to resourceRootDir
			string path = Path.Combine(resourceRootDir, filename);
			if (File.Exists(path)) {
				return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			}

			if (!accessArchives) {
				return null;
			}

			foreach (ArchiveFile archiveFile in ArchiveFiles)
			{

				if (archiveFile.Contains(filename)) {
					int index = archiveFile.GetIndex(filename);
					return archiveFile.OpenStream(index);
				}
			}

			return null;
		}

		public List<string> GetAllFilenames(string filenameRegexStr, bool accessArchives = true)
		{
			string filenameRegex = filenameRegexStr;

			List<string> filenames = GetFilesFromDirectoryRegex(filenameRegex);

			if (!accessArchives) {
				return filenames;
			}

			foreach (ArchiveFile archiveFile in ArchiveFiles)
			{
				for (int i = 0; i < archiveFile.GetCount(); ++i)
				{
					if (Regex.IsMatch(archiveFile.GetName(i), filenameRegex, RegexOptions.IgnoreCase)) {
						filenames.Add(archiveFile.GetName(i));
					}
				}
			}

			return filenames;
		}

		public List<string> GetAllFilenamesOfType(string extension, bool accessArchives = true)
		{
			List<string> filenames = GetFilesFromDirectory("*." + extension);

			if (!accessArchives) {
				return filenames;
			}

			foreach (ArchiveFile archiveFile in ArchiveFiles)
			{
				for (int i = 0; i < archiveFile.GetCount(); ++i)
				{
					string internalFilename = archiveFile.GetName(i);

					if (string.Compare(Path.GetExtension(internalFilename), Path.GetExtension(extension), true) == 0 && !IsDuplicateFilename(filenames, internalFilename)) {
						filenames.Add(internalFilename);
					}
				}
			}

			return filenames;
		}

		// Returns an empty string if file is not located in an archive file in the ResourceManager's working directory.
		public string FindContainingArchivePath(string filename)
		{
			foreach (ArchiveFile archiveFile in ArchiveFiles)
			{
				if (archiveFile.Contains(filename)) {
					return archiveFile.GetArchiveFilename();
				}
			}

			return "";
		}

		// Returns a list of all loaded archives
		public List<string> GetArchiveFilenames()
		{
			List<string> archiveFilenames = new List<string>(ArchiveFiles.Count);

			foreach (ArchiveFile archiveFile in ArchiveFiles)
			{
				archiveFilenames.Add(archiveFile.GetArchiveFilename());
			}
			return archiveFilenames;
		}

		private string resourceRootDir;
		private List<Archive.ArchiveFile> ArchiveFiles = new List<Archive.ArchiveFile>();

		private bool ExistsInArchives(string filename, out int archiveIndexOut, out int internalIndexOut)
		{
			archiveIndexOut = -1;
			internalIndexOut = -1;

			for (int i = 0; i < ArchiveFiles.Count; ++i)
			{
				for (int j = 0; j < ArchiveFiles[i].GetCount(); ++j)
				{
					if (string.Compare(ArchiveFiles[i].GetName(j), filename, true) == 0) // NOTE: Case insensitive comparison
					{
						archiveIndexOut = i;
						internalIndexOut = j;
						return true;
					}
				}
			}

			return false;
		}

		private bool IsDuplicateFilename(List<string> currentFilenames, string filenameToCheck)
		{
			// Brett: When called on a large loop of filenames (60 or more), this function will create a bottleneck.

			foreach (string currentFilename in currentFilenames) {
				if (string.Compare(Path.GetFileName(currentFilename), filenameToCheck, true) == 0) // NOTE: Case insensitive comparison
				{
					return true;
				}
			}

			return false;
		}

		// Returns only files from the directory
		private List<string> GetFilesFromDirectory(string searchPattern)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(resourceRootDir);
			FileInfo[] fileInfos = dirInfo.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);

			List<string> files = new List<string>(fileInfos.Length);

			foreach (FileInfo fileInfo in fileInfos)
			{
				files.Add(fileInfo.Name);
			}

			return files;
		}

		private List<string> GetFilesFromDirectoryRegex(string filenameRegex)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(resourceRootDir);
			FileInfo[] fileInfos = dirInfo.GetFiles();

			List<string> files = new List<string>(fileInfos.Length);

			foreach (FileInfo fileInfo in fileInfos)
			{
				if (Regex.IsMatch(fileInfo.Name, filenameRegex, RegexOptions.IgnoreCase))
					files.Add(fileInfo.Name);
			}

			return files;
		}

		public void Dispose()
		{
			foreach (ArchiveFile archive in ArchiveFiles)
				archive.Dispose();

			ArchiveFiles.Clear();
		}
	}
}
