using System.IO;
using UnityEngine;
using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

namespace GQ.Editor.Util
{
	
	public class Files : GQ.Client.FileIO.Files
	{

		#region Asset Agnostic API

		public static bool ExistsFile (string filePath)
		{
			if (Assets.IsAssetPath (filePath)) {
				return Assets.ExistsAssetAtPath (filePath);
			} else {
				return File.Exists (filePath);
			}
		}

		public static bool ExistsDir (string dirPath)
		{
			if (Assets.IsAssetPath (dirPath)) {
				return Assets.ExistsAssetAtPath (dirPath);
			} else {
				return Directory.Exists (dirPath);
			}
		}

		/// <summary>
		/// Creates a new dir at the given path. Returns true iff successful.
		/// 
		/// This method works on assets as well as on "normal" files and directories.
		/// </summary>
		/// <param name="dirPath">Dir path.</param>
		public static bool CreateDir (string dirPath)
		{
			// Normalize given path:
			while (dirPath.EndsWith ("/"))
				dirPath = dirPath.Substring (0, dirPath.Length - 2);

			if (Assets.IsAssetPath (dirPath)) {
				dirPath = Assets.RelativeAssetPath (dirPath);
				if (Assets.ExistsAssetAtPath (dirPath))
					// asset dir already exists:
					return false;
				else {
					string newFolderName = Path.GetFileName (dirPath);
					string parentFolder = Path.GetDirectoryName (dirPath);
					string guid = AssetDatabase.CreateFolder (parentFolder, newFolderName);
					return !String.IsNullOrEmpty (AssetDatabase.GUIDToAssetPath (guid));
				}
			} else {
				if (Directory.Exists (dirPath))
					return false;
				else {
					try {
						Directory.CreateDirectory (dirPath);
						return true;
					} catch (Exception e) {
						Debug.LogWarning ("CreateDir(" + dirPath + ") threw exception: " + e.Message);
						return false;
					}
				}
			}
		}

		/// <summary>
		/// Clears the given dir. Returns true iff successful.
		/// 
		/// This method works on assets as well as on "normal" files and directories.
		/// </summary>
		/// <param name="dirPath">Dir path.</param>
		public static bool ClearDir (string dirPath)
		{
			if (!Files.ExistsDir (dirPath))
				return false;

			if (Assets.IsAssetPath (dirPath))
				return Assets.ClearAssetFolder (dirPath);
			else {
				bool cleaned = true;

				// delete all files:
				string[] files = Directory.GetFiles (dirPath);
				foreach (string file in files) {
					File.Delete (file);
					cleaned &= !File.Exists (file);
				}

				// delete all directories:
				string[] dirs = Directory.GetDirectories (dirPath);
				foreach (string dir in dirs) {
					Directory.Delete (dir, true);
					cleaned &= !Directory.Exists (dir);
				}

				return cleaned;
			}
		}

		/// <summary>
		/// Deletes the given dir and all contained files or subdirectories.
		/// 
		/// This method works on assets as well as on "normal" files and directories.
		/// </summary>
		/// <returns><c>true</c>, if dir was deleted, <c>false</c> otherwise.</returns>
		/// <param name="dirPath">Dir path.</param>
		public static bool DeleteDir (string dirPath)
		{
			// Normalize given path:
			while (dirPath.EndsWith ("/"))
				dirPath = dirPath.Substring (0, dirPath.Length - 2);

			Debug.Log ("DeleteDir: " + dirPath);

			if (Assets.IsAssetPath (dirPath)) {
				if (!Assets.ExistsAssetAtPath (dirPath))
					// asset dir does not exist:
					return false;
				else {
					return AssetDatabase.DeleteAsset (dirPath);
				}
			} else {
				if (!Directory.Exists (dirPath))
					return false;
				else {
					try {
						Directory.Delete (dirPath, true);
						return !Directory.Exists (dirPath);
					} catch (Exception e) {
						Debug.LogWarning ("DeleteDir(" + dirPath + ") threw exception: " + e.Message);
						return false;
					}
				}
			}
		}

		/// <summary>
		/// Copies the file.
		/// 
		/// This method works on assets as well as on "normal" files and directories.
		/// </summary>
		/// <returns><c>true</c>, if file was copyed, <c>false</c> otherwise.</returns>
		/// <param name="fromFilePath">Path of the original file.</param>
		/// <param name="toDirPath">Path of the target file that will be generated. 
		/// 	If the target file previously exists, copying either fails or overwrites the old file, regarding the overwrite flag given.
		/// </param>
		public static bool CopyFile (string fromFilePath, string toDirPath, bool overwrite = true)
		{
			if (!Files.ExistsFile (fromFilePath))
				// we can not copy a non existing file:
				return false;

			if (!Files.ExistsDir (toDirPath) && (!overwrite || !Files.CreateDir (toDirPath)))
				// target dir does not exist and we shall or can not create it => we give up:
				return false;

			string toFilePath = Files.CombinePath (toDirPath, Files.FileName (fromFilePath));

			if (!overwrite && Files.ExistsFile (toFilePath))
				// we shall not overwrite already existing target file:
				return false;

			if (Assets.IsAssetPath (fromFilePath) && Assets.IsAssetPath (toFilePath)) {
				// we copy an asset file to an asset file:
				if (Assets.ExistsAssetAtPath (toFilePath))
					AssetDatabase.DeleteAsset (toFilePath);
				bool isCopied = AssetDatabase.CopyAsset (fromFilePath, toFilePath);
				AssetDatabase.Refresh ();
				return isCopied;
			} else {
				// we copy a file / asset to a non-asset target file:
				if (Assets.IsAssetPath (fromFilePath)) {
					// if we copy an asset we need to use its absolute file path:
					fromFilePath = Assets.AbsolutePath4Asset (fromFilePath);
				}
				try {
					File.Copy (fromFilePath, toFilePath, true);
					if (!File.Exists (toFilePath))
						return false;

					if (Assets.IsAssetPath (toFilePath)) {
						AssetDatabase.Refresh ();
					}
					return true;

				} catch (Exception e) {
					Debug.LogWarning ("CopyFile(" + fromFilePath + ", " + toDirPath + ") threw exception: " + e.Message);
					return false;
				}
			}
		}

		/// <summary>
		/// Deletes the file at the given path if it exists.
		/// </summary>
		/// <returns><c>true</c>, if file existed and was deleted, <c>false</c> otherwise.</returns>
		/// <param name="filePath">File path.</param>
		public static bool DeleteFile (string filePath)
		{
			if (Assets.IsAssetPath (filePath)) {
				if (!Assets.ExistsAssetAtPath (filePath))
					// asset dir does not exist:
					return false;
				else {
					File.Delete (filePath);
					AssetDatabase.Refresh ();
					return !File.Exists (filePath);
				}
			} else {
				if (!File.Exists (filePath))
					return false;
				else {
					try {
						File.Delete (filePath);
						return !File.Exists (filePath);
					} catch (Exception e) {
						Debug.LogWarning ("DeleteFile(" + filePath + ") threw exception: " + e.Message);
						return false;
					}
				}
			}
		}

		/// <summary>
		/// Moves the file.
		/// </summary>
		/// <returns><c>true</c>, if file was moved, <c>false</c> otherwise.</returns>
		/// <param name="fromFilePath">Path of the original file.</param>
		/// <param name="toDirPath">Path fof the target directory.</param>
		public static bool MoveFile (string fromFilePath, string toDirPath, bool replace = true)
		{
			if (!CopyFile (fromFilePath, toDirPath, replace))
				return false;

			return DeleteFile (fromFilePath);
		}

		public static bool MoveDir (string fromDirPath, string toDirPath, bool replace = true)
		{
			if (CopyDir (fromDirPath, toDirPath, replace) == false)
				return false;

			return DeleteDir (fromDirPath);
		}

		/// <summary>
		/// Copies the given dir fromDir into the given toDir. 
		/// The third parameter specifies if also recursively all contained folders are copied - default is true.
		/// 
		/// This method works on assets as well as on "normal" files and directories.
		/// </summary>
		/// <returns><c>true</c>, if dir was copyed, <c>false</c> otherwise.</returns>
		/// <param name="fromDirPath">From dir path.</param>
		/// <param name="toDirPath">To dir path.</param>
		/// <param name="replace">If set to <c>true</c> a already existing directory and all its contents is replaced.</param>
		public static bool CopyDir (string fromDirPath, string toDirPath, bool replace = true)
		{
			if (!ExistsDir (fromDirPath))
				// nothing to copy:
				return false;

			if (!ExistsDir (toDirPath) && !CreateDir (toDirPath))
				//	no target dir given nor can we create it:
				return false;

			string targetDirPath = 
				Files.CombinePath (
					toDirPath,
					Files.DirName (fromDirPath)
				);

			if (ExistsDir (targetDirPath)) {
				if (replace) {
					// replace the existing target dir:
					ClearDir (targetDirPath);
				} else {
					// cancel copying since we shall not replace the existing target dir:
					return false;
				}
			}
			
			if (Assets.IsAssetPath (fromDirPath) && Assets.IsAssetPath (toDirPath)) {
				// case Asset -> Asset
				return Assets.copyAssetsDir (fromDirPath, toDirPath);
			} else {
				return copyDirToNonAssetsDir (fromDirPath, toDirPath);  
			}
			return false;
		}

		private static bool copyDirToNonAssetsDir (string fromDirPath, string toDirPath, bool recursive = true, bool copyContentsOnly = false)
		{
			bool copied = true;

			// create this folder within the target path:
			string targetFolder;

			if (copyContentsOnly) {
				targetFolder = toDirPath;
			} else {
				targetFolder = Files.CombinePath (toDirPath, Files.DirName (fromDirPath));
				Directory.CreateDirectory (targetFolder);
				copied &= Directory.Exists (targetFolder); // copied dir should now exist in fromDir. 
			}

			foreach (string file in Directory.GetFiles(fromDirPath)) {

				string fileName = Files.FileName (file);

				if (!fileName.StartsWith (".") && !fileName.EndsWith (".meta")) {

					File.Copy (file, Files.CombinePath (targetFolder, fileName));
				}
			}

			if (recursive)
				foreach (string subDir in Directory.GetDirectories(fromDirPath)) {

					copied &= copyDirToNonAssetsDir (subDir, targetFolder, copyContentsOnly: false);
				}

			AssetDatabase.Refresh ();

			return copied;
		}

		// TODO additional parameter used as filter for excluded content (files and dirs)
		public static bool CopyDirContents (string fromDirPath, string toDirPath, bool replace = true, bool copyContentsOnly = false)
		{
			if (!ExistsDir (fromDirPath))
				// nothing to copy:
				return false;

			if (!ExistsDir (toDirPath) && !CreateDir (toDirPath))
				//	no target dir given nor can we create it:
				return false;

			if (ExistsDir (toDirPath) && replace)
				// replace the existing target dir:
				ClearDir (toDirPath);
			else
				// cancel copying since we shall not replace the existing target dir:
				return false;

			if (Assets.IsAssetPath (fromDirPath) && Assets.IsAssetPath (toDirPath)) {
				// case Asset -> Asset
				return Assets.CopyAssetsDirContents (fromDirPath, toDirPath, true);
			} else {
				// case Asset or Non-Asset -> Non-Asset
				return copyDirToNonAssetsDir (fromDirPath, toDirPath, replace, copyContentsOnly : true);  
			}
		}

		#endregion


		#region Asset Database Related

		public static void StripAssetMetadata (string dir)
		{
			
			if (Assets.IsAssetPath (dir))
				throw new ArgumentException ("DO NOT strip metadata from asset files and directories as " + dir);
			
			string[] files = Directory.GetFiles (dir);
			foreach (var file in files) {
				if (file.EndsWith (".meta"))
					File.Delete (file);
			}

			string[] subdirs = Directory.GetDirectories (dir);

			foreach (var subdir in subdirs) {
				StripAssetMetadata (subdir);
			}
		}

		#endregion

	}

}
