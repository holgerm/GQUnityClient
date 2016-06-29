using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.IO;
using GQTests;
using GQ.Util;

namespace GQTests.Testing {

	public class GQAssertTest {

		[Test]
		public void TestDataFolderExistsAndCanAccess () {
			//Arrange
			string BASE_DIR = GQAssert.TEST_DATA_BASE_DIR + "GQAssertTest/DataFolderExistsAndCanAccess/";
			DirectoryInfo testDataDir = new DirectoryInfo(BASE_DIR);

			//Assert
			//The object has a new name
			Assert.That(testDataDir.Exists, "Test data folder should exist at " + GQAssert.TEST_DATA_BASE_DIR);


			DirectoryInfo subDir = testDataDir.CreateSubdirectory("TestDir"); 
			Assert.That(subDir.Exists, "Should be able to create a directory inside test data folder.");
			Assert.AreEqual(1, testDataDir.GetDirectories().Length, "Should be only one subdirectory now inside test data folder.");

			subDir.Delete();
			Assert.AreEqual(0, testDataDir.GetDirectories().Length, "Should be NO subdirectories now inside test data folder.");
		}

		[Test]
		public void TestUniqueNameInDir () {
			// Arrange:
			string BASE_DIR = GQAssert.TEST_DATA_BASE_DIR + "GQAssertTest/UniqueNameInDir/";
			// The rest is arranged in form of dirs and files already

			// Asserts for 'file.rtf':
			Assert.AreEqual(
				"file.rtf", 
				GQAssert.UniqueNameInDir("file.rtf", BASE_DIR + "EmptyFolder"), 
				"Original filename should be kept since it is already unique in EmptyFolder.");
			Assert.AreEqual(
				"file1.rtf", 
				GQAssert.UniqueNameInDir("file.rtf", BASE_DIR + "FolderFile0"), 
				"Unique filename should be 'file1.rtf' since 'file.rtf' has already existed before in folder.");
			Assert.AreEqual(
				"file2.rtf", 
				GQAssert.UniqueNameInDir("file.rtf", BASE_DIR + "FolderFile01"));
			Assert.AreEqual(
				"file.rtf", 
				GQAssert.UniqueNameInDir("file.rtf", BASE_DIR + "FolderFile123"), 
				"Unique filename should be kept 'file.rtf' since it is not used yet.");
			Assert.AreEqual(
				"file4.rtf", 
				GQAssert.UniqueNameInDir("file.rtf", BASE_DIR + "FolderFile0123"));
			Assert.AreEqual(
				"file5.rtf", 
				GQAssert.UniqueNameInDir("file.rtf", BASE_DIR + "FolderFile012346"), 
				"Unique filename should be 'file5.rtf' since that name does not exist yet.");

			// Asserts for 'file':
			Assert.AreEqual(
				"file", 
				GQAssert.UniqueNameInDir("file", BASE_DIR + "EmptyFolder"), 
				"Original filename should be kept since it is already unique in EmptyFolder.");
			Assert.AreEqual(
				"file1", 
				GQAssert.UniqueNameInDir("file", BASE_DIR + "FolderFile0"), 
				"Unique filename should be 'file1' since 'file' has already existed before in folder.");
			Assert.AreEqual(
				"file2", 
				GQAssert.UniqueNameInDir("file", BASE_DIR + "FolderFile01"));
			Assert.AreEqual(
				"file", 
				GQAssert.UniqueNameInDir("file", BASE_DIR + "FolderFile123"), 
				"Unique filename should be kept 'file' since it is not used yet.");
			Assert.AreEqual(
				"file4", 
				GQAssert.UniqueNameInDir("file", BASE_DIR + "FolderFile0123"));
			Assert.AreEqual(
				"file5", 
				GQAssert.UniqueNameInDir("file", BASE_DIR + "FolderFile012346"), 
				"Unique filename should be 'file5' since that name does not exist yet.");
		}

		[Test]
		public void TestFileExistsAt () {
			// Arrange:
			string BASE_DIR = GQAssert.TEST_DATA_BASE_DIR + "GQAssertTest/FileOrDirExistsAt/";

			// Assert:
			Assert.That(GQAssert.FileExistsAt(BASE_DIR + "ExistingFile"));
			Assert.That(!GQAssert.FileExistsAt(BASE_DIR + "NonExistingFile"));

		}

		[Test]
		public void TestDirectoryExistsAt () {
			// Arrange:
			string BASE_DIR = GQAssert.TEST_DATA_BASE_DIR + "GQAssertTest/FileOrDirExistsAt/";

			// Assert:
			Assert.That(GQAssert.DirectoryExistsAt(BASE_DIR + "ExistingDir"));
			Assert.That(!GQAssert.DirectoryExistsAt(BASE_DIR + "NonExistingDir"));
		}
	}
}
