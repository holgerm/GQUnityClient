using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEditor;

namespace GQ.Util {

	public static class Assets {

		public static bool Exists (string pathToAsset) {
//			Debug.Log("ASSET EXISTS? " + pathToAsset + " : " + AssetDatabase.AssetPathToGUID(pathToAsset));
			bool fileExists = File.Exists(pathToAsset) || Directory.Exists(pathToAsset);
			bool guidExists = !String.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(pathToAsset));
			return fileExists && guidExists;
		}

		public static string AbsolutePath (string relativeAssetPath) {
			string projectParentPath = Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length);
			return Files.CombinePath(projectParentPath, relativeAssetPath);
		}

		public static bool CopyAssetsDir (string fromDir, string toDir) {
			bool copied = true;

			DirectoryInfo fromDirInfo = new DirectoryInfo(fromDir);
			foreach ( FileInfo file in fromDirInfo.GetFiles() ) {
				if ( !file.Name.StartsWith(".") && !file.Name.EndsWith(".meta") ) {
					string fromFilePath = Files.CombinePath(fromDir, file.Name);
					string toFilePath = Files.CombinePath(toDir, file.Name);
					copied &= AssetDatabase.CopyAsset(fromFilePath, toFilePath);
				}
			}

			AssetDatabase.Refresh();
				
			return copied;
		}


		/// <summary>
		/// Clears the given folder from any subfolders and assets and other files within.
		/// </summary>
		/// <returns><c>true</c>, if asset folder was cleared, i.e. if no file or directory remains, <c>false</c> otherwise.</returns>
		/// <param name="pathToFolder">Path to folder.</param>
		public static bool ClearAssetFolder (string pathToFolder) {
			bool cleared = true;

			if ( !Directory.Exists(pathToFolder) )
				return cleared;

			// first delete all files / file assets:
			foreach ( string file in Directory.GetFiles(pathToFolder) ) {
				
				if ( file.EndsWith(".meta") )
					continue;
				
				FileInfo fileInfo = new FileInfo(file);
				string filePathRel = Files.CombinePath(pathToFolder, fileInfo.Name);
				cleared &= AssetDatabase.DeleteAsset(filePathRel);
			}

			// recursively clear all subdirectories:
			foreach ( string dir in Directory.GetDirectories(pathToFolder) ) {
				DirectoryInfo dirInfo = new DirectoryInfo(dir);
				string dirPath = Files.CombinePath(pathToFolder, dirInfo.Name);

				// empty the subdir:
				cleared &= ClearAssetFolder(dirPath);

				// remove the subdir:
				cleared &= AssetDatabase.DeleteAsset(dirPath);
			}

			AssetDatabase.Refresh();

			return cleared;
		}

		/// <summary>
		/// Creates an assets subfolder within the given directory at basePath. 
		/// </summary>
		/// <returns>The guid of the new subfolder asset.</returns>
		/// <param name="basePath">Base path.</param>
		/// <param name="subfolderName">Subfolder name.</param>
		public static string CreateSubfolder (string basePath, string subfolderName) {
			while ( basePath.EndsWith("/") ) {
				basePath = basePath.Substring(0, basePath.Length - 1);
			}
			string guid = AssetDatabase.CreateFolder(basePath, subfolderName);
//			AssetDatabase.Refresh();

			return guid;
		}

	}
}
