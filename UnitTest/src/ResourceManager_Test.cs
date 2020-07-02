using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet;
using OP2UtilityDotNet.Archive;

namespace UnitTest.src
{
	[TestClass]
	public class ResourceManager_Test
	{
		[TestMethod]
		public void ConstructResourceManager()
		{
			// Load Resource Manager where directories exist with .vol and .clm 'extensions'
			ResourceManager rm = new ResourceManager("./src/data");
			rm.Dispose();

			// Refuse to load when passed a filename
			Assert.ThrowsException<Exception>(() => new ResourceManager("./data/Empty.txt").Dispose());
		}

		[TestMethod]
		public void GetResourceStreamExistingFileUnpacked()
		{
			using (ResourceManager resourceManager = new ResourceManager("./src/data"))
			{
				// Opening an existing file should result in a valid stream
				Assert.AreNotEqual(null, resourceManager.GetResourceStream("Empty.txt"));
			}
		}

		[TestMethod]
		public void GetFilenames()
		{
			// Ensure Directory.vol is not returned
			using (ResourceManager resourceManager = new ResourceManager("./src/data"))
			{
				List<string> filenames = resourceManager.GetAllFilenamesOfType(".vol");

				Assert.AreEqual(0, filenames.Count);

				Assert.AreEqual(0, resourceManager.GetAllFilenames("Directory.vol").Count);
			}
		}

		[TestMethod]
		public void GetArchiveFilenames()
		{
			string archiveName = "./Test.vol";
			VolFile.CreateArchive(archiveName, new string[0]);

			// Scope block to ensures ResourceManager is destructed after use
			// This ensures the VOL file is closed before attempting to delete it
			// This is needed for Windows filesystem semantics
			using (ResourceManager resourceManager = new ResourceManager("./"))
			{
				CollectionAssert.AreEqual(new string[] { archiveName }, resourceManager.GetArchiveFilenames());
			}

			File.Delete(archiveName);
		}
	}
}
