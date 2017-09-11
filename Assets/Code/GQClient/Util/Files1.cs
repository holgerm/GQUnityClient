using System.IO;
using UnityEngine;
using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

namespace GQ.Client.Util
{
	
	public static class Files1
	{

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

		public static string StripExtension (string filename)
		{
			int lastDotIndex = filename.LastIndexOf ('.');

			if (filename.Equals (".") || filename.Equals ("..") || lastDotIndex <= 0)
				return filename;
						
			string strippedFilename = filename.Substring (0, filename.LastIndexOf ('.'));

			if (strippedFilename.Equals (""))
				return filename;
			else
				return strippedFilename;
		}

		public static string Extension (string filename)
		{
			int lastDotIndex = filename.LastIndexOf ('.');

			if (lastDotIndex <= 0)
				return "";

			return filename.Substring (lastDotIndex + 1);
		}

		public static string ExtensionSeparator (string filename)
		{
			int lastDotIndex = filename.LastIndexOf ('.');

			if (filename.Equals (".") || filename.Equals ("..") || lastDotIndex <= 0)
				return "";
			else
				return ".";
		}

		#endregion

		#region Paths

		/// <summary>
		/// Combines the path segments given. The first argument can be an absolute path, the follwing are always treated as relative: leading "/" are ignored.
		/// </summary>
		/// <returns>The path.</returns>
		/// <param name="basePath">Base path.</param>
		/// <param name="relPathsToAppend">Rel paths to append.</param>
		public static string CombinePath (string basePath, params string[] relPathsToAppend)
		{
			StringBuilder combinedPath = new StringBuilder ();
			combinedPath.Append (basePath);

			for (int i = 0; i < relPathsToAppend.Length; i++) {
				string curSegment = relPathsToAppend [i].Trim ();

				if (curSegment.Equals (""))
					continue;

				// add separator only if already some path gathered:
				if (!combinedPath.ToString ().Equals (""))
					combinedPath.Append (PATH_ELEMENT_SEPARATOR);

				combinedPath.Append (curSegment);
			}

			return Regex.Replace (combinedPath.ToString (), "/+", PATH_ELEMENT_SEPARATOR);
		}

		public static string LocalPath4WWW(string assetRelativePath) 
		{
			string path;

			if (assetRelativePath.StartsWith ("Assets"))
				assetRelativePath = assetRelativePath.Substring ("Assets".Length);

			if (!assetRelativePath.StartsWith ("/"))
				assetRelativePath = "/" + assetRelativePath;

			return "file://" + Application.dataPath + assetRelativePath;
		}

		public static string FileName (string filePath)
		{
			string filename = "";
			int lastSlashIndex = filePath.LastIndexOf (PATH_ELEMENT_SEPARATOR);

			if (lastSlashIndex == -1)
				// no slash in path => it is the filename:
				return filePath;

			filename = filePath.Substring (lastSlashIndex + 1);
			if (filename.Equals (".") || filename.Equals (".."))
				return "";
			else
				return filename;
		}

		public static string DirName (string dirPath)
		{
			// eliminate multi slashes:
			dirPath = Regex.Replace (dirPath, "/+", PATH_ELEMENT_SEPARATOR);
			string[] pathSegments = dirPath.Split (SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

			if (pathSegments.Length == 0)
				return "";

			int lastIndex = pathSegments.Length - 1;

			if (pathSegments [lastIndex].Equals (".")) {
				// get path segement before the dot:
				if (lastIndex > 0)
					return pathSegments [lastIndex - 1];
				else
					return "";
			}

			if (pathSegments [lastIndex].Equals ("..")) {
				// get 2nd last segment before the two dots:
				if (lastIndex > 1)
					return pathSegments [lastIndex - 2];
				else
					return "";
			}

			// the "normal" case - return the last segment of the path:
			return pathSegments [lastIndex];
		}

		/// <summary>
		/// Returns the path to the parent dir of the given filePath. If the given filePath denotes the root dir, an argument Exception is thrown.
		/// </summary>
		/// <returns>The dir.</returns>
		/// <param name="filePath">File path.</param>
		public static string ParentDir (string filePath)
		{
			// eliminate trailing slash:
			if (filePath.EndsWith ("/") || filePath.EndsWith ("/."))
				filePath = filePath.Substring (0, filePath.Length - 1);
			
			// eliminate multi slashes:
			filePath = Regex.Replace (filePath, "/+", PATH_ELEMENT_SEPARATOR);

			// eliminate dot segments:
			filePath = Regex.Replace (filePath, "^.\\./", "");

			// reduce path for double dot segments:
			filePath = Regex.Replace (filePath, "(/?)\\w+/\\.\\.", "");

			// short cut to simple pathological cases:
			if (filePath.Equals ("/") ||
			    filePath.Equals ("") ||
			    filePath.Equals (".") ||
			    filePath.Equals ("/.") ||
			    filePath.Equals ("..") ||
			    filePath.Equals ("/.."))
				throw new ArgumentException ("No parent exists for given filePath.");

			string[] pathSegments = filePath.Split (SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

			if (pathSegments.Length == 0)
				throw new ArgumentException ("No parent exists for given filePath.");

			if (pathSegments.Length == 1)
				return "/";

			StringBuilder result = new StringBuilder ();

			if (filePath.StartsWith ("/"))
				result.Append ("/");

			for (int i = 0; i < pathSegments.Length - 1; i++) {
				result.Append (pathSegments [i]);
				result.Append (PATH_ELEMENT_SEPARATOR);
			}

			return result.ToString ();
		}

		#endregion

		#region Directory Features

		// TODO Make Asset Agnostic! & Test
		public static bool IsEmptyDir (string dirPath)
		{
			
			if (!Directory.Exists (dirPath))
				return false;
			
			string[] files = Array.FindAll (Directory.GetFiles (dirPath), x => IsNormalFile (x));
			string[] dirs = Directory.GetDirectories (dirPath);
			return (files.Length == 0 && dirs.Length == 0);
		}

		/// <summary>
		/// Checks wether the given file is considered hidden, so it does not count against empty dir for example.
		/// </summary>
		/// <returns><c>true</c>, if hidden file was ised, <c>false</c> otherwise.</returns>
		/// <param name="filePath">File path.</param>
		internal static bool IsNormalFile (string filePath)
		{
			string fileName = FileName (filePath);

			if (fileName.StartsWith (".") || fileName.EndsWith (".meta"))
				return false;
			else
				return true;
		}

		/// <summary>
		/// Determines if the given path is valid, i.e. if it is either an absolute or a relative well formed path.
		/// </summary>
		/// <returns><c>true</c> if is valid path the specified path; otherwise, <c>false</c>.</returns>
		/// <param name="path">Path.</param>
		public static bool IsValidPath (string path)
		{
			
			return isValidFilePath (path) || isValidDirPath (path);
		}

		private static bool isValidFilePath (string path)
		{
			bool valid = true;

			FileInfo fi = null;
			try {
				fi = new FileInfo (path);
			} catch (Exception e) {
				valid = false;
			} 
			valid &= !ReferenceEquals (fi, null);

			return valid;
		}

		private static bool isValidDirPath (string path)
		{
			bool valid = true;

			DirectoryInfo di = null;
			try {
				di = new DirectoryInfo (path);
			} catch (Exception e) {
				valid = false;
			}
			valid &= !ReferenceEquals (di, null);

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
		public static bool IsParentDirPath (string parentDirPath, string childDirPath, bool acceptSame = false)
		{
			// eliminate multi slashes:
			parentDirPath = Regex.Replace (parentDirPath, "/+", PATH_ELEMENT_SEPARATOR);
			string[] parentDirPathSegments = parentDirPath.Split (SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

			childDirPath = Regex.Replace (childDirPath, "/+", PATH_ELEMENT_SEPARATOR);
			string[] childDirPathSegments = childDirPath.Split (SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

			bool isParent = 
				acceptSame ? 
					childDirPathSegments.Length >= parentDirPathSegments.Length : 
					childDirPathSegments.Length > parentDirPathSegments.Length; 

			for (int i = 0; i < parentDirPathSegments.Length; i++) {
				isParent &= childDirPathSegments [i].Equals (parentDirPathSegments [i]);
			}
			return isParent;
		}

		#endregion

	}

}
