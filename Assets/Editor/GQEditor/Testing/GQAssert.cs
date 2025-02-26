﻿using System.IO;
using UnityEngine;
using GQ.Editor.Util;

namespace GQTests
{

	public class GQAssert
	{

		// TODO move these paths to Files class:

		public static string TEST_DATA_BASE_DIR {
			get {
				return _TEST_DATA_BASE_DIR;
			}
		}
		private static string _TEST_DATA_BASE_DIR = "Assets/Editor/GQTestsData/";

		public static string TEST_DATA_SERVER_DIR = Files.CombinePath (TEST_DATA_BASE_DIR, "Server");

		public static string TEST_DATA_TEMP_DIR = "Temp";


		private static string _PROJECT_PATH = Application.dataPath.Substring (0, Application.dataPath.Length - "/Assets".Length);

		public static string PROJECT_PATH {
			get {
				return _PROJECT_PATH;
			}
		}

		public static string UniqueNameInDir (string nameGiven, string dirPath)
		{
			string curNameChecking = nameGiven;

			bool nameAlreadyTaken = checkIfNameExistsInDir (curNameChecking, dirPath);

			string strippedName = Files.StripExtension (nameGiven);
			string extension = Files.Extension (nameGiven);
			int counter = 1;

			while (nameAlreadyTaken) {
				curNameChecking = strippedName + counter;
				if (extension.Length > 0)
					curNameChecking += '.' + extension;
				
				nameAlreadyTaken = checkIfNameExistsInDir (curNameChecking, dirPath);
				counter++;
			}

			return curNameChecking;
		}

		private static bool checkIfNameExistsInDir (string nameToCheck, string dir)
		{
			FileInfo fileWithSameName = new FileInfo (Files.CombinePath (dir, nameToCheck));
			if (fileWithSameName.Exists)
				return true;

			DirectoryInfo dirWithSameName = new DirectoryInfo (Files.CombinePath (dir, nameToCheck));
			return dirWithSameName.Exists;
		}

	}

}
