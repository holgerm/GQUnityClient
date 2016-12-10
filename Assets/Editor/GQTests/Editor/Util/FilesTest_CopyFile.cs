using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Editor.Util;
using GQTests;
using System.IO;

namespace GQTests.Editor.Util {

	/// <summary>
	/// Test for Copy functionality in the Files utility class.
	/// 
	/// The CopyFile() method has three parameters:
	/// 
	/// string fromFilePath must point to an existing file, hence we test cases where it:
	/// - points to an existing file (normal, working case)
	/// - points to a non existing file (does not work, returns false)
	/// 
	/// string toFilePath points to an existing directory. We test cases where it:
	/// - points to an exsiting directory (normal case, creates the file within that directory)
	/// - does not point to an exsting directory : we try to create the directory and give up if that fails.
	/// </summary>
	public class FilesTest_CopyFile : FilesTest {

		static string originAssetFilePath = 
			Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins", "SomeFiles", "PlainTextDocument.txt");
		static string newAssetFilePath = 
			Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins", "SomeFiles", "OtherPlainTextDocument.txt");
		//		static string targetAssetFilePath =
		//			Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Targets", "TargetTextDocument.txt");
		static string originNonAssetFilePath = 
			Files.CombinePath(GQAssert.PROJECT_PATH, "TestsData", "NonEmptyDir", "PlainTextDocument.txt");
		static string newNonAssetFilePath = 
			Files.CombinePath(GQAssert.PROJECT_PATH, "TestsData", "NonEmptyDir", "OtherPlainTextDocument.txt");
		//		static string targetNonAssetFilePath =
		//			Files.CombinePath(GQAssert.PROJECT_PATH, "TestsData", "Targets", "TargetTextDocument.txt");

		static string originFileContent = File.ReadAllText(originAssetFilePath);
		static string newFileContent = File.ReadAllText(newAssetFilePath);
		static string overwriteContent = 
			File.ReadAllText(
				Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins", "NonEmptyDir", "PlainTextDocument.txt"));


		[SetUp]
		public void setUp () {
			ensureCleanTargets();
		}

		[TearDown]
		public void tearDown () {
			ensureCleanTargets();
		}

		[Test]
		public void CopyFile_Asset_2_Asset () {
			// Arrange:
			string targetAssetFilePath = 
				Files.CombinePath(
					targetAssetDirPath, 
					Files.FileName(originAssetFilePath)
				);
			
			// Act:
			bool isCopied = Files.CopyFile(originAssetFilePath, targetAssetDirPath);

			// Assert:
			Assert.That(isCopied);
			Assert.That(Assets.ExistsAssetAtPath(targetAssetFilePath));
		}

		/// <summary>
		/// Copies a file ("FilesTest/Origins/SomeFiles/PlainTextDocument.txt") to the target dir. 
		/// It does so in similar manner for assets and non-assets.
		/// </summary>
		private void prepareTargetFileToBeOverridden () {
			string targetAssetFilePath = 
				Files.CombinePath(
					targetAssetDirPath, 
					Files.FileName(originAssetFilePath)
				);
			Files.CopyFile(
				originAssetFilePath, 
				targetAssetDirPath,
				true);				
			Assert.That(Files.ExistsFile(targetAssetFilePath));
			Assert.AreEqual(originFileContent, File.ReadAllText(targetAssetFilePath));

			string targetNonAssetFilePath = 
				Files.CombinePath(
					targetNonAssetDirPath, 
					Files.FileName(originNonAssetFilePath)
				);
			Files.CopyFile(originNonAssetFilePath, targetNonAssetDirPath, true);
			Assert.That(Files.ExistsFile(targetNonAssetFilePath));
			Assert.AreEqual(originFileContent, File.ReadAllText(targetNonAssetFilePath));
		}

		[Test]
		public void CopyFile_Asset_2_Asset_OverwriteFalse () {
			// Prepare:
			prepareTargetFileToBeOverridden();
			string targetAssetFilePath = 
				Files.CombinePath(
					targetAssetDirPath, 
					Files.FileName(originAssetFilePath)
				);
			

			// Act:
			Files.CopyFile(newAssetFilePath, targetAssetDirPath, false);

			// Assert:
			expectContentAtPath(originFileContent, targetAssetFilePath, "CopyFile with overwrite flag set to FALSE should NOT overwrite file.");
		}

		[Test]
		public void CopyFile_Asset_2_Asset_OverwriteTrue () {
			// Prepare:
			prepareTargetFileToBeOverridden();
			string targetAssetFilePath = 
				Files.CombinePath(
					targetAssetDirPath, 
					Files.FileName(newAssetFilePath)
				);

			// Act:
			Files.CopyFile(newAssetFilePath, targetAssetDirPath, true);

			// Assert:
			expectContentAtPath(newFileContent, targetAssetFilePath, "CopyFile with overwrite flag set to true should overwrite file.");
		}

		[Test]
		public void CopyFile_Asset_2_Asset_OverwriteDefault () {
			// Prepare:
			prepareTargetFileToBeOverridden();
			string targetAssetFilePath = 
				Files.CombinePath(
					targetAssetDirPath, 
					Files.FileName(newAssetFilePath)
				);

			// Act:
			Files.CopyFile(newAssetFilePath, targetAssetDirPath);

			// Assert:
			expectContentAtPath(newFileContent, targetAssetFilePath, "CopyFile with using default for overwrite flag should overwrite file.");
		}

		[Test]
		public void CopyFile_Asset_2_NonAsset () {
			// Prepare:
			string targetNonAssetFilePath = 
				Files.CombinePath(
					targetNonAssetDirPath, 
					Files.FileName(originAssetFilePath)
				);
			
			// Act:
			bool isCopied = Files.CopyFile(originAssetFilePath, targetNonAssetDirPath);

			// Assert:
			Assert.That(isCopied);
			Assert.That(File.Exists(targetNonAssetFilePath));
		}

		[Test]
		public void CopyFile_Asset_2_NonAsset_OverwriteFalse () {
			// Prepare:
			prepareTargetFileToBeOverridden();
			string targetNonAssetFilePath = 
				Files.CombinePath(
					targetNonAssetDirPath, 
					Files.FileName(originNonAssetFilePath)
				);

			// Act:
			Files.CopyFile(newAssetFilePath, targetNonAssetDirPath, false);

			// Assert:
			expectContentAtPath(originFileContent, targetNonAssetFilePath, "CopyFile with overwrite flag set to FALSE should NOT overwrite file.");
		}

		[Test]
		public void CopyFile_Asset_2_NonAsset_OverwriteTrue () {
			// Prepare:
			prepareTargetFileToBeOverridden();
			string targetNonAssetFilePath = 
				Files.CombinePath(
					targetNonAssetDirPath, 
					Files.FileName(newAssetFilePath)
				);
			
			// Act:
			Files.CopyFile(newAssetFilePath, targetNonAssetDirPath, true);

			// Assert:
			expectContentAtPath(newFileContent, targetNonAssetFilePath, "CopyFile with overwrite flag set to true should overwrite file.");
		}

		[Test]
		public void CopyFile_Asset_2_NonAsset_OverwriteDefault () {
			// Prepare:
			prepareTargetFileToBeOverridden();
			string targetNonAssetFilePath = 
				Files.CombinePath(
					targetNonAssetDirPath, 
					Files.FileName(newAssetFilePath)
				);
			
			// Act:
			Files.CopyFile(newAssetFilePath, targetNonAssetDirPath);

			// Assert:
			expectContentAtPath(newFileContent, targetNonAssetFilePath, "CopyFile with using default for overwrite flag should overwrite file.");
		}

		[Test]
		public void CopyFile_NonAsset_2_Asset () {
			// Prepare:
			string targetAssetFilePath = 
				Files.CombinePath(
					targetAssetDirPath, 
					Files.FileName(originNonAssetFilePath)
				);

			// Act:
			bool isCopied = Files.CopyFile(originNonAssetFilePath, targetAssetDirPath);

			// Assert:
			Assert.That(isCopied);
			Assert.That(Assets.ExistsAssetAtPath(targetAssetFilePath));
		}

		[Test]
		public void CopyFile_NonAsset_2_Asset_OverwriteFalse () {
			// Prepare:
			prepareTargetFileToBeOverridden();
			string targetAssetFilePath = 
				Files.CombinePath(
					targetAssetDirPath, 
					Files.FileName(originNonAssetFilePath)
				);

			// Act:
			Files.CopyFile(newNonAssetFilePath, targetAssetDirPath, false);

			// Assert:
			expectContentAtPath(originFileContent, targetAssetFilePath, "CopyFile with overwrite flag set to FALSE should NOT overwrite file.");
		}

		[Test]
		public void CopyFile_NonAsset_2_Asset_OverwriteTrue () {
			// Prepare:
			prepareTargetFileToBeOverridden();
			string targetAssetFilePath = 
				Files.CombinePath(
					targetAssetDirPath, 
					Files.FileName(newNonAssetFilePath)
				);
			
			// Act:
			Files.CopyFile(newNonAssetFilePath, targetAssetDirPath, true);

			// Assert:
			expectContentAtPath(newFileContent, targetAssetFilePath, "CopyFile with overwrite flag set to true should overwrite file.");
		}

		[Test]
		public void CopyFile_NonAsset_2_Asset_OverwriteDefault () {
			// Prepare:
			prepareTargetFileToBeOverridden();
			string targetAssetFilePath = 
				Files.CombinePath(
					targetAssetDirPath, 
					Files.FileName(newNonAssetFilePath)
				);

			// Act:
			Files.CopyFile(newNonAssetFilePath, targetAssetDirPath);

			// Assert:
			expectContentAtPath(newFileContent, targetAssetFilePath, "CopyFile with using default for overwrite flag should overwrite file.");
		}

		[Test]
		public void CopyFile_NonAsset_2_NonAsset () {
			// Prepare:
			string targetNonAssetFilePath = 
				Files.CombinePath(
					targetNonAssetDirPath, 
					Files.FileName(originNonAssetFilePath)
				);

			// Act:
			bool isCopied = Files.CopyFile(originNonAssetFilePath, targetNonAssetDirPath);

			// Assert:
			Assert.That(isCopied);
			Assert.That(File.Exists(targetNonAssetFilePath));
		}

		[Test]
		public void CopyFile_NonAsset_2_NonAsset_OverwriteFalse () {
			// Prepare:
			prepareTargetFileToBeOverridden();
			string targetNonAssetFilePath = 
				Files.CombinePath(
					targetNonAssetDirPath, 
					Files.FileName(originNonAssetFilePath)
				);

			// Act:
			Files.CopyFile(newNonAssetFilePath, targetNonAssetDirPath, false);

			// Assert:
			expectContentAtPath(originFileContent, targetNonAssetFilePath, "CopyFile with overwrite flag set to FALSE should NOT overwrite file.");
		}

		[Test]
		public void CopyFile_NonAsset_2_NonAsset_OverwriteTrue () {
			// Prepare:
			prepareTargetFileToBeOverridden();
			string targetNonAssetFilePath = 
				Files.CombinePath(
					targetNonAssetDirPath, 
					Files.FileName(newNonAssetFilePath)
				);

			// Act:
			Files.CopyFile(newNonAssetFilePath, targetNonAssetDirPath, true);

			// Assert:
			expectContentAtPath(newFileContent, targetNonAssetFilePath, "CopyFile with overwrite flag set to true should overwrite file.");
		}

		[Test]
		public void CopyFile_NonAsset_2_NonAsset_OverwriteDefault () {
			// Prepare:
			prepareTargetFileToBeOverridden();
			string targetNonAssetFilePath = 
				Files.CombinePath(
					targetNonAssetDirPath, 
					Files.FileName(newNonAssetFilePath)
				);
			
			// Act:
			Files.CopyFile(newNonAssetFilePath, targetNonAssetDirPath);

			// Assert:
			expectContentAtPath(newFileContent, targetNonAssetFilePath, "CopyFile with using default for overwrite flag should overwrite file.");
		}

		[Test]
		public void FileCopy_NonExistingFile () {
			// Arrange:
			string toBeCopiedFilePath = Files.CombinePath(originAssetDirEmptyPath, "NonExisting.File");

			// Act:
			bool isCopied = Files.CopyFile(toBeCopiedFilePath, targetAssetDirPath);

			// Assert:
			Assert.IsFalse(isCopied, "Non-existing file can not be copied.");
			Assert.That(Files.IsEmptyDir(targetAssetDirPath));
		}

		[Test]
		public void FileCopy_NonExistingTargetDir () {
			// Arrange:
			string nonExistingTargetDirPath = Files.CombinePath(targetAssetDirPath, "NewTargetSubdir");
			string targetFilePath = 
				Files.CombinePath(
					nonExistingTargetDirPath, 
					Files.FileName(originAssetFilePath)
				);

			// Pre-Assert:
			Assert.That(!Files.ExistsDir(nonExistingTargetDirPath));
			Assert.That(!Files.ExistsFile(targetFilePath));

			// Act:
			bool isCopied = Files.CopyFile(originAssetFilePath, nonExistingTargetDirPath);

			// Assert:
			Assert.That(isCopied);
			Assert.That(Files.ExistsDir(nonExistingTargetDirPath));
			Assert.That(File.Exists(targetFilePath));
		}

		[Test]
		public void FileCopy_OverrideExistingTargetFile () {
			// Arrange:
			prepareTargetFileToBeOverridden();
			string overwriteFilePath = 
				Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins", "NonEmptyDir", "PlainTextDocument.txt");
			string overwriteContent = File.ReadAllText(overwriteFilePath);

			// Pre-Assert:
			expectContentAtPath(
				"some plain text", 
				Files.CombinePath(targetAssetDirPath, "PlainTextDocument.txt"), 
				"Original file should be placed into target directory before we overwrite it by copyFile()"
			);

			// Act:
			Files.CopyFile(overwriteFilePath, targetAssetDirPath, true);

			// Assert:
			// TODO
			Assert.That(Files.ExistsFile(Files.CombinePath(targetAssetDirPath, "PlainTextDocument.txt")));
			expectContentAtPath(
				"non empty text file", 
				Files.CombinePath(targetAssetDirPath, "PlainTextDocument.txt"),
				"New file 'OtherPlainTextDocument.txt' should replace the original file at target dir after copyFile()"
			);
		}

		[Test]
		public void FileCopy_NotOverrideExistingTarget () {
			// Arrange:
			prepareTargetFileToBeOverridden();
			string overwriteFilePath = 
				Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins", "NonEmptyDir", "PlainTextDocument.txt");
			string overwriteContent = File.ReadAllText(overwriteFilePath);

			// Pre-Assert:
			expectContentAtPath(
				"some plain text", 
				Files.CombinePath(targetAssetDirPath, "PlainTextDocument.txt"), 
				"Original file should be placed into target directory before we overwrite it by copyFile()"
			);

			// Act:
			Files.CopyFile(overwriteFilePath, targetAssetDirPath, false);

			// Assert:
			// TODO
			Assert.That(Files.ExistsFile(Files.CombinePath(targetAssetDirPath, "PlainTextDocument.txt")));
			expectContentAtPath(
				"some plain text", 
				Files.CombinePath(targetAssetDirPath, "PlainTextDocument.txt"),
				"The original file at target dir should NOT have been replaced even after copyFile()"
			);
		}


	}
}