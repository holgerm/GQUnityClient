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
