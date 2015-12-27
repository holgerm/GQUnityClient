using System.IO;

namespace GQ.Util {
	public static class Files {

		/// <summary>
		/// Allowed file extensions for image files (".img", ".jpg", ".psd").
		/// </summary>
		public static string[] ImageExtensions = new string[] {
			".png",
			".jpg",
			".psd"
		};

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
					File.Copy(sourceFilePath + extensions[i], targetFilePath + extensions[i]);
				}
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
}
