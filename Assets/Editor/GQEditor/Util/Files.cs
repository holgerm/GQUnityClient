using System.IO;
using UnityEngine;
using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

namespace GQ.Editor.Util {
	public static class Files {

		#region Asset Agnostic API

		public static bool ExistsFile (string filePath) {
			if ( Assets.IsAssetPath(filePath) ) {
				return Assets.ExistsAssetAtPath(filePath);
			}
			else {
				return File.Exists(filePath);
			}
		}

		public static bool ExistsDir (string dirPath) {
			if ( Assets.IsAssetPath(dirPath) ) {
				return Assets.ExistsAssetAtPath(dirPath);
			}
			else {
				return Directory.Exists(dirPath);
			}
		}

		/// <summary>
		/// Creates a new dir at the given path. Returns true iff successful.
		/// 
		/// This method works on assets as well as on "normal" files and directories.
		/// </summary>
		/// <param name="dirPath">Dir path.</param>
		public static bool CreateDir (string dirPath) {
			// Normalize given path:
			while ( dirPath.EndsWith("/") )
				dirPath = dirPath.Substring(0, dirPath.Length - 2);

			if ( Assets.IsAssetPath(dirPath) ) {
				dirPath = Assets.RelativeAssetPath(dirPath);
				if ( Assets.ExistsAssetAtPath(dirPath) )
					// asset dir already exists:
					return false;
				else {
					string newFolderName = Path.GetFileName(dirPath);
					string parentFolder = Path.GetDirectoryName(dirPath);
					string guid = AssetDatabase.CreateFolder(parentFolder, newFolderName);
					return !String.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(guid));
				}
			}
			else {
				if ( Directory.Exists(dirPath) )
					return false;
				else {
					try {
						Directory.CreateDirectory(dirPath);
						return true;
					} catch ( Exception e ) {
						Debug.LogWarning("CreateDir(" + dirPath + ") threw exception: " + e.Message);
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
		public static bool ClearDir (string dirPath) {
			if ( !Files.ExistsDir(dirPath) )
				return false;

			if ( Assets.IsAssetPath(dirPath) )
				return Assets.ClearAssetFolder(dirPath);
			else {
				bool cleaned = true;

				// delete all files:
				string[] files = Directory.GetFiles(dirPath);
				foreach ( string file in files ) {
					File.Delete(file);
					cleaned &= !File.Exists(file);
				}

				// delete all directories:
				string[] dirs = Directory.GetDirectories(dirPath);
				foreach ( string dir in dirs ) {
					Directory.Delete(dir, true);
					cleaned &= !Directory.Exists(dir);
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
		public static bool DeleteDir (string dirPath) {
			// Normalize given path:
			while ( dirPath.EndsWith("/") )
				dirPath = dirPath.Substring(0, dirPath.Length - 2);

			Debug.Log("DeleteDir: " + dirPath);

			if ( Assets.IsAssetPath(dirPath) ) {
				if ( !Assets.ExistsAssetAtPath(dirPath) )
					// asset dir does not exist:
					return false;
				else {
					return AssetDatabase.DeleteAsset(dirPath);
				}
			}
			else {
				if ( !Directory.Exists(dirPath) )
					return false;
				else {
					try {
						Directory.Delete(dirPath, true);
						return !Directory.Exists(dirPath);
					} catch ( Exception e ) {
						Debug.LogWarning("DeleteDir(" + dirPath + ") threw exception: " + e.Message);
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
		public static bool CopyFile (string fromFilePath, string toDirPath, bool overwrite = true) {
			if ( !Files.ExistsFile(fromFilePath) )
				// we can not copy a non existing file:
				return false;

			if ( !Files.ExistsDir(toDirPath) && (!overwrite || !Files.CreateDir(toDirPath)) )
				// target dir does not exist and we shall or can not create it => we give up:
				return false;

			string toFilePath = Files.CombinePath(toDirPath, Files.FileName(fromFilePath));

			if ( !overwrite && Files.ExistsFile(toFilePath) )
				// we shall not overwrite already existing target file:
				return false;

			if ( Assets.IsAssetPath(fromFilePath) && Assets.IsAssetPath(toFilePath) ) {
				// we copy an asset file to an asset file:
				if ( Assets.ExistsAssetAtPath(toFilePath) )
					AssetDatabase.DeleteAsset(toFilePath);
				bool isCopied = AssetDatabase.CopyAsset(fromFilePath, toFilePath);
				AssetDatabase.Refresh();
				return isCopied;
			}
			else {
				// we copy a file / asset to a non-asset target file:
				if ( Assets.IsAssetPath(fromFilePath) ) {
					// if we copy an asset we need to use its absolute file path:
					fromFilePath = Assets.AbsolutePath4Asset(fromFilePath);
				}
				try {
					File.Copy(fromFilePath, toFilePath, true);
					if ( !File.Exists(toFilePath) )
						return false;

					if ( Assets.IsAssetPath(toFilePath) ) {
						AssetDatabase.Refresh();
					}
					return true;

				} catch ( Exception e ) {
					Debug.LogWarning("CopyFile(" + fromFilePath + ", " + toDirPath + ") threw exception: " + e.Message);
					return false;
				}
			}
		}

		/// <summary>
		/// Deletes the file at the given path if it exists.
		/// </summary>
		/// <returns><c>true</c>, if file existed and was deleted, <c>false</c> otherwise.</returns>
		/// <param name="filePath">File path.</param>
		public static bool DeleteFile (string filePath) {
			if ( Assets.IsAssetPath(filePath) ) {
				if ( !Assets.ExistsAssetAtPath(filePath) )
					// asset dir does not exist:
					return false;
				else {
					File.Delete(filePath);
					AssetDatabase.Refresh();
					return !File.Exists(filePath);
				}
			}
			else {
				if ( !File.Exists(filePath) )
					return false;
				else {
					try {
						File.Delete(filePath);
						return !File.Exists(filePath);
					} catch ( Exception e ) {
						Debug.LogWarning("DeleteFile(" + filePath + ") threw exception: " + e.Message);
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
		public static bool MoveFile (string fromFilePath, string toDirPath, bool replace = true) {
			if ( !CopyFile(fromFilePath, toDirPath, replace) )
				return false;

			return DeleteFile(fromFilePath);
		}

		public static bool MoveDir (string fromDirPath, string toDirPath, bool replace = true) {
			if ( CopyDir(fromDirPath, toDirPath, replace) == false )
				return false;

			return DeleteDir(fromDirPath);
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
		public static bool CopyDir (string fromDirPath, string toDirPath, bool replace = true) {
			if ( !ExistsDir(fromDirPath) )
				// nothing to copy:
				return false;

			if ( !ExistsDir(toDirPath) && !CreateDir(toDirPath) )
				//	no target dir given nor can we create it:
				return false;

			string targetDirPath = 
				Files.CombinePath(
					toDirPath,
					Files.DirName(fromDirPath)
				);

			if ( ExistsDir(targetDirPath) ) {
				if ( replace ) {
					// replace the existing target dir:
					ClearDir(targetDirPath);
				}
				else {
					// cancel copying since we shall not replace the existing target dir:
					return false;
				}
			}
			
			if ( Assets.IsAssetPath(fromDirPath) && Assets.IsAssetPath(toDirPath) ) {
				// case Asset -> Asset
				return Assets.copyAssetsDir(fromDirPath, toDirPath);
			}
			else {
				return copyNonAssetsDir(fromDirPath, toDirPath);  
			}
			return false;
		}

		private static bool copyNonAssetsDir (string fromDirPath, string toDirPath, bool recursive = true) {
			bool copied = true;

			// create this folder within the target path:
			DirectoryInfo copiedFolder = Directory.CreateDirectory(Files.CombinePath(toDirPath, Files.DirName(fromDirPath)));
			copied &= copiedFolder.Exists; // copied dir should now exist in fromDir. 

			foreach ( string file in Directory.GetFiles(fromDirPath) ) {

				string fileName = Files.FileName(file);

				if ( !fileName.StartsWith(".") && !fileName.EndsWith(".meta") ) {

					File.Copy(file, Files.CombinePath(copiedFolder.FullName, fileName));
				}
			}

			if ( recursive )
				foreach ( string subDir in Directory.GetDirectories(fromDirPath) ) {

					copied &= copyNonAssetsDir(subDir, copiedFolder.FullName);
				}

			AssetDatabase.Refresh();

			return copied;
		}

		// TODO additional parameter used as filter for excluded content (files and dirs)
		public static bool CopyDirContents (string fromDirPath, string toDirPath, bool replace = true) {
			if ( !ExistsDir(fromDirPath) )
				// nothing to copy:
				return false;

			if ( !ExistsDir(toDirPath) && !CreateDir(toDirPath) )
				//	no target dir given nor can we create it:
				return false;

			if ( ExistsDir(toDirPath) && replace )
				// replace the existing target dir:
				ClearDir(toDirPath);
			else
				// cancel copying since we shall not replace the existing target dir:
				return false;

			if ( Assets.IsAssetPath(fromDirPath) && Assets.IsAssetPath(toDirPath) ) {
				// case Asset -> Asset
				return Assets.CopyAssetsDirContents(fromDirPath, toDirPath, true);
			}
			if ( !Assets.IsAssetPath(fromDirPath) && !Assets.IsAssetPath(toDirPath) ) {
				// case Non-Asset -> Non-Asset
				return copyNonAssetsDir(fromDirPath, toDirPath, replace);  
			}
			return false;
		}

		#endregion

		#region Extensions and Separators

		/// <summary>
		/// Unity uses for path separators on all platforms the forward slash even on Windows.
		/// </summary>
		private const string PATH_ELEMENT_SEPARATOR = "/";
		private static readonly char[] SEPARATORS = new char[] {
			'/'
		};


		/// <summary>
		/// Allowed file extensions for image files (".img", ".jpg", ".psd").
		/// </summary>
		public static string[] ImageExtensions = new string[] {
			".png",
			".jpg",
			".psd"
		};

		public static string StripExtension (string filename) {
			int lastDotIndex = filename.LastIndexOf('.');

			if ( filename.Equals(".") || filename.Equals("..") || lastDotIndex <= 0 )
				return filename;
						
			string strippedFilename = filename.Substring(0, filename.LastIndexOf('.'));

			if ( strippedFilename.Equals("") )
				return filename;
			else
				return strippedFilename;
		}

		public static string Extension (string filename) {
			int lastDotIndex = filename.LastIndexOf('.');

			if ( lastDotIndex <= 0 )
				return "";

			return filename.Substring(lastDotIndex + 1);
		}

		public static string ExtensionSeparator (string filename) {
			int lastDotIndex = filename.LastIndexOf('.');

			if ( filename.Equals(".") || filename.Equals("..") || lastDotIndex <= 0 )
				return "";
			else
				return ".";
		}

		/// <summary>
		/// Combines the path segments given. The first argument can be an absolute path, the follwing are always treated as relative: leading "/" are ignored.
		/// </summary>
		/// <returns>The path.</returns>
		/// <param name="basePath">Base path.</param>
		/// <param name="relPathsToAppend">Rel paths to append.</param>
		public static string CombinePath (string basePath, params string[] relPathsToAppend) {
			StringBuilder combinedPath = new StringBuilder();
			combinedPath.Append(basePath);

			for ( int i = 0; i < relPathsToAppend.Length; i++ ) {
				string curSegment = relPathsToAppend[i].Trim();

				if ( curSegment.Equals("") )
					continue;

				// add separator only if already some path gathered:
				if ( !combinedPath.ToString().Equals("") )
					combinedPath.Append(PATH_ELEMENT_SEPARATOR);

				combinedPath.Append(curSegment);
			}

			return Regex.Replace(combinedPath.ToString(), "/+", PATH_ELEMENT_SEPARATOR);
		}

		public static string FileName (string filePath) {
			string filename = "";
			int lastSlashIndex = filePath.LastIndexOf(PATH_ELEMENT_SEPARATOR);

			if ( lastSlashIndex == -1 )
				// no slash in path => it is the filename:
				return filePath;

			filename = filePath.Substring(lastSlashIndex + 1);
			if ( filename.Equals(".") || filename.Equals("..") )
				return "";
			else
				return filename;
		}

		public static string DirName (string dirPath) {
			// eliminate multi slashes:
			dirPath = Regex.Replace(dirPath, "/+", PATH_ELEMENT_SEPARATOR);
			string[] pathSegments = dirPath.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

			if ( pathSegments.Length == 0 )
				return "";

			int lastIndex = pathSegments.Length - 1;

			if ( pathSegments[lastIndex].Equals(".") ) {
				// get path segement before the dot:
				if ( lastIndex > 0 )
					return pathSegments[lastIndex - 1];
				else
					return "";
			}

			if ( pathSegments[lastIndex].Equals("..") ) {
				// get 2nd last segment before the two dots:
				if ( lastIndex > 1 )
					return pathSegments[lastIndex - 2];
				else
					return "";
			}

			// the "normal" case - return the last segment of the path:
			return pathSegments[lastIndex];
			
		}

		#endregion

		/// <summary>
		/// Clears the directory, i.e. deletes all contained files including subdirectories. 
		/// The directory itself remains empty but is not deleted.
		/// </summary>
		/// <param name="dirPath">Dir path.</param>
		[Obsolete("Use ClearDir() instead which is Asset agnostic.")]
		public static void ClearDirectory (string dirPath) {
			DirectoryInfo downloadedMessageInfo = new DirectoryInfo(dirPath);
			
			foreach ( FileInfo file in downloadedMessageInfo.GetFiles() ) {
				file.Delete(); 
			}

			foreach ( DirectoryInfo dir in downloadedMessageInfo.GetDirectories() ) {
				dir.Delete(true); 
			}
		}

		/// <summary>
		/// Checks whether the given file exists with at least one of the given extensions. 
		/// The argument filePath has to be given WITHOUT any extension.
		/// </summary>
		/// <param name="filePath">File path.</param>
		/// <param name="allowedExtensions">Allowed extensions.</param>
		[Obsolete("Do we really need these image specific functions?")]
		public static bool Exists (string filePath, string[] allowedExtensions) {
			bool exists = false;
			int i = 0;
			while ( !exists && i < allowedExtensions.Length ) {
				exists |= File.Exists(filePath + allowedExtensions[i]);
				i++;
			}
			return exists;
		}

		/// <summary>
		/// Checks whether the given image file exists with at least one of the allowed extensions (cf. ImageExtension). 
		/// The argument filePath has to be given WITHOUT any extension.
		/// </summary>
		/// <param name="filePath">File path.</param>
		[Obsolete("Do we really need these image specific functions?")]
		public static bool ExistsImage (string filePath) {
			return Exists(filePath, ImageExtensions);
		}

		/// <summary>
		/// Delete the specified file (with any of the given extensions).
		/// </summary>
		/// <param name="filePath">File path.</param>
		/// <param name="extensions">Extensions.</param>
		[Obsolete("Do we really need these image specific functions?")]
		public static void Delete (string filePath, string[] extensions) {
			foreach ( string ext in extensions ) {
				if ( File.Exists(filePath + ext) ) {
					File.Delete(filePath + ext);
				}
			}
		}

		/// <summary>
		/// Deletes the image file (with any extension, cf.ImageExtensions).
		/// </summary>
		/// <param name="filePath">File path.</param>
		[Obsolete("Do we really need these image specific functions?")]
		public static void DeleteImage (string filePath) {
			Delete(filePath, ImageExtensions);
		}

		/// <summary>
		/// Copy the specified file from sourceFilePath to targetFilePath. Any given extensions will be copied.
		/// </summary>
		/// <param name="sourceFilePath">Source file path.</param>
		/// <param name="targetFilePath">Target file path.</param>
		/// <param name="extensions">Extensions.</param>
		[Obsolete("Do we really need these image specific functions?")]
		public static void Copy (string sourceFilePath, string targetFilePath, string[] extensions) {
			if ( !Exists(sourceFilePath, extensions) ) {
				// if source file not exists ignore the copy call:
				return;
			}

			// delete all matching target files:
			Delete(targetFilePath, extensions);
			bool copied = false;
			int i = 0;
			while ( !copied && i < extensions.Length ) {
				// copy file with all found extensions:
				if ( File.Exists(sourceFilePath + extensions[i]) ) {
					File.Copy(sourceFilePath + extensions[i], targetFilePath + extensions[i], true);
					copied = true;
				}
				i++;
			}
		}

		/// <summary>
		/// Copies the directory and overwrites if target already exists.
		/// </summary>
		/// <returns><c>true</c>, if directory was copyed, <c>false</c> otherwise.</returns>
		/// <param name="originalDirPath">Original dir path.</param>
		/// <param name="targetPath">Target path must denote the path of the target dir that not exists yet.</param>
		[Obsolete("Use Files.CopyDir() instead.")]
		public static void CopyDirectoryFiles (string originalDirPath, string targetPath) {
			DirectoryInfo origin = new DirectoryInfo(originalDirPath);
			if ( !origin.Exists ) {
				throw new ArgumentException("Can not copy from the non existing directory path: " + originalDirPath);
			}
			DirectoryInfo target = new DirectoryInfo(targetPath);
			if ( target.Exists ) {
				target.Delete(true);
			}

			target.Create();
			foreach ( var file in origin.GetFiles() ) {
				file.CopyTo(target.FullName + PATH_ELEMENT_SEPARATOR + file.Name);
			}
		}

		/// <summary>
		/// Copies the image file from sourceFilePath to targetFilePath. Keeps any allowed extension (cf. ImageExtensions).
		/// </summary>
		/// <param name="sourceFilePath">Source file path.</param>
		/// <param name="targetFilePath">Target file path.</param>
		[Obsolete("Do we really need these image specific functions?")]
		public static void CopyImage (string sourceFilePath, string targetFilePath) {
			Copy(sourceFilePath, targetFilePath, ImageExtensions);
		}

		#region Directory Features

		// TODO Make Asset Agnostic! & Test
		public static bool IsEmptyDir (string dirPath) {
			
			if ( !Directory.Exists(dirPath) )
				return false;
			
			string[] files = Array.FindAll(Directory.GetFiles(dirPath), x => IsNormalFile(x));
			string[] dirs = Directory.GetDirectories(dirPath);
			return (files.Length == 0 && dirs.Length == 0);
		}

		/// <summary>
		/// Checks wether the given file is considered hidden, so it does not count against empty dir for example.
		/// </summary>
		/// <returns><c>true</c>, if hidden file was ised, <c>false</c> otherwise.</returns>
		/// <param name="filePath">File path.</param>
		internal static bool IsNormalFile (string filePath) {
			string fileName = FileName(filePath);

			if ( fileName.StartsWith(".") || fileName.EndsWith(".meta") )
				return false;
			else
				return true;
		}

		/// <summary>
		/// Determines if the given path is valid, i.e. if it is either an absolute or a relative well formed path.
		/// </summary>
		/// <returns><c>true</c> if is valid path the specified path; otherwise, <c>false</c>.</returns>
		/// <param name="path">Path.</param>
		public static bool IsValidPath (string path) {
			
			return isValidFilePath(path) || isValidDirPath(path);
		}

		private static bool isValidFilePath (string path) {
			bool valid = true;

			FileInfo fi = null;
			try {
				fi = new FileInfo(path);
			} catch ( Exception e ) {
				valid = false;
			} 
			valid &= !ReferenceEquals(fi, null);

			return valid;
		}

		private static bool isValidDirPath (string path) {
			bool valid = true;

			DirectoryInfo di = null;
			try {
				di = new DirectoryInfo(path);
			} catch ( Exception e ) {
				valid = false;
			}
			valid &= !ReferenceEquals(di, null);

			return valid;
		}

		/// <summary>
		/// Determines if is parentDirPath is a parent dir path of childDirPath. 
		/// If acceptSame is true, equals a path is accepted as its own parent.
		/// </summary>
		/// <returns><c>true</c> if is parent dir path the specified parentDirPath childDirPath acceptSame; otherwise, <c>false</c>.</returns>
		/// <param name="parentDirPath">Parent dir path.</param>
		/// <param name="childDirPath">Child dir path.</param>
		/// <param name="acceptSame">If set to <c>true</c> accept same.</param>
		public static bool IsParentDirPath (string parentDirPath, string childDirPath, bool acceptSame = false) {
			// eliminate multi slashes:
			parentDirPath = Regex.Replace(parentDirPath, "/+", PATH_ELEMENT_SEPARATOR);
			string[] parentDirPathSegments = parentDirPath.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

			childDirPath = Regex.Replace(childDirPath, "/+", PATH_ELEMENT_SEPARATOR);
			string[] childDirPathSegments = childDirPath.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

			bool isParent = 
				acceptSame ? 
					childDirPathSegments.Length >= parentDirPathSegments.Length : 
					childDirPathSegments.Length > parentDirPathSegments.Length; 

			for ( int i = 0; i < parentDirPathSegments.Length; i++ ) {
				isParent &= childDirPathSegments[i].Equals(parentDirPathSegments[i]);
			}
			return isParent;
		}

		#endregion


		#region Asset Database Related

		public static void StripAssetMetadata (string dir) {
			
			if ( Assets.IsAssetPath(dir) )
				throw new ArgumentException("DO NOT strip metadata from asset files and directories as " + dir);
			
			string[] files = Directory.GetFiles(dir);
			foreach ( var file in files ) {
				if ( file.EndsWith(".meta") )
					File.Delete(file);
			}

			string[] subdirs = Directory.GetDirectories(dir);

			foreach ( var subdir in subdirs ) {
				StripAssetMetadata(subdir);
			}
		}

		#endregion

	}




}
