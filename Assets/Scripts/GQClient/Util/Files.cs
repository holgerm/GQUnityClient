using System.IO;
using UnityEngine;
using System;

namespace GQ.Util {
	public static class Files {

		#region Extensions and Separators

		/// <summary>
		/// Unity uses for path separators on all platforms the forward slash even on Windows.
		/// </summary>
		public const string PATH_ELEMENT_SEPARATOR = "/";

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

		#endregion

		#if !UNITY_WEBPLAYER

		/// <summary>
		/// Clears the directory, i.e. deletes all contained files including subdirectories. 
		/// The directory itself remains empty but is not deleted.
		/// </summary>
		/// <param name="dirPath">Dir path.</param>
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
		public static bool ExistsImage (string filePath) {
			return Exists(filePath, ImageExtensions);
		}

		/// <summary>
		/// Delete the specified file (with any of the given extensions).
		/// </summary>
		/// <param name="filePath">File path.</param>
		/// <param name="extensions">Extensions.</param>
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
		public static void DeleteImage (string filePath) {
			Delete(filePath, ImageExtensions);
		}

		/// <summary>
		/// Copy the specified file from sourceFilePath to targetFilePath. Any given extensions will be copied.
		/// </summary>
		/// <param name="sourceFilePath">Source file path.</param>
		/// <param name="targetFilePath">Target file path.</param>
		/// <param name="extensions">Extensions.</param>
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
		public static void CopyDirectory (string originalDirPath, string targetPath) {
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
		public static void CopyImage (string sourceFilePath, string targetFilePath) {
			Copy(sourceFilePath, targetFilePath, ImageExtensions);
		}

		#endif

	}


	public static class LocalWWW {

		/// <summary>
		/// Use LocalWWW.Create("path/to/my.file") instead of new WWW(pre + Application.persistentDataPath + "path/to/my.file").
		/// </summary>
		/// <param name="localFilePath">Local file path.</param>
		public static WWW Create (string localFilePath) {
			string pre = "file: /";

			if ( Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ) {
				pre = "file:";
			}

			return new WWW(pre + Application.persistentDataPath + localFilePath);
		}

	}

}
