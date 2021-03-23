using UnityEngine;
using System.IO;
using System;
using UnityEditor;

namespace GQ.Editor.Util
{

	public static class Assets
	{

		/// <summary>
		/// Determines if the specified path points to an existing asset.
		/// </summary>
		/// <returns><c>true</c> if is asset path the specified path; otherwise, <c>false</c>.</returns>
		/// <param name="path">Path.</param>
		public static bool ExistsAssetAtPath (string path)
		{
			if (!IsAssetPath (path))
				return false;

			if (path.StartsWith ("/")) {
				// we have an absolute path within the assets dir:
				bool fileExists = File.Exists (path) || Directory.Exists (path);
				bool guidExists = !String.IsNullOrEmpty (AssetDatabase.AssetPathToGUID (RelativeAssetPath (path)));
				return fileExists && guidExists;
			} else {
				// we have a relative path:
				bool fileExists = File.Exists (AbsolutePath4Asset (path)) || Directory.Exists (AbsolutePath4Asset (path));
				bool guidExists = !String.IsNullOrEmpty (AssetDatabase.AssetPathToGUID (path));
				return fileExists && guidExists;
			}
		}

		public static bool IsAssetPath (string path)
		{
			if (!Files.IsValidPath (path))
				return false;
			
            if (path.StartsWith ("/", StringComparison.CurrentCulture)) {
				// we have an absolute path:
				return path.StartsWith (Application.dataPath, StringComparison.CurrentCulture);
			} else {
				// we have a relative path:
				return path.StartsWith ("Assets/", StringComparison.CurrentCulture);
			}

		}

		public static string AbsolutePath4Asset (string relativeAssetPath)
		{
			if (!IsAssetPath (relativeAssetPath))
				throw new ArgumentException ("Given path is not a valid asset path.");
			
			string projectParentPath = Application.dataPath.Substring (0, Application.dataPath.Length - "/Assets".Length);
			if (!relativeAssetPath.StartsWith (projectParentPath, StringComparison.CurrentCulture))
				return Files.CombinePath (projectParentPath, relativeAssetPath);
			else
				return relativeAssetPath;
		}

        /// <summary>
        /// Normalizes the given path relative to the Assets main directory (including Asstes itself) 
        /// when the given path points into Assets subtree.
        /// </summary>
        /// <returns>The asset path.</returns>
        /// <param name="path">Path.</param>
		public static string RelativeAssetPath (string path)
		{
            if (path.StartsWith (Application.dataPath, StringComparison.CurrentCulture)) {
				string projectParentPath = Application.dataPath.Substring (0, Application.dataPath.Length - "/Assets".Length);
				return path.Substring (projectParentPath.Length + 1);
			}
			if (path.StartsWith ("Assets", StringComparison.CurrentCulture)) {
				return path;
			}
				
			throw new ArgumentException ("Non asset path can not be normalized (" + path + ")");
		}

		/// <summary>
		/// Copies the assets dir fromDir into the  existing dir toDir. 
		/// Afterwards a similarly named dir as fromDir exists within toDir and contains all files and subdirs as fromDir does.
		/// </summary>
		/// <returns><c>true</c>, if assets dir was copyed, <c>false</c> otherwise.</returns>
		/// <param name="fromDir">From dir.</param>
		/// <param name="toDir">To dir.</param>
		/// <param name="recursive">If set to <c>true</c> recursive.</param>
		[Obsolete ("Use Files.CopyDir() instead. This method will only be kept as private implmentation.")]
		public static bool copyAssetsDir (string fromDir, string toDir, bool recursive = true)
		{
			if (!Assets.IsAssetPath (fromDir) || !Assets.IsAssetPath (toDir))
				return false;

			fromDir = Assets.RelativeAssetPath (fromDir);
			toDir = Assets.RelativeAssetPath (toDir);

			bool copied = true;

			DirectoryInfo fromDirInfo = new DirectoryInfo (fromDir);
			DirectoryInfo toDirInfo = new DirectoryInfo (toDir);
			string targetDir = Files.CombinePath (toDirInfo.FullName, fromDirInfo.Name);
			copied &= Assets.ExistsAssetAtPath (targetDir); // copied dir should now exist in fromDir. 

			foreach (FileInfo file in fromDirInfo.GetFiles()) {
			
				if (!file.Name.StartsWith (".") && !file.Name.EndsWith (".meta")) {

					string fileRelPath = RelativeAssetPath (file.FullName);
					string targetFilePath = Files.CombinePath (targetDir, file.Name);
					string targetRelPath = RelativeAssetPath (targetFilePath);
					copied &= AssetDatabase.CopyAsset (fileRelPath, targetRelPath);
				}
			}

			if (recursive)
				foreach (DirectoryInfo subDir in fromDirInfo.GetDirectories()) {

					copied &= copyAssetsDir (subDir.FullName, targetDir);
				}

			AssetDatabase.Refresh ();

			return copied;
		}

		public static bool CopyAssetsDirContents (string fromDir, string toDir, bool recursive = true)
		{
			if (!Assets.IsAssetPath (fromDir) || !Assets.IsAssetPath (toDir))
				return false;

			fromDir = Assets.RelativeAssetPath (fromDir);
			toDir = Assets.RelativeAssetPath (toDir);

			bool copied = true;

			DirectoryInfo fromDirInfo = new DirectoryInfo (fromDir);

			foreach (FileInfo file in fromDirInfo.GetFiles()) {

				if (!file.Name.StartsWith (".") && !file.Name.EndsWith (".meta")) {

					string fileRelPath = RelativeAssetPath (file.FullName);
					string targetFilePath = Files.CombinePath (toDir, file.Name);
					copied &= AssetDatabase.CopyAsset (fileRelPath, targetFilePath);
				}
			}

			if (recursive)
				foreach (DirectoryInfo subDir in fromDirInfo.GetDirectories()) {

					copied &= copyAssetsDir (subDir.FullName, toDir);
				}

			AssetDatabase.Refresh ();

			return copied;
		}


		/// <summary>
		/// Clears the given folder from any subfolders and assets and other files within.
		/// </summary>
		/// <returns><c>true</c>, if asset folder was cleared, i.e. if no file or directory remains, <c>false</c> otherwise.</returns>
		/// <param name="pathToFolder">Path to folder.</param>
		/// <param name="createIfNeeded">If true, an empty folder will be created if it not already exists.</param>
		[Obsolete ("Use Files.ClearDir() instead. This method will be internal soon.")]
		public static bool ClearAssetFolder (string pathToFolder, bool createIfNeeded = false)
		{
			bool cleared = true;

			if (!Directory.Exists (pathToFolder)) {
				if (createIfNeeded) {
					DirectoryInfo parentInfo = Directory.GetParent (pathToFolder);
					string dirName = Path.GetDirectoryName (pathToFolder);
					Assets.CreateSubfolder (parentInfo.FullName, dirName);
				} else
					return cleared;
			}


			// first delete all files / file assets:
			foreach (string file in Directory.GetFiles(pathToFolder)) {
				FileInfo fileInfo = new FileInfo (file);

				if (fileInfo.Name.EndsWith (".meta"))
					// ignore meta files, they will be deleted implicitly via AssetDatabase.DeleteAsset() later on ...
					continue;

				if (fileInfo.Name.EndsWith (".gitignore"))
					// keep optional .gitignore file, it is used independently of the currently prepared product.
					continue;

				if (fileInfo.Name.StartsWith (".")) {
					// delete hidden files - they are no assets and must be dealt as normal files
					File.Delete (fileInfo.FullName);
					continue;
				}
				
				string filePathRel = Files.CombinePath (pathToFolder, fileInfo.Name);
				cleared &= AssetDatabase.DeleteAsset (filePathRel);
			}

			// recursively clear all subdirectories:
			foreach (string dir in Directory.GetDirectories(pathToFolder)) {
				DirectoryInfo dirInfo = new DirectoryInfo (dir);
				string dirPath = Files.CombinePath (pathToFolder, dirInfo.Name);

				// empty the subdir:
				cleared &= ClearAssetFolder (dirPath);

				// remove the subdir:
				cleared &= AssetDatabase.DeleteAsset (dirPath);
			}

			AssetDatabase.Refresh ();

			return cleared;
		}

		/// <summary>
		/// Creates an assets subfolder within the given directory at basePath. 
		/// </summary>
		/// <returns>The guid of the new subfolder asset.</returns>
		/// <param name="basePath">Base path.</param>
		/// <param name="subfolderName">Subfolder name.</param>
		[Obsolete ("Use Files.CreateDir() instead.")]
		public static string CreateSubfolder (string basePath, string subfolderName)
		{
			while (basePath.EndsWith ("/")) {
				basePath = basePath.Substring (0, basePath.Length - 2);
			}
			return AssetDatabase.CreateFolder (basePath, subfolderName);
		}

	}
}
