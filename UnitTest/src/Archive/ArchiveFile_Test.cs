using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.Archive;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTest.src.Archive
{
	[TestClass]
	public class ArchiveFile_Test
	{
		[TestMethod]
		public void VolFile_EmptyArchive()
		{
			const string archiveFilename = "EmptyArchive.vol";

			{
				// Create an empty archive
				List<string> filesToPack = new List<string>();
				VolFile.CreateArchive(archiveFilename, filesToPack);

				using (VolFile archiveFile = new VolFile(archiveFilename))
				{
					//SCOPED_TRACE("Empty Volume File");
					TestEmptyArchive(archiveFile, archiveFilename);

					Assert.ThrowsException<ArgumentOutOfRangeException>(() => archiveFile.GetCompressionCode(0));
				}
			}

			File.Delete(archiveFilename);
		}

		[TestMethod]
		public void ClmFile_EmptyArchive()
		{
			const string archiveFilename = "EmptyArchive.clm";

			{
				// Create an empty archive
				List<string> filesToPack = new List<string>();
				ClmFile.CreateArchive(archiveFilename, filesToPack);

				using (ClmFile archiveFile = new ClmFile(archiveFilename))
				{
					//SCOPED_TRACE("Empty CLM File");
					TestEmptyArchive(archiveFile, archiveFilename);
				}
			}

			File.Delete(archiveFilename);
		}

		// Deletes any files extracted during the test
		void TestEmptyArchive(ArchiveFile archiveFile, string archiveFilename)
		{
			const string extractDirectory = "./TestExtract";

			Assert.ThrowsException<Exception>(() => archiveFile.GetIndex("TestItem"));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => archiveFile.GetName(0));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => archiveFile.GetSize(0));
			Assert.IsFalse(archiveFile.Contains("TestItem"));

			Assert.ThrowsException<ArgumentOutOfRangeException>(() => archiveFile.OpenStream(0));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => archiveFile.ExtractFile(0, "Test"));
			archiveFile.ExtractAllFiles(extractDirectory);

			Assert.AreEqual(archiveFilename, archiveFile.GetArchiveFilename());

			Assert.AreEqual(0, archiveFile.GetCount());
			Assert.IsTrue(0 <= archiveFile.GetArchiveFileSize());

			File.Delete(extractDirectory);
		}
	}
}

