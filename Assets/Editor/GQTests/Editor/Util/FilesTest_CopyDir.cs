using NUnit.Framework;
using GQ.Editor.Util;

namespace GQTests.Editor.Util {

	public class FilesTest_CopyDir : FilesTest {

		#region Helpers

		[SetUp]
		public void setUp () {
			ensureCleanTargets();
		}

		[TearDown]
		public void tearDown () {
			ensureCleanTargets();
		}

		private void prepareTargetDirToBeOverridden () {
			Files.CopyDir(originAssetDirWithFilesPath, targetAssetDirPath, true);
			Assert.That(Files.ExistsDir(targetAssetDirPath));
			expectOriginalDirStructureAtPath(targetAssetDirPath);

			Files.CopyDir(originNonAssetDirWithFilesPath, targetNonAssetDirPath, true);
			Assert.That(Files.ExistsDir(targetNonAssetDirPath));
			expectOriginalDirStructureAtPath(targetNonAssetDirPath);
		}

		#endregion


		#region Dir Asset -> Asset

		[Test]
		public void CopyDir_Asset_2_Asset_Empty () {
			// Act:
			bool isCopied = Files.CopyDir(originAssetDirEmptyPath, targetAssetDirPath);

			// Assert:
			Assert.That(isCopied, "CopyDir() should return true.");
			Assert.That(GQ.Editor.Util.Assets.ExistsAssetAtPath(targetAssetDirPath));
			expectEmptyDirSturctureAtPath(targetAssetDirPath);
		}

		[Test]
		public void CopyDir_Asset_2_Asset_NonEmpty () {
			// Act:
			bool isCopied = Files.CopyDir(originAssetDirWithFilesPath, targetAssetDirPath);

			// Assert:
			Assert.That(isCopied, "CopyDir() should return true.");
			Assert.That(GQ.Editor.Util.Assets.ExistsAssetAtPath(targetAssetDirPath));
			expectOriginalDirStructureAtPath(Files.CombinePath(targetAssetDirPath));
		}

		[Test]
		public void CopyDir_Asset_2_Asset_OverwriteFalse () {
			// Prepare:
			prepareTargetDirToBeOverridden();

			// Act:
			Files.CopyDir(newAssetDirPath, targetAssetDirPath, false);

			// Assert:
			expectOriginalDirStructureAtPath(targetAssetDirPath);
		}

		[Test]
		public void CopyDir_Asset_2_Asset_OverwriteTrue () {
			prepareTargetDirToBeOverridden();

			// Act:
			Files.CopyDir(newAssetDirPath, targetAssetDirPath, true);

			// Assert:
			expectNewDirStructureAtPath(targetAssetDirPath);
		}

		[Test]
		public void CopyDir_Asset_2_Asset_OverwriteDefault () {
			prepareTargetDirToBeOverridden();

			// Act:
			Files.CopyDir(newAssetDirPath, targetAssetDirPath);

			// Assert:
			expectNewDirStructureAtPath(targetAssetDirPath);
		}

		[Test]
		public void CopyDirContents_Asset_2_Asset_NonEmpty () {
			// Act:
			bool isCopied = Files.CopyDirContents(originAssetDirWithFilesPath, targetAssetDirPath);

			// Assert:
			Assert.That(isCopied, "CopyDir() should return true.");
			Assert.That(GQ.Editor.Util.Assets.ExistsAssetAtPath(targetAssetDirPath));
			expectOriginalDirContentInDir(targetAssetDirPath);
		}

		#endregion


		#region Dir Asset -> Non-Asset

		[Test]
		public void CopyDir_Asset_2_NonAsset_Empty () {
			// Act:
			bool isCopied = Files.CopyDir(originAssetDirEmptyPath, targetNonAssetDirPath);

			// Assert:
			Assert.That(isCopied, "CopyDir() should return true.");
			string expectedEmptyDirPath = Files.CombinePath(targetNonAssetDirPath, "EmptyDir");
			Assert.That(Files.ExistsDir(expectedEmptyDirPath), "There should exist a dir at " + expectedEmptyDirPath);
			Assert.That(Files.IsEmptyDir(expectedEmptyDirPath), "The dir at " + expectedEmptyDirPath + " should be empty.");
		}

		[Test]
		public void CopyDir_Asset_2_NonAsset_NonEmpty () {
			// Act:
			bool isCopied = Files.CopyDir(originAssetDirWithFilesPath, targetNonAssetDirPath);

			// Assert:
			Assert.That(isCopied, "CopyDir() should return true.");
			Assert.That(Files.ExistsDir(targetNonAssetDirPath), "There should exist a dir at " + targetNonAssetDirPath);
			expectOriginalDirStructureAtPath(targetNonAssetDirPath);
		}

		[Test]
		public void CopyDir_Asset_2_NonAsset_OverwriteFalse () {
			// Prepare:
			prepareTargetDirToBeOverridden();

			// Act:
			Files.CopyDir(newAssetDirPath, targetNonAssetDirPath, false);

			// Assert:
			expectOriginalDirStructureAtPath(targetNonAssetDirPath);
		}

		[Test]
		public void CopyDir_Asset_2_NonAsset_OverwriteTrue () {
			prepareTargetDirToBeOverridden();

			// Act:
			Files.CopyDir(newAssetDirPath, targetNonAssetDirPath, true);

			// Assert:
			expectNewDirStructureAtPath(targetNonAssetDirPath);
		}

		[Test]
		public void CopyDir_Asset_2_NonAsset_OverwriteDefault () {
			prepareTargetDirToBeOverridden();

			// Act:
			Files.CopyDir(newAssetDirPath, targetNonAssetDirPath);

			// Assert:
			expectNewDirStructureAtPath(targetNonAssetDirPath);
		}

		#endregion


		#region Dir Non-Asset -> Asset

		[Test]
		public void CopyDir_NonAsset_2_Asset_Empty () {
			// Act:
			var isCopied = Files.CopyDir(originNonAssetDirEmptyPath, targetAssetDirPath);

			// Assert:
			Assert.That(isCopied, "CopyDir() should return true.");
			var expectedEmptyDirPath = Files.CombinePath(targetAssetDirPath, "EmptyDir");
			Assert.That(GQ.Editor.Util.Assets.ExistsAssetAtPath(expectedEmptyDirPath), "There should exist a dir at " + expectedEmptyDirPath);
			Assert.That(Files.IsEmptyDir(expectedEmptyDirPath), "The dir at " + expectedEmptyDirPath + " should be empty.");
		}

		[Test]
		public void CopyDir_NonAsset_2_Asset_NonEmpty () {
			// Act:
			bool isCopied = Files.CopyDir(originNonAssetDirWithFilesPath, targetAssetDirPath);

			// Assert:
			Assert.That(isCopied, "CopyDir() should return true.");
			Assert.That(
				GQ.Editor.Util.Assets.ExistsAssetAtPath(targetAssetDirPath), 
				"There should exist a dir at " + targetAssetDirPath);
			expectOriginalDirStructureAtPath(targetAssetDirPath);
		}

		[Test]
		public void CopyDir_NonAsset_2_Asset_OverwriteFalse () {
			// Prepare:
			prepareTargetDirToBeOverridden();

			// Act:
			Files.CopyDir(newNonAssetDirPath, targetAssetDirPath, false);

			// Assert:
			expectOriginalDirStructureAtPath(targetAssetDirPath);
		}

		[Test]
		public void CopyDir_NonAsset_2_Asset_OverwriteTrue () {
			prepareTargetDirToBeOverridden();

			// Act:
			Files.CopyDir(newNonAssetDirPath, targetAssetDirPath, true);

			// Assert:
			expectNewDirStructureAtPath(targetAssetDirPath);
		}

		[Test]
		public void CopyDir_NonAsset_2_Asset_OverwriteDefault () {
			prepareTargetDirToBeOverridden();

			// Act:
			Files.CopyDir(newNonAssetDirPath, targetAssetDirPath);

			// Assert:
			expectNewDirStructureAtPath(targetAssetDirPath);
		}

		#endregion


		#region Dir Non-Asset -> Non-Asset

		[Test]
		public void CopyDir_NonAsset_2_NonAsset_Empty () {
			// Act:
			bool isCopied = Files.CopyDir(originNonAssetDirEmptyPath, targetNonAssetDirPath);

			// Assert:
			Assert.That(isCopied, "CopyDir() should return true.");
			string expectedEmptyDirPath = Files.CombinePath(targetNonAssetDirPath, "EmptyDir");
			Assert.That(Files.ExistsDir(expectedEmptyDirPath), "There should exist a dir at " + expectedEmptyDirPath);
			Assert.That(Files.IsEmptyDir(expectedEmptyDirPath), "The dir at " + expectedEmptyDirPath + " should be empty.");
		}

		[Test]
		public void CopyDir_NonAsset_2_NonAsset_NonEmpty () {
			// Act:
			bool isCopied = Files.CopyDir(originNonAssetDirWithFilesPath, targetNonAssetDirPath);

			// Assert:
			Assert.That(isCopied, "CopyDir() should return true.");
			Assert.That(Files.ExistsDir(targetNonAssetDirPath), "There should exist a dir at " + targetNonAssetDirPath);
			expectOriginalDirStructureAtPath(targetNonAssetDirPath);
		}

		[Test]
		public void CopyDir_NonAsset_2_NonAsset_OverwriteFalse () {
			// Prepare:
			prepareTargetDirToBeOverridden();

			// Act:
			Files.CopyDir(newNonAssetDirPath, targetNonAssetDirPath, false);

			// Assert:
			expectOriginalDirStructureAtPath(targetNonAssetDirPath);
		}

		[Test]
		public void CopyDir_NonAsset_2_NonAsset_OverwriteTrue () {
			prepareTargetDirToBeOverridden();

			// Act:
			Files.CopyDir(newNonAssetDirPath, targetNonAssetDirPath, true);

			// Assert:
			expectNewDirStructureAtPath(targetNonAssetDirPath);
		}

		[Test]
		public void CopyDir_NonAsset_2_NonAsset_OverwriteDefault () {
			prepareTargetDirToBeOverridden();

			// Act:
			Files.CopyDir(newNonAssetDirPath, targetNonAssetDirPath);

			// Assert:
			expectNewDirStructureAtPath(targetNonAssetDirPath);
		}

		#endregion

		[Test, Ignore("todo")]
		public void CopyDirKeepsSisterDirs () {
			// Arrange:
			// TODO

			// Act:
			// TODO

			// Assert:
			// TODO
			Assert.Fail("Test not yet implemented!");
		}
	}
}