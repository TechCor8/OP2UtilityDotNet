using System.Collections.Generic;
using System.IO;

namespace OP2UtilityDotNet.Archive
{
	public abstract class ArchiveFile : System.IDisposable
	{
		public ArchiveFile(string filename)
		{
			m_ArchiveFilename = filename;
			m_Count = 0;
			m_ArchiveFileSize = 0;
		}

		public string GetArchiveFilename() { return m_ArchiveFilename; }
		public long GetArchiveFileSize() { return m_ArchiveFileSize; }
		public int GetCount() { return m_Count; }
		public bool Contains(string name)
		{
			for (int i = 0; i < GetCount(); ++i)
			{
				if (string.Compare(GetName(i), name, true) == 0) // NOTE: Case insensitive comparison
				{
					return true;
				}
			}

			return false;
		}

		public void ExtractFile(string name, string pathOut)
		{
			ExtractFile(GetIndex(name), pathOut);
		}

		public virtual int GetIndex(string name)
		{
			for (int i = 0; i < GetCount(); ++i)
			{
				if (string.Compare(GetName(i), name, true) == 0) // NOTE: Case insensitive comparison
				{
					return i;
				}
			}

			throw new System.Exception("Archive " + m_ArchiveFilename + " does not contain " + name);
		}

		public abstract string GetName(int index);
		public abstract int GetSize(int index);
		public abstract void ExtractFile(int index, string pathOut);
		public virtual void ExtractAllFiles(string destDirectory)
		{
			for (int i = 0; i < GetCount(); ++i)
			{
				ExtractFile(i, Path.Combine(destDirectory, GetName(i)));
			}
		}
		public abstract Stream OpenStream(int index);
		public virtual Stream OpenStream(string name)
		{
			return OpenStream(GetIndex(name));
		}

		protected void VerifyIndexInBounds(int index)
		{
			if (index >= m_Count)
			{
				throw new System.ArgumentOutOfRangeException("Index " + index + " is out of bounds in archive " + m_ArchiveFilename + ".");
			}
		}

		// Returns the filenames from each path stripping the rest of the path.
		protected static List<string> GetNamesFromPaths(List<string> paths)
		{
			List<string> filenames = new List<string>();

			foreach (string filename in paths)
			{
				filenames.Add(Path.GetFileName(filename));
			}

			return filenames;
		}

		// Throws an error if 2 names are identical, case insensitve.
		// names must be presorted.
		protected static void VerifySortedContainerHasNoDuplicateNames(List<string> names)
		{
			for (int i = 1; i < names.Count; ++i)
			{
				if (string.Equals(names[i-1], names[i]))
				{
					throw new System.Exception("Unable to create an archive containing files with the same name. Duplicate name: " + names[i]);
				}
			}
		}

		// Compares 2 filenames case insensitive to determine which comes first alphabetically.
		// Does not compare the entire path, but only the filename.
		protected static bool ComparePathFilenames(string path1, string path2)
		{
			return string.Equals(Path.GetFileName(path1), Path.GetFileName(path2), System.StringComparison.InvariantCultureIgnoreCase);
		}

		public abstract void Dispose();

		protected string m_ArchiveFilename;
		protected int m_Count;
		protected long m_ArchiveFileSize;
	}
}
