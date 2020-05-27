using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Editor.Util;
using System.IO;

namespace GQTests.Editor.Util
{

	public class FilesTest_DeleteClear : FilesTest
	{
	
		[SetUp]
		public void setUp ()
		{
			ensureCleanTargets ();
		}

		[TearDown]
		public void tearDown ()
		{
			ensureCleanTargets ();
		}

		[Test]
		public void ClearDir_Asset ()
		{
			// Arrange:
			Files.CopyDir (originAssetDirWithFilesPath, targetAssetDirPath);

			// Pre-Assert:
			expectOriginalDirStructureAtPath (targetAssetDirPath);

			// Assert:
			Assert.That (Files.IsEmptyDir (targetAssetDirPath), "Target should be an empty dir after ClearDir()");
		}

		[Test]
		public void ClearDir_Asset_NonExisting ()
		{
			// Arrange:
			string nonExistingAssetPath = Files.CombinePath (GQAssert.PROJECT_PATH, "NonExisting", "Path");
			Assert.IsFalse (GQ.Editor.Util.Assets.ExistsAssetAtPath (nonExistingAssetPath));

			// Act:
			bool cleared = Files.ClearDir (nonExistingAssetPath);

			// Assert:
			Assert.IsFalse (cleared, "Should not clear a non existing assets dir.");
		}


		[Test]
		public void ClearDir_NonAsset ()
		{
			// Arrange:
			string nonExistingNonAssetPath = Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "NonExisting", "Path");
			Assert.IsFalse (Directory.Exists (nonExistingNonAssetPath));

			// Act:
			bool cleared = Files.ClearDir (nonExistingNonAssetPath);

			// Assert:
			Assert.IsFalse (cleared, "Should not clear a non existing dir.");
		}

		[Test]
		public void ClearDir_NonAsset_NonExisting ()
		{
			// Arrange:
			Files.CopyDir (originNonAssetDirWithFilesPath, targetNonAssetDirPath);

			// Pre-Assert:
			expectOriginalDirStructureAtPath (targetNonAssetDirPath);

			// Assert:
			Assert.That (Files.IsEmptyDir (targetNonAssetDirPath), "Target should be an empty dir after ClearDir()");
		}

		[Test]
		public void DeleteDir_Asset ()
		{
			// Arrange:
			string pathToDirThatWillBeDeleted = 
				Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "WillBeDeleted");
			if (!GQ.Editor.Util.Assets.ExistsAssetAtPath (pathToDirThatWillBeDeleted))
				Files.CreateDir (pathToDirThatWillBeDeleted);
			string pathToSubDirThatWillBeDeleted = 
				Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "WillBeDeleted", "SubDirWillBeDeleted");
			if (!GQ.Editor.Util.Assets.ExistsAssetAtPath (pathToSubDirThatWillBeDeleted))
				Files.CreateDir (pathToDirThatWillBeDeleted);
			// TODO some files inside ...

			// Act:
			bool isDeleted = Files.DeleteDir (pathToDirThatWillBeDeleted);

			// Assert:
			Assert.That (isDeleted);
			Assert.That (!GQ.Editor.Util.Assets.ExistsAssetAtPath (pathToSubDirThatWillBeDeleted));
			Assert.That (!GQ.Editor.Util.Assets.ExistsAssetAtPath (pathToDirThatWillBeDeleted));
		}

		[Test]
		public void DeleteDir_Asset_NonExisting ()
		{
			// Arrange:
			string pathToDirNonExisting = 
				Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "NonExistingDir");
			if (GQ.Editor.Util.Assets.ExistsAssetAtPath (pathToDirNonExisting))
				Files.DeleteDir (pathToDirNonExisting);

			// Act:
			bool isDeleted = Files.DeleteDir (pathToDirNonExisting);

			// Assert:
			Assert.That (!isDeleted);
			Assert.That (!GQ.Editor.Util.Assets.ExistsAssetAtPath (pathToDirNonExisting));
		}

		[Test]
		public void DeleteDir_NonAsset ()
		{
			// Arrange:
			string pathToDirThatWillBeDeleted = 
				Files.CombinePath (GQAssert.PROJECT_PATH, "TestsData", "WillBeDeleted");
			if (!GQ.Editor.Util.Assets.ExistsAssetAtPath (pathToDirThatWillBeDeleted))
				Files.CreateDir (pathToDirThatWillBeDeleted);
			string pathToSubDirThatWillBeDeleted = 
				Files.CombinePath (GQAssert.PROJECT_PATH, "TestsData", "WillBeDeleted", "SubDirWillBeDeleted");
			if (!GQ.Editor.Util.Assets.ExistsAssetAtPath (pathToSubDirThatWillBeDeleted))
				Files.CreateDir (pathToDirThatWillBeDeleted);
			// TODO some files inside ...

			// Act:
			bool isDeleted = Files.DeleteDir (pathToDirThatWillBeDeleted);

			// Assert:
			Assert.That (isDeleted);
			Assert.That (!Directory.Exists (pathToSubDirThatWillBeDeleted));
			Assert.That (!Directory.Exists (pathToDirThatWillBeDeleted));
		}

		[Test]
		public void DeleteDir_NonAsset_NonExisting ()
		{
			// Arrange:
			string pathToDirNonExisting = 
				Files.CombinePath (GQAssert.PROJECT_PATH, "TestsData", "NonExistingDir");
			if (Directory.Exists (pathToDirNonExisting))
				Files.DeleteDir (pathToDirNonExisting);

			// Act:
			bool isDeleted = Files.DeleteDir (pathToDirNonExisting);

			// Assert:
			Assert.That (!isDeleted);
			Assert.That (!GQ.Editor.Util.Assets.ExistsAssetAtPath (pathToDirNonExisting));
		}

		[Test]
		public void DeleteFile_Asset ()
		{
			// Arrange:
			string originFilePath = 
				Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins", "SomeFiles", "file 1.rtf");
			Files.CopyFile (originFilePath, targetAssetDirPath);
			string fileToBeDeletedPath =
				Files.CombinePath (targetAssetDirPath, "file 1.rtf");

			// Pre-Assert:
			Assert.That (File.Exists (fileToBeDeletedPath));

			// Act:
			bool isDeleted = Files.DeleteFile (fileToBeDeletedPath);

			// Assert:
			Assert.That (!Files.ExistsFile (fileToBeDeletedPath));
			Assert.That (isDeleted);
		}

		[Test]
		public void DeleteFile_Asset_NonExisting ()
		{
			// Arrange:
			string fileToBeDeleted =
				Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Targets", "doesNot.exist");

			// Pre-Assert:
			Assert.That (!File.Exists (fileToBeDeleted));

			// Act:
			bool isDeleted = Files.DeleteFile (fileToBeDeleted);

			// Assert:
			// TODO
			Assert.That (!isDeleted);
			Assert.That (!Files.ExistsFile (fileToBeDeleted));
		}

		[Test]
		public void DeleteFile_NonAsset ()
		{
			// Arrange:
			string originFilePath = 
				Files.CombinePath (GQAssert.PROJECT_PATH, "TestsData", "NonEmptyDir", "PlainTextDocument.txt");
			Files.CopyFile (originFilePath, targetNonAssetDirPath);
			string fileToBeDeleted =
				Files.CombinePath (targetNonAssetDirPath, "PlainTextDocument.txt");

			// Pre-Assert:
			Assert.That (File.Exists (fileToBeDeleted));

			// Act:
			bool isDeleted = Files.DeleteFile (fileToBeDeleted);

			// Assert:
			// TODO
			Assert.That (isDeleted);
			Assert.That (!Files.ExistsFile (fileToBeDeleted));
		}

		[Test]
		public void DeleteFile_NonAsset_NonExisting ()
		{
			// Arrange:
			string fileToBeDeleted =
				Files.CombinePath (GQAssert.PROJECT_PATH, "TestsData", "Targets", "doesNot.exist");

			// Pre-Assert:
			Assert.That (!File.Exists (fileToBeDeleted));

			// Act:
			bool isDeleted = Files.DeleteFile (fileToBeDeleted);

			// Assert:
			// TODO
			Assert.That (!isDeleted);
			Assert.That (!Files.ExistsFile (fileToBeDeleted));
		}

	}
}
