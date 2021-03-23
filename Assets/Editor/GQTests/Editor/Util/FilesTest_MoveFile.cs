using NUnit.Framework;
using GQ.Editor.Util;

namespace GQTests.Editor.Util
{

	public class FilesTest_MoveFile : FilesTest
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
		public void MoveAssetFile ()
		{
			// Arrange:
			Files.CreateDir (moveStartAssetDir);
			Files.CopyDirContents (originAssetDirWithFilesPath, moveStartAssetDir);
			Files.CreateDir (moveEndAssetDir);

			string toBeMovedFilePath = Files.CombinePath (moveStartAssetDir, "PlainTextDocument.txt");

			// Assert:
			string movedFilePath = Files.CombinePath (targetAssetDirPath, "PlainTextDocument.txt");
			Assert.That (Files.ExistsFile (movedFilePath), "File 'PlainTextDocument.txt' should have been moved to " + targetAssetDirPath);
			Assert.That (!Files.ExistsFile (toBeMovedFilePath), "File should not exist at its former path after it has been moved.");
		}

		[Test]
		public void MoveFileAndOverwrite ()
		{
			// Arrange:
			Files.CreateDir (moveStartAssetDir);
			Files.CopyDirContents (originAssetDirWithFilesPath, moveStartAssetDir);
			Files.CreateDir (moveEndAssetDir);

			string toBeMovedFilePath = Files.CombinePath (moveStartAssetDir, "PlainTextDocument.txt");

			string overwriteFilePath = 
				Files.CombinePath (GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins", "SomeFiles", "PlainTextDocument.txt");

			Files.CopyFile (overwriteFilePath, moveEndAssetDir);

			expectContentAtPath (
				"some plain text", 
				Files.CombinePath (moveEndAssetDir, "PlainTextDocument.txt"), 
				"Text 'non empty text file' should be placed to be overwritten.");
			expectContentAtPath ("non empty text file", toBeMovedFilePath, "Preparation for overwrite text not correct.");

			// Assert:
			Assert.That (Files.ExistsFile (Files.CombinePath (moveEndAssetDir, "PlainTextDocument.txt")), "File 'PlainTextDocument.txt' should have been moved to " + moveEndAssetDir);
			Assert.That (!Files.ExistsFile (toBeMovedFilePath), "File should not exist at its former path after it has been moved.");
			expectContentAtPath (
				"non empty text file", 
				Files.CombinePath (moveEndAssetDir, "PlainTextDocument.txt"), 
				"Text 'non empty text file' should have been overwritten by 'some plain text'.");
		}

		[Test]
		public void MoveNonExsitingAssetFile ()
		{
			// Arrange:
			Files.CreateDir (moveStartAssetDir);
			string toBeMovedFilePath = 
				Files.CombinePath (
					moveStartAssetDir, 
					"NonExsitingFile.txt");
			Files.CreateDir (moveEndAssetDir);

			// Act:
			bool isMoved = Files.MoveFile (toBeMovedFilePath, moveEndAssetDir);

			// Assert:
			Assert.IsFalse (
				isMoved, 
				"Moving a non-existing file should return false."
			);
			Assert.That (
				!Files.ExistsFile (
					Files.CombinePath (
						moveEndAssetDir,
						Files.FileName (toBeMovedFilePath))
				),
				"Moving a non-existing file should NOT create such a file at the target dir."		
			);
		}

		[Test]
		public void MoveFileToNonExistingDirWithoutOverriding ()
		{
			// Arrange:
			Files.CreateDir (moveStartAssetDir);
			Files.CopyDir (originAssetDirWithFilesPath, moveStartAssetDir);
			string toBeMovedFilePath = 
				Files.CombinePath (
					moveStartAssetDir, 
					Files.DirName (originAssetDirWithFilesPath), 
					"PlainTextDocument.txt");
			// We are NOT creating a target dir now hence the moveEndAssetDir wil lead to a non-existing dir:
			Assert.That (!Files.ExistsDir (moveEndAssetDir), "Target dir should not exist before move in this test.");

			// Act:
			bool isMoved = Files.MoveFile (toBeMovedFilePath, moveEndAssetDir, false);

			// Assert:
			Assert.IsFalse (
				isMoved, 
				"Moving a file to a non-existing dir which shall not be created should return false.");
			Assert.That (
				!Files.ExistsDir (moveEndAssetDir), 
				"Moving a file to a non-existing dir with replace = false should NOT create the target dir.");
			Assert.That (
				!Files.ExistsFile (Files.CombinePath (moveEndAssetDir, "PlainTextDocument.txt")), 
				"Moving a file to a non-existing dir with replace = false should NOT create the moved file at target");
			Assert.That (
				Files.ExistsFile (toBeMovedFilePath), 
				"Moving a file to a non-existing dir with replace = false should keep it at its original place.");
		}

		[Test]
		public void MoveFileToNonExistingDirAndCreateIt ()
		{
			// Arrange:
			Files.CreateDir (moveStartAssetDir);
			Files.CopyDir (originAssetDirWithFilesPath, moveStartAssetDir);
			string toBeMovedFilePath = 
				Files.CombinePath (
					moveStartAssetDir, 
					Files.DirName (originAssetDirWithFilesPath), 
					"PlainTextDocument.txt");
			// We are NOT creating a target dir now hence the moveEndAssetDir wil lead to a non-existing dir:
			Assert.That (!Files.ExistsDir (moveEndAssetDir), "Target dir should not exist before move in this test.");

			// Act:
			bool isMoved = Files.MoveFile (toBeMovedFilePath, moveEndAssetDir);

			// Assert:
			Assert.IsTrue (
				isMoved, 
				"Moving a file to a non-existing dir which can be created should return true.");
			Assert.That (
				Files.ExistsDir (moveEndAssetDir), 
				"Moving a file to a non-existing dir should create the target dir.");
			Assert.That (
				Files.ExistsFile (Files.CombinePath (moveEndAssetDir, "PlainTextDocument.txt")), 
				"Moving a file to a non-existing dir should create the moved file at target");
			Assert.That (
				!Files.ExistsFile (toBeMovedFilePath), 
				"Moving a file to a non-existing dir should delete it at its original place.");
		}

		private static string moveStartAssetDir = Files.CombinePath (targetAssetDirPath, "MoveStart");
		private string moveEndAssetDir = Files.CombinePath (targetAssetDirPath, "MoveEnd");

		private void prepareMoveAssetDir ()
		{
			// Arrange:
			Files.CreateDir (moveStartAssetDir);
			Files.CopyDir (originAssetDirWithFilesPath, moveStartAssetDir);
			Files.CreateDir (moveEndAssetDir);

			// Pre-Assert:
			expectOriginalDirStructureAtPath (moveStartAssetDir);
			Assert.That (Files.IsEmptyDir (moveEndAssetDir));
		}

		[Test]
		public void MoveDir_Asset_2_Asset ()
		{
			// Arrange & Pre-Assert:
			prepareMoveAssetDir ();
			string toBeMovedDirPath = Files.CombinePath (moveStartAssetDir, "NonEmptyDir");

			// Act:
			bool isMoved = Files.MoveDir (toBeMovedDirPath, moveEndAssetDir);

			// Assert:
			Assert.That (isMoved, "MoveDir() should return true.");
			expectOriginalDirStructureAtPath (moveEndAssetDir);
			Assert.That (Files.IsEmptyDir (moveStartAssetDir), "After move dir should not be anymore at start path: " + moveStartAssetDir);
		}

		[Test]
		public void MoveEmptyAssetDir ()
		{
			// Arrange:
			Files.CreateDir (moveStartAssetDir);
			Files.CopyDir (originAssetDirEmptyPath, moveStartAssetDir);
			Files.CreateDir (moveEndAssetDir);

			// Pre-Assert:
			expectEmptyDirSturctureAtPath (moveStartAssetDir);
			Assert.That (Files.IsEmptyDir (moveEndAssetDir));

			string toBeMovedDirPath = Files.CombinePath (moveStartAssetDir, "EmptyDir");

			// Act:
			bool isMoved = Files.MoveDir (toBeMovedDirPath, moveEndAssetDir);

			// Assert:
			Assert.That (isMoved, "MoveDir() should return true.");
			expectEmptyDirSturctureAtPath (moveEndAssetDir);
			Assert.That (Files.IsEmptyDir (moveStartAssetDir), "After move dir should not be anymore at start path: " + moveStartAssetDir);
		}

		[Test]
		public void MoveAssetDirAndOverride ()
		{
			// Arrange & Pre-Assert:
			prepareMoveAssetDir ();
			Files.CopyDir (originAssetDirEmptyPath, moveEndAssetDir);
			expectEmptyDirSturctureAtPath (moveEndAssetDir);
			string toBeMovedDirPath = Files.CombinePath (moveStartAssetDir, "NonEmptyDir");

			// Act:
			bool isMoved = Files.MoveDir (toBeMovedDirPath, moveEndAssetDir, true);

			// Assert:
			Assert.That (isMoved, "MoveDir() should return true.");
			expectOriginalDirStructureAtPath (moveEndAssetDir);
			Assert.That (Files.IsEmptyDir (moveStartAssetDir), "After move dir should not be anymore at start path: " + moveStartAssetDir);
		}

		[Test]
		public void MoveAssetDirAndDoNotOverride ()
		{
			// Arrange & Pre-Assert:
			prepareMoveAssetDir ();
			Files.CopyDir (originAssetDirEmptyPath, moveEndAssetDir);
			expectEmptyDirSturctureAtPath (moveEndAssetDir);
			string toBeMovedDirPath = Files.CombinePath (moveStartAssetDir, "EmptyDir");

			// Act:
			bool isMoved = Files.MoveDir (toBeMovedDirPath, moveEndAssetDir, false);

			// Assert:
			Assert.That (!isMoved, "MoveDir() should return false, since we did not override.");
			expectEmptyDirSturctureAtPath (moveEndAssetDir);
			expectOriginalDirStructureAtPath (moveStartAssetDir); // since we did not override nothing gets moved.
		}

		private static string moveStartNonAssetDir = Files.CombinePath (targetNonAssetDirPath, "MoveStart");
		private string moveEndNonAssetDir = Files.CombinePath (targetNonAssetDirPath, "MoveEnd");

		private void prepareMoveNonAssetDir ()
		{
			// Arrange:
			Files.CreateDir (moveStartNonAssetDir);
			Files.CopyDir (originNonAssetDirWithFilesPath, moveStartNonAssetDir);
			Files.CreateDir (moveEndNonAssetDir);

			// Pre-Assert:
			expectOriginalDirStructureAtPath (moveStartNonAssetDir);
			Assert.That (Files.IsEmptyDir (moveEndNonAssetDir));
		}

		[Test]
		public void MoveDir_NonAsset_2_NonAsset ()
		{
			// Arrange & Pre-Assert:
			prepareMoveNonAssetDir ();
			string toBeMovedDirPath = Files.CombinePath (moveStartNonAssetDir, "NonEmptyDir");

			// Act:
			bool isMoved = 
				Files.MoveDir (
					toBeMovedDirPath, 
					moveEndNonAssetDir
				);

			// Assert:
			Assert.That (
				isMoved, 
				"MoveDir() should return true."
			);
			expectOriginalDirStructureAtPath (moveEndNonAssetDir);
			Assert.That (
				Files.IsEmptyDir (moveStartNonAssetDir), 
				"After move dir should not be anymore at start path: " + moveStartNonAssetDir
			);
		}


	}
}
