using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Editor.Util;
using System.IO;
using System;

namespace GQTests.Editor.Util {

	public class FilesTest_Misc : FilesTest {

		#region Asset Agnostic API

		[Test]
		public void ExistsFile () {
			// Arrange:
			string existingAssetPath = 
				Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins", "SomeFiles", "file 1.rtf");
			string nonExistingAssetPath = 
				Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins", "Empty", "DoesNot.exist");
			string existingNonAssetPath = 
				Files.CombinePath(GQAssert.PROJECT_PATH, "TestsData", "NonEmptyDir", "Image.png");
			string nonExistingNonAssetPath = 
				Files.CombinePath(GQAssert.PROJECT_PATH, "TestsData", "EmptyDir", "DoesNot.exist");

			// Assert:
			Assert.That(Files.ExistsFile(existingAssetPath));
			Assert.That(!Files.ExistsFile(nonExistingAssetPath));
			Assert.That(Files.ExistsFile(existingNonAssetPath));
			Assert.That(!Files.ExistsFile(nonExistingNonAssetPath));
		}

		[Test]
		public void ExistsDir () {
			// Arrange:
			string existingAssetPath = 
				Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins", "SomeFiles");
			string nonExistingAssetPath = 
				Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins", "Empty", "DoesNotExist");
			string existingNonAssetPath = 
				Files.CombinePath(GQAssert.PROJECT_PATH, "TestsData", "NonEmptyDir");
			string nonExistingNonAssetPath = 
				Files.CombinePath(GQAssert.PROJECT_PATH, "TestsData", "EmptyDir", "DoesNotExist");

			// Assert:
			Assert.That(Files.ExistsDir(existingAssetPath));
			Assert.That(!Files.ExistsDir(nonExistingAssetPath));
			Assert.That(Files.ExistsDir(existingNonAssetPath));
			Assert.That(!Files.ExistsDir(nonExistingNonAssetPath));
		}

		[Test]
		public void CreateDir_Asset () {
			// Arrange:
			string pathToEmptyDir = Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "NewDir");
			if ( Assets.ExistsAssetAtPath(pathToEmptyDir) )
				Files.DeleteDir(pathToEmptyDir);

			// Act:
			bool isCreated = Files.CreateDir(pathToEmptyDir);

			// Assert:
			Assert.That(isCreated, "CreateDir should succeed on creating a new non existing asset dir.");
			Assert.That(Directory.Exists(pathToEmptyDir), "CreateDir should have created a new asset dir.");
			Assert.That(Assets.ExistsAssetAtPath(pathToEmptyDir), "New asset dir should be a valid asset.");

			// Clean:
			Files.DeleteDir(pathToEmptyDir);
		}

		[Test]
		public void CreateDir_Asset_AlreadyExisting () {
			// Arrange:
			Files.CopyDir(originAssetDirWithFilesPath, targetAssetDirPath);
			string existingDirPath = Files.CombinePath(targetAssetDirPath, Files.DirName(originAssetDirWithFilesPath));

			// Act:
			bool result = Files.CreateDir(existingDirPath);

			// Assert:
			Assert.That(result == false, "CreateDir should NOT succeed on creating an already existing asset dir.");
		}

		[Test]
		public void CreateDir_NonAsset () {
			// Arrange:
			string pathToNonExistingNonAssetsDir = 
				Files.CombinePath(GQAssert.PROJECT_PATH, "TestsData", "EmptyDir", "NewDir");

			// Pre-Assert:
			if ( Directory.Exists(pathToNonExistingNonAssetsDir) ) {
				Directory.Delete(pathToNonExistingNonAssetsDir);
			}
			Assert.That(!Directory.Exists(pathToNonExistingNonAssetsDir));

			// Act:
			bool isCreated = Files.CreateDir(pathToNonExistingNonAssetsDir);

			// Assert:
			Assert.That(isCreated);
			Assert.That(Directory.Exists(pathToNonExistingNonAssetsDir));

			// Clean:
			Directory.Delete(pathToNonExistingNonAssetsDir);
		}

		[Test]
		public void CreateDir_NonAsset_AlreadyExisting () {
			// Arrange:
			string pathToExistingNonAssetsDir = 
				Files.CombinePath(GQAssert.PROJECT_PATH, "TestsData", "EmptyDir");

			// Pre-Assert:
			Assert.That(Directory.Exists(pathToExistingNonAssetsDir));

			// Act:
			bool isCreated = Files.CreateDir(pathToExistingNonAssetsDir);

			// Assert:
			Assert.That(!isCreated, "Should not be able to create an already existing asset dir using Files.CreateDir()");
			Assert.That(Directory.Exists(pathToExistingNonAssetsDir));
		}

		#endregion

		#region Extensions

		[Test]
		public void StripExtension () {
			//Assert
			Assert.AreEqual("filename", Files.StripExtension("filename.txt"), 
				"Should erase a '.txt' extension.");

			Assert.AreEqual("filename", Files.StripExtension("filename"), 
				"Should keep filename without any extension as it has been.");

			Assert.AreEqual("filename.txt", Files.StripExtension("filename.txt.png"), 
				"Should erase only the last extension.");
			Assert.AreEqual("filename", Files.StripExtension("filename."), 
				"Should erase a trailing '.' from filename as empty extension.");

			Assert.AreEqual("filename.", Files.StripExtension("filename..txt"), 
				"Should erase a '.txt' extension but keep additional dots before extension.");

			Assert.AreEqual(".filename", Files.StripExtension(".filename.txt"), 
				"Should erase a '.txt' also if filename starts with dot and has additional extension.");

			Assert.AreEqual(".", Files.StripExtension("."), 
				"Should keep single dot as filename.");

			Assert.AreEqual("..", Files.StripExtension(".."), 
				"Should keep double dot as filename.");

			Assert.AreEqual(".txt", Files.StripExtension(".txt"), 
				"Should keep filename if it starts with '.' and has no more dots");
		}

		[Test]
		public void Extension () {
			// Assert:
			Assert.AreEqual("txt", Files.Extension("filename.txt"), 
				"Should extract 'txt' from '.txt' extension.");

			Assert.AreEqual("", Files.Extension("filename"),
				"Should extract '' from filename without any extension.");

			Assert.AreEqual("png", Files.Extension("filename.txt.png"), 
				"Should extract the last extension if there are multiple.");

			Assert.AreEqual("", Files.Extension("filename."), 
				"Should extract '' from filename ending on a dot.");

			Assert.AreEqual("txt", Files.Extension("filename..txt"), 
				"Should ignore multiple dots separating the extension.");

			Assert.AreEqual("txt", Files.Extension(".filename.txt"), 
				"Should also find the extension if filename additionally starts with a dot.");

			Assert.AreEqual("", Files.Extension("."), 
				"Filename '.' has no extension.");

			Assert.AreEqual("", Files.Extension(".."), 
				"Filename '..' has no extension.");

			Assert.AreEqual("", Files.Extension(".txt"), 
				"Should ignore the extension because the filename otherwise would be empty.");
		}

		[Test]
		public void ExtensionSeparator () {
			// Assert:
			Assert.AreEqual(".", Files.ExtensionSeparator("filename.txt"));

			Assert.AreEqual("", Files.ExtensionSeparator("filename"));

			Assert.AreEqual(".", Files.ExtensionSeparator("filename.txt.png"));

			Assert.AreEqual(".", Files.ExtensionSeparator("filename."));

			Assert.AreEqual(".", Files.ExtensionSeparator("filename..txt"));

			Assert.AreEqual(".", Files.ExtensionSeparator(".filename.txt"));

			Assert.AreEqual("", Files.ExtensionSeparator("."));

			Assert.AreEqual("", Files.ExtensionSeparator(".."));

			Assert.AreEqual("", Files.ExtensionSeparator(".txt"));
		}

		[Test]
		public void ReassembleFilenames () {
			// Assert:
			Assert.AreEqual("filename.txt", reassemble("filename.txt"));

			Assert.AreEqual("filename", reassemble("filename"));

			Assert.AreEqual("filename.txt.png", reassemble("filename.txt.png"));

			Assert.AreEqual("filename.", reassemble("filename."));

			Assert.AreEqual("filename..txt", reassemble("filename..txt"));

			Assert.AreEqual(".filename.txt", reassemble(".filename.txt"));

			Assert.AreEqual(".", reassemble("."));

			Assert.AreEqual("..", reassemble(".."));

			Assert.AreEqual(".txt", reassemble(".txt"));
		}

		private string reassemble (string filename) {
			return Files.StripExtension(filename) + Files.ExtensionSeparator(filename) + Files.Extension(filename);
		}

		#endregion

		#region Path

		[Test]
		public void CombinePath () {
			// Assert normal cases:
			Assert.AreEqual(
				"some/simple/path/to/a/file.txt", 
				Files.CombinePath("some", "simple", "path", "to", "a", "file.txt"));
			Assert.AreEqual(
				"some/simple/path/to/a/file.txt", 
				Files.CombinePath("some/", "simple/", "path/", "to/", "a/", "file.txt"));
			Assert.AreEqual(
				"some/simple/path/to/a/file.txt", 
				Files.CombinePath("some/", "/simple/", "/path/", "/to/", "/a/", "/file.txt"),
				"Additional path segments should be treated as relative paths.");
			Assert.AreEqual(
				"/absolute/some/simple/path/to/a/file.txt", 
				Files.CombinePath("/absolute", "some", "simple", "path", "to", "a", "file.txt"));
			Assert.AreEqual(
				"file.txt", 
				Files.CombinePath("", "", "", "", "", "file.txt"));

			// Pathologic case:
			Assert.AreEqual(
				"/absolute/some/simple/path/to/a/file.txt", 
				Files.CombinePath("/absolute/some/simple/path/to/a/file.txt"));
			Assert.AreEqual(
				"file.txt", 
				Files.CombinePath("file.txt"));
			Assert.AreEqual(
				"", 
				Files.CombinePath(""));
			Assert.AreEqual(
				"", 
				Files.CombinePath(null));
		}

		[Test]
		public void IsValidPath () {
			// Arrange:
			// TODO

			// Act:
			// TODO

			// Assert:
			// OK:
			Assert.That(Files.IsValidPath("."), "'.' is a valid path.");
			Assert.That(Files.IsValidPath(".."), "'..' is a valid path.");
			Assert.That(Files.IsValidPath("/"), "'/' is a valid path.");
			Assert.That(Files.IsValidPath("relative/path/to/a/file.txt"));
			Assert.That(Files.IsValidPath("/absolute/path/to/a/file.txt"));
			Assert.That(Files.IsValidPath("justTheFile.txt"));
			Assert.That(Files.IsValidPath(".justAHiddenFile"));
			Assert.That(Files.IsValidPath("/absolute/path/ending/with/a/slash/"));

			// Not OK:
			char[] invalidPathChars = Path.GetInvalidPathChars();
			foreach ( char c in invalidPathChars ) {
				string invPath = "somePath_With_Invalid_Char_" + c + "_in_it";
				Assert.That(!Files.IsValidPath(invPath), "Invalid path '" + invPath + "' should be rejected by Files.IsValidPath().");
			}
		}

		[Test]
		public void FileName () {
			// Assert:
			Assert.AreEqual("file.txt", Files.FileName("relative/path/to/a/file.txt"));
			Assert.AreEqual("file.txt", Files.FileName("/absolute/path/to/a/file.txt"));
			Assert.AreEqual(".file", Files.FileName("relative/path/to/a/hidden/.file"));
			Assert.AreEqual(".file", Files.FileName("/absolute/path/to/a/hidden/.file"));
			Assert.AreEqual("", Files.FileName("relative/path/ending/with/a/slash/"));
			Assert.AreEqual("", Files.FileName("/absolute/path/ending/with/a/slash/"));
			Assert.AreEqual("", Files.FileName("relative/path/ending/with/a/dot/."));
			Assert.AreEqual("", Files.FileName("/absolute/path/ending/with/a/dot/."));
			Assert.AreEqual("", Files.FileName("relative/path/ending/with/two/dots/.."));
			Assert.AreEqual("", Files.FileName("/absolute/path/ending/with/two/dots/.."));
			Assert.AreEqual("justTheFile.txt", Files.FileName("justTheFile.txt"));
			Assert.AreEqual("justTheFileWithoutExtension", Files.FileName("justTheFileWithoutExtension"));
			Assert.AreEqual(".justAHiddenFile", Files.FileName(".justAHiddenFile"));
			// URL:
			Assert.AreEqual (
				"1_k800_badkoesenpumphaus.jpg", 
				Files.FileName("http://qeevee.org:9091/uploadedassets/281/editor/10346/1_k800_badkoesenpumphaus.jpg")
			);
		}

		[Test]
		public void DirName () {
			// Assert:
			Assert.AreEqual("dir", Files.DirName("relative/path/to/a/dir"));
			Assert.AreEqual("dir", Files.DirName("/absolute/path/to/a/dir"));
			Assert.AreEqual(".dir", Files.DirName("relative/path/to/a/hidden/.dir"));
			Assert.AreEqual(".dir", Files.DirName("/absolute/path/to/a/hidden/.dir"));
			Assert.AreEqual("slash", Files.DirName("relative/path/ending/with/a/slash/"));
			Assert.AreEqual("slash", Files.DirName("/absolute/path/ending/with/a/slash/"));
			Assert.AreEqual("dot", Files.DirName("relative/path/ending/with/a/dot/."));
			Assert.AreEqual("dot", Files.DirName("/absolute/path/ending/with/a/dot/."));
			Assert.AreEqual("two", Files.DirName("relative/path/ending/with/two/dots/.."));
			Assert.AreEqual("two", Files.DirName("/absolute/path/ending/with/two/dots/.."));
			Assert.AreEqual("justTheDir", Files.DirName("justTheDir"));
			Assert.AreEqual("aDirNameWith.Extension", Files.DirName("aDirNameWith.Extension"));
			Assert.AreEqual(".justAHiddenDir", Files.DirName(".justAHiddenDir"));
		}

		[Test]
		public void ParentDir () {
			// Assert:
			Assert.AreEqual("relative/path/to/a/", Files.ParentDir("relative/path/to/a/dir"));
			Assert.AreEqual("/absolute/path/to/a/", Files.ParentDir("/absolute/path/to/a/dir"));
			Assert.AreEqual("short/", Files.ParentDir("short/path"));
			Assert.AreEqual("/short/", Files.ParentDir("/short/path"));
			Assert.AreEqual("/", Files.ParentDir("shortestpath"));
			Assert.AreEqual("/", Files.ParentDir("/shortestpath"));
			Assert.AreEqual("relative/path/ending/with/a/", Files.ParentDir("relative/path/ending/with/a/dot/."));
			Assert.AreEqual("/absolute/path/ending/with/a/", Files.ParentDir("/absolute/path/ending/with/a/dot/."));
			Assert.AreEqual("relative/path/ending/with/", Files.ParentDir("relative/path/ending/with/two/dots/.."));
			Assert.AreEqual("/absolute/path/ending/with/", Files.ParentDir("/absolute/path/ending/with/two/dots/.."));
			Assert.AreEqual("relative/path/two/", Files.ParentDir("relative/path/containing/../two/dots"));
			Assert.AreEqual("/absolute/path/and/ending/with/", Files.ParentDir("/absolute/path/containing/../and/ending/with/two/dots/.."));
			Assert.AreEqual("relative/path/to/a/hidden/", Files.ParentDir("relative/path/to/a/hidden/.dir"));
			Assert.AreEqual("/absolute/path/to/a/hidden/", Files.ParentDir("/absolute/path/to/a/hidden/.dir"));

			Assert.Throws<ArgumentException>(
				delegate {
					Files.ParentDir("");
				}, 
				"Calling ParentDir on empty path should throw ArgumentException.");
			Assert.Throws<ArgumentException>(
				delegate {
					Files.ParentDir(".");
				}, 
				"Calling ParentDir on dot should throw ArgumentException.");
			Assert.Throws<ArgumentException>(
				delegate {
					Files.ParentDir("/.");
				}, 
				"Calling ParentDir on dot should throw ArgumentException.");
			Assert.Throws<ArgumentException>(
				delegate {
					Files.ParentDir("..");
				}, 
				"Calling ParentDir on double dot should throw ArgumentException.");
			Assert.Throws<ArgumentException>(
				delegate {
					Files.ParentDir("/..");
				}, 
				"Calling ParentDir on double dot should throw ArgumentException.");
			Assert.Throws<ArgumentException>(
				delegate {
					Files.ParentDir("/");
				}, 
				"Calling ParentDir on root should throw ArgumentException.");
		}

		#endregion

		#region Directory Features

		[Test]
		public void IsDirEmpty_Asset () {
			// Arrange:
			ensureCleanTargets();

			string emptyDirPath = GQAssert.TEST_DATA_BASE_DIR + "FilesTest/Origins/EmptyDir";
			string targetDirPath = GQAssert.TEST_DATA_BASE_DIR + "FilesTest/Targets";
			string nonEmptyDirPath = GQAssert.TEST_DATA_BASE_DIR + "FilesTest/Origins/SomeFiles";
			string nonExistingDirPath = GQAssert.TEST_DATA_BASE_DIR + "FilesTest/Origins/NonExistingDirectory";
			Assert.That(!Directory.Exists(nonExistingDirPath), "ERROR in Test Data. This directory should not exist. " + nonExistingDirPath);

			// Assert:
			Assert.That(Files.IsEmptyDir(emptyDirPath), "Empty directory should have been detected correctly.");
			Assert.That(Files.IsEmptyDir(targetDirPath), "Targets directory should have been detected as empty correctly, it only contains hidden files.");
			Assert.That(!Files.IsEmptyDir(nonEmptyDirPath), "Non-empty directory should have been detected correctly.");
			Assert.That(!Files.IsEmptyDir(nonExistingDirPath), "Non-existing directory should have not been detected as empty dir.");
		}

		[Test]
		public void IsDirEmpty_NonAsset () {
			ensureCleanTargets();

			// Arrange:
			string nonExistingAssetDirPath = GQAssert.PROJECT_PATH + "Non/Existing/Directory";
			Assert.That(!Directory.Exists(nonExistingAssetDirPath), "ERROR in Test Data. This directory should not exist. " + nonExistingAssetDirPath);

			// Assert:
			Assert.That(Files.IsEmptyDir(originAssetDirEmptyPath), "Empty directory should have been detected correctly.");
			Assert.That(Files.IsEmptyDir(targetAssetDirPath), "Targets directory should have been detected as empty correctly, it only contains hidden files.");
			Assert.That(!Files.IsEmptyDir(originNonAssetDirWithFilesPath), "Non-empty directory should have been detected correctly.");
			Assert.That(!Files.IsEmptyDir(nonExistingAssetDirPath), "Non-existing directory should have not been detected as empty dir.");
		}

		[Test]
		public void IsParentDirPath () {
			// Assert:
			Assert.That(Files.IsParentDirPath("/", "grandfather/father/son"));
			Assert.That(Files.IsParentDirPath("/", "/grandfather/father/son"));
			Assert.That(Files.IsParentDirPath("/", "grandfather/father/son/"));
			Assert.That(Files.IsParentDirPath("/", "/grandfather/father/son/"));
			Assert.That(Files.IsParentDirPath("grandfather", "grandfather/father/son"));
			Assert.That(Files.IsParentDirPath("grandfather/father", "grandfather/father/son"));
			Assert.That(!Files.IsParentDirPath("grandfather/father/son", "grandfather/father/son"));
			Assert.That(!Files.IsParentDirPath("grandfather/father/son", "grandfather/father/son", false));
			Assert.That(Files.IsParentDirPath("grandfather/father/son", "grandfather/father/son", true));

			Assert.That(!Files.IsParentDirPath("/a", "/b"));
		}

		#endregion

		#region Asset Database Related

		[Test, Ignore("todo")]
		public void StripAssetMetadata () {
			// Arrange:
			// TODO test a method that strips all asset metadata from a directory's contained files and subdirs.

			// Act:
			// TODO

			// Assert:
			// TODO
			Assert.Fail("Test not yet implemented!");
		}

		#endregion
	}
}
