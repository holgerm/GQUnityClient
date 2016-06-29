using System.IO;
using NUnit.Framework;
using System;
using System.Security;
using GQ.Util;
using UnityEngine;

namespace GQTests {

	public class GQAssert {

		static private string _TEST_DATA_BASE_DIR = "Assets/Editor/GQTestsData/";

		static public string TEST_DATA_BASE_DIR {
			get {
				return _TEST_DATA_BASE_DIR;
			}
		}

		public static bool FileExistsAt (string path, string message = "") {
			bool result = true;
			if ( message.Length > 0 )
				message += " ";

			try {
				FileInfo file = new FileInfo(path);
				result = file.Exists;
			} catch ( ArgumentNullException e ) {
				Assert.Fail(message + "File path was null.");
				result = false;
			} catch ( SecurityException e ) {
				Assert.Fail(message + "The caller does not have the required permission to access " + path + ".");
				result = false;
			} catch ( ArgumentException e ) {
				Assert.Fail(message + "File path is invalid: " + path + ".");
				result = false;
			} catch ( UnauthorizedAccessException e ) {
				Assert.Fail(message + "Access to file " + path + " is denied.");
				result = false;
			} catch ( PathTooLongException e ) {
				Assert.Fail(message + "Directory path is too long: " + path + ".");
				result = false;
			} catch ( NotSupportedException e ) {
				Assert.Fail(message + "File name contains a colon " + path + ".");
				result = false;
			} 
			return result;
		}

		public static bool DirectoryExistsAt (string path, string message = "") {
			bool result = true;
			if ( message.Length > 0 )
				message += " ";

			try {
				DirectoryInfo dir = new DirectoryInfo(path);
				result = dir.Exists;
			} catch ( ArgumentNullException e ) {
				Assert.Fail(message + "Directory path was null.");
				result = false;
			} catch ( SecurityException e ) {
				Assert.Fail(message + "The caller does not have the required permission to access " + path + ".");
				result = false;
			} catch ( ArgumentException e ) {
				Assert.Fail(message + "Directory path is invalid: " + path + ".");
				result = false;
			} catch ( PathTooLongException e ) {
				Assert.Fail(message + "Directory path is too long: " + path + ".");
				result = false;
			} 
			return result;
		}

		public static string UniqueNameInDir (string nameGiven, string dirPath) {
			string curNameChecking = nameGiven;

			bool nameAlreadyTaken = checkIfNameExistsInDir(curNameChecking, dirPath);

			string strippedName = Files.StripExtension(nameGiven);
			string extension = Files.Extension(nameGiven);
			int counter = 1;

			while ( nameAlreadyTaken ) {
				curNameChecking = strippedName + counter;
				if ( extension.Length > 0 )
					curNameChecking += '.' + extension;
				
				nameAlreadyTaken = checkIfNameExistsInDir(curNameChecking, dirPath);
				counter++;
			}

			return curNameChecking;
		}

		private static bool checkIfNameExistsInDir (string nameToCheck, string dir) {
			FileInfo fileWithSameName = new FileInfo(dir + Files.PATH_ELEMENT_SEPARATOR + nameToCheck);
			if ( fileWithSameName.Exists )
				return true;

			DirectoryInfo dirWithSameName = new DirectoryInfo(dir + Files.PATH_ELEMENT_SEPARATOR + nameToCheck);
			return dirWithSameName.Exists;
		}

	}

}
