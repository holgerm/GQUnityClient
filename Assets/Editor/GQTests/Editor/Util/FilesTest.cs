using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Util;
using System.IO;
using GQ.Editor.Util;
using System;

namespace GQTests.Editor.Util {

	public class FilesTest {
		
		protected static string originAssetDirEmptyPath = 
			Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins", "EmptyDir");
		protected static string originAssetDirBasePath = 
			Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins");
		protected static string originAssetDirWithFilesPath = 
			Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins", "NonEmptyDir");
		protected static string newAssetDirPath = 
			Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Origins", "OtherDir");
		protected static string targetAssetDirPath =
			Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "FilesTest", "Targets");

		protected static string originNonAssetDirEmptyPath = 
			Files.CombinePath(GQAssert.PROJECT_PATH, "TestsData", "EmptyDir");
		protected static string originNonAssetDirBasePath = 
			Files.CombinePath(GQAssert.PROJECT_PATH, "TestsData");
		protected static string originNonAssetDirWithFilesPath = 
			Files.CombinePath(GQAssert.PROJECT_PATH, "TestsData", "NonEmptyDir");
		protected static string newNonAssetDirPath = 
			Files.CombinePath(GQAssert.PROJECT_PATH, "TestsData", "OtherDir");
		protected static string targetNonAssetDirPath =
			Files.CombinePath(GQAssert.PROJECT_PATH, "TestsData", "Targets");


		#region Assertion Helpers

		protected void ensureCleanTargets () {
			// Assets:
			if ( !Files.IsEmptyDir(targetAssetDirPath) )
				Files.ClearDir(targetAssetDirPath);

			Assert.That(Files.IsEmptyDir(targetAssetDirPath));
			expectOriginalDirStructureAtPath(originAssetDirBasePath);
			expectNewDirStructureAtPath(originAssetDirBasePath);

			// NON-Assets:
			if ( !Files.IsEmptyDir(targetNonAssetDirPath) )
				Files.ClearDir(targetNonAssetDirPath);

			Assert.That(Files.IsEmptyDir(targetNonAssetDirPath));
			expectOriginalDirStructureAtPath(originNonAssetDirBasePath);
			expectNewDirStructureAtPath(originNonAssetDirBasePath);
		}


		protected void expectOriginalDirStructureAtPath (string path) {
			// check that path is a directory containing NonEmptyDir with all its contents:
			Assert.That(Directory.Exists(path), "Dir should exist at " + path);
			string[] dirs = Directory.GetDirectories(path);
			string nonEmptyDir = Array.Find(dirs, x => Files.DirName(x).Equals("NonEmptyDir"));
			Assert.NotNull(nonEmptyDir, "NonEmptyDir should be found at " + path);

			// check that all files and subdir are present:
			expectOriginalDirContentInDir(nonEmptyDir);

			// check that no further files are present:
			int numberOfContainedFiles = Array.FindAll(Directory.GetFiles(nonEmptyDir), x => Files.IsNormalFile(x)).Length;
			Assert.AreEqual(
				5, 
				numberOfContainedFiles, 
				"There should be 5 files in 'NonEmptyDir' but we found " + numberOfContainedFiles + " files at path: " + path);

			// check that no further directories are present:
			int numberOfContainedDirs = Directory.GetDirectories(nonEmptyDir).Length;
			Assert.AreEqual(
				1, 
				numberOfContainedDirs, 
				"There should be exactly one subdir in 'NonEmptyDir' but we found " + numberOfContainedDirs + " dirs at path: " + path);
		}

		protected void expectOriginalDirContentInDir (string path) {
			// check files
			string[] files = Directory.GetFiles(path);
			string lookedUpFile = Array.Find(files, x => Files.FileName(x).Equals("Image.png"));
			Assert.NotNull(lookedUpFile, "There should be a file named Image.png in " + path);
			lookedUpFile = Array.Find(files, x => Files.FileName(x).Equals("JSONDocument.json"));
			Assert.NotNull(lookedUpFile, "There should be a file named JSONDocument.json in " + path);
			lookedUpFile = Array.Find(files, x => Files.FileName(x).Equals("OtherPlainTextDocument.txt"));
			Assert.NotNull(lookedUpFile, "There should be a file named OtherPlainTextDocument.txt in " + path);
			lookedUpFile = Array.Find(files, x => Files.FileName(x).Equals("PlainTextDocument.txt"));
			Assert.NotNull(lookedUpFile, "There should be a file named PlainTextDocument.txt in " + path);
			lookedUpFile = Array.Find(files, x => Files.FileName(x).Equals("RichTextDocument.rtf"));
			Assert.NotNull(lookedUpFile, "There should be a file named RichTextDocument.rtf in " + path);

			// check Subdir (which should be empty)
			string[] dirs = Directory.GetDirectories(path);
			string subDir = Array.Find(dirs, x => Files.DirName(x).Equals("SubDir"));
			Assert.NotNull(subDir, "There should be a dir called SubDir in " + path);

			String[] subFiles = Array.FindAll(Directory.GetFiles(subDir), x => Files.IsNormalFile(x));
			Assert.NotNull(subFiles, "SubDir should contain files.");
			Assert.AreEqual(1, subFiles.Length, "SubDir should contain 1 files, but we found " + subFiles.Length + ".");
			Assert.That(
				subFiles[0].EndsWith("PlainTextDocument.txt"), 
				"The first and only file contained in SubDir should be named 'PlainTextDocument.txt' but we found: " + subFiles[0]);
		}

		protected void expectEmptyDirSturctureAtPath (string path) {
			Assert.That(Directory.Exists(path));
			string[] dirs = Directory.GetDirectories(path);
			string emptyDir = Array.Find(dirs, x => Files.DirName(x).Equals("EmptyDir"));
			Assert.NotNull(emptyDir, "Directory named Empty should be found at " + path);
			Assert.AreEqual(0, Directory.GetDirectories(emptyDir).Length, "Dir EmptyDir should not contain subdirs but we found " + Directory.GetDirectories(emptyDir).Length);
			Assert.AreEqual(0, Directory.GetFiles(emptyDir).Length, "Empty dir should not contain files but we found " + Directory.GetFiles(emptyDir).Length);
		}


		protected void expectNewDirStructureAtPath (string basePath) {
			// check OtherDir is contained in basePath:
			string[] dirs = Directory.GetDirectories(basePath);
			string otherDir = Array.Find(dirs, x => x.EndsWith("OtherDir"));
			Assert.NotNull(otherDir, "There should exist dir 'OtherDir' at path " + basePath);

			// check OtherDir contains only one file named Other.txt:
			dirs = Directory.GetDirectories(otherDir);
			Assert.NotNull(dirs, "There should be an empty list of subdirs in 'OtherDir' but we got null.");
			Assert.AreEqual(
				0, dirs.Length, 
				"There should be no subdirs in 'OtherDir' but we found " + dirs.Length + " dirs.");

			// check OtherDir contains 1 file, namely Other.txt:
			string[] files = Array.FindAll(Directory.GetFiles(otherDir), x => Files.IsNormalFile(x));
			Assert.NotNull(files, "OtherDir should contain files.");
			Assert.AreEqual(
				1, 
				files.Length, 
				"OtherDir should contain 1 file, but we found " + files.Length + " files in " + otherDir);
			Assert.That(
				files[0].EndsWith("Other.txt"), 
				"The first file contained in OtherDir should be named 'Other.txt' but we found: " + files[0]);
		}

		protected void expectContentAtPath (string expectedContent, string targetPath, string message) {
			Assert.That(Files.ExistsFile(targetPath));
			Assert.AreEqual(expectedContent, File.ReadAllText(targetPath), message);
		}

		#endregion
	}
}
