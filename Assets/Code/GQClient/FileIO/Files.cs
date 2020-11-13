using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Code.GQClient.Err;
using Code.QM.Util;
using UnityEngine;

namespace Code.GQClient.FileIO
{
    public class Files
    {
        #region Extensions and Separators

        /// <summary>
        /// Unity uses for path separators on all platforms the forward slash even on Windows.
        /// </summary>
        protected const string PATH_ELEMENT_SEPARATOR = "/";

        protected static readonly char[] SEPARATORS =
        {
            '/'
        };

        private static readonly string SAME_DIR = ".";

        private static readonly string PARENT_DIR = "..";


        public static string StripExtension(string filename)
        {
            if (null == filename) 
                return null;
            
            var lastDotIndex = filename.LastIndexOf('.');

            if (filename.Equals(".") || filename.Equals("..") || lastDotIndex <= 0)
                return filename;

            var strippedFilename = filename.Substring(0, filename.LastIndexOf('.'));

            if (strippedFilename.Equals(""))
                return filename;
            else
                return strippedFilename;
        }

        /// <summary>
        /// Gives you the filename extension without the leading dot, e.g. "txt", "png", "wav".
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string Extension(string filename)
        {
            var lastDotIndex = filename.LastIndexOf('.');

            if (lastDotIndex <= 0)
                return "";

            return filename.Substring(lastDotIndex + 1);
        }

        public static string ExtensionSeparator(string filename)
        {
            var lastDotIndex = filename.LastIndexOf('.');

            if (filename.Equals(".") || filename.Equals("..") || lastDotIndex <= 0)
                return "";
            else
                return ".";
        }

        #endregion

        #region Paths

        /// <summary>
        /// Combines the path segments given. The first argument can be an absolute path, the following are always treated as relative: leading "/" are ignored.
        /// </summary>
        /// <returns>The path.</returns>
        /// <param name="basePath">Base path.</param>
        /// <param name="relPathsToAppend">Rel paths to append.</param>
        public static string CombinePath(string basePath, params string[] relPathsToAppend)
        {
            var combinedPath = new StringBuilder();
            combinedPath.Append(basePath);

            foreach (var t in relPathsToAppend)
            {
                var curSegment = t.Trim();

                if (curSegment.Equals(""))
                    continue;

                // add separator only if already some path gathered:
                if (!combinedPath.ToString().Equals(""))
                    combinedPath.Append(PATH_ELEMENT_SEPARATOR);

                combinedPath.Append(curSegment);
            }

            var fullPath =
                Regex.Replace(combinedPath.ToString(), "/+", PATH_ELEMENT_SEPARATOR);

            // eliminate ".." parent dir short cuts in the path:
            var segments = fullPath.Split(new string[] {PATH_ELEMENT_SEPARATOR}, StringSplitOptions.None);
            var optimizedPath = new StringBuilder();
            for (var i = 0; i < segments.Length; i++)
            {
                if (segments[i] == SAME_DIR)
                    if (segments.Length > i + 1)
                        i++;
                    else
                        continue;


                if (segments.Length > i + 1 && segments[i + 1] == PARENT_DIR)
                {
                    i++;
                }
                else
                {
                    if (i > 0)
                        optimizedPath.Append(PATH_ELEMENT_SEPARATOR);
                    optimizedPath.Append(segments[i]);
                }
            }

            return optimizedPath.ToString();
        }

        public static string LocalPath4WWW(string assetRelativePath)
        {
            if (assetRelativePath.StartsWith("Assets", StringComparison.CurrentCulture))
                assetRelativePath = assetRelativePath.Substring("Assets".Length);

            if (!assetRelativePath.StartsWith("/", StringComparison.CurrentCulture))
                assetRelativePath = "/" + assetRelativePath;

            if (!assetRelativePath.StartsWith(Application.dataPath, StringComparison.CurrentCulture))
                assetRelativePath = Application.dataPath + assetRelativePath;

            return "file://" + assetRelativePath;
        }

        public static string AbsoluteLocalPath(string relPath)
        {
            if (!relPath.StartsWith(Device.GetPersistentDatapath(), StringComparison.CurrentCulture))
            {
                relPath = CombinePath(Device.GetPersistentDatapath(), relPath);
            }

            if (Application.isEditor)
                return "file://" + relPath;
            else if (Application.platform == RuntimePlatform.Android)
                return "file://" + relPath;
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return "file://" + relPath;
            }
            else
                return "file://" + relPath;
        }

        public static string FileName(string filePath)
        {
            if (filePath == null)
                return null;

            string filename = "";
            int lastSlashIndex = filePath.LastIndexOf(PATH_ELEMENT_SEPARATOR, StringComparison.CurrentCulture);

            if (lastSlashIndex == -1)
                // no slash in path => it is the filename:
                return filePath;

            filename = filePath.Substring(lastSlashIndex + 1);
            if (filename.Equals(".") || filename.Equals(".."))
                return "";
            else
                return filename;
        }

        public static string DirName(string dirPath)
        {
            // eliminate multi slashes:
            dirPath = Regex.Replace(dirPath, "/+", PATH_ELEMENT_SEPARATOR);
            string[] pathSegments = dirPath.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

            if (pathSegments.Length == 0)
                return "";

            int lastIndex = pathSegments.Length - 1;

            if (pathSegments[lastIndex].Equals("."))
            {
                // get path segment before the dot:
                if (lastIndex > 0)
                    return pathSegments[lastIndex - 1];
                else
                    return "";
            }

            if (pathSegments[lastIndex].Equals(".."))
            {
                // get 2nd last segment before the two dots:
                if (lastIndex > 1)
                    return pathSegments[lastIndex - 2];
                else
                    return "";
            }

            // the "normal" case - return the last segment of the path:
            return pathSegments[lastIndex];
        }

        /// <summary>
        /// Returns the path to the parent dir of the given filePath. If the given filePath denotes the root dir, an argument Exception is thrown.
        /// </summary>
        /// <returns>The dir.</returns>
        /// <param name="filePath">File path.</param>
        public static string ParentDir(string filePath)
        {
            // eliminate trailing slash:
            if (filePath.EndsWith("/", StringComparison.CurrentCulture) ||
                filePath.EndsWith("/.", StringComparison.CurrentCulture))
                filePath = filePath.Substring(0, filePath.Length - 1);

            // eliminate multi slashes:
            filePath = Regex.Replace(filePath, "/+", PATH_ELEMENT_SEPARATOR);

            // eliminate dot segments:
            filePath = Regex.Replace(filePath, "^.\\./", "");

            // reduce path for double dot segments:
            filePath = Regex.Replace(filePath, "(/?)\\w+/\\.\\.", "");

            // short cut to simple pathological cases:
            if (filePath.Equals("/") ||
                filePath.Equals("") ||
                filePath.Equals(".") ||
                filePath.Equals("/.") ||
                filePath.Equals("..") ||
                filePath.Equals("/.."))
                throw new ArgumentException("No parent exists for given filePath.");

            string[] pathSegments = filePath.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

            if (pathSegments.Length == 0)
                throw new ArgumentException("No parent exists for given filePath.");

            if (pathSegments.Length == 1)
                return "/";

            StringBuilder result = new StringBuilder();

            if (filePath.StartsWith("/", StringComparison.CurrentCulture))
                result.Append("/");

            for (int i = 0; i < pathSegments.Length - 1; i++)
            {
                result.Append(pathSegments[i]);
                result.Append(PATH_ELEMENT_SEPARATOR);
            }

            return result.ToString();
        }

        #endregion

        #region Directory Features

        // TODO Make Asset Agnostic! & Test
        public static bool IsEmptyDir(string dirPath)
        {
            if (!Directory.Exists(dirPath))
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
        public static bool IsNormalFile(string filePath)
        {
            string fileName = FileName(filePath);

            if (fileName.StartsWith(".", StringComparison.CurrentCulture) ||
                fileName.EndsWith(".meta", StringComparison.CurrentCulture))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Determines if the given path is valid, i.e. if it is either an absolute or a relative well formed path.
        /// </summary>
        /// <returns><c>true</c> if is valid path the specified path; otherwise, <c>false</c>.</returns>
        /// <param name="path">Path.</param>
        public static bool IsValidPath(string path)
        {
            return isValidFilePath(path) || isValidDirPath(path);
        }

        private static bool isValidFilePath(string path)
        {
            bool valid = true;

            FileInfo fi = null;
            try
            {
                fi = new FileInfo(path);
            }
            catch (Exception)
            {
                valid = false;
            }

            valid &= !ReferenceEquals(fi, null);

            return valid;
        }

        private static bool isValidDirPath(string path)
        {
            bool valid = true;

            DirectoryInfo di = null;
            try
            {
                di = new DirectoryInfo(path);
            }
            catch (Exception)
            {
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
        public static bool IsParentDirPath(string parentDirPath, string childDirPath, bool acceptSame = false)
        {
            // eliminate multi slashes:
            parentDirPath = Regex.Replace(parentDirPath, "/+", PATH_ELEMENT_SEPARATOR);
            string[] parentDirPathSegments = parentDirPath.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

            childDirPath = Regex.Replace(childDirPath, "/+", PATH_ELEMENT_SEPARATOR);
            string[] childDirPathSegments = childDirPath.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);

            bool isParent =
                acceptSame
                    ? childDirPathSegments.Length >= parentDirPathSegments.Length
                    : childDirPathSegments.Length > parentDirPathSegments.Length;

            for (int i = 0; i < parentDirPathSegments.Length; i++)
            {
                isParent &= childDirPathSegments[i].Equals(parentDirPathSegments[i]);
            }

            return isParent;
        }

        public static void DeleteDirCompletely(string path)
        {
            if (!Directory.Exists(path))
            {
                Log.WarnDeveloper("Trying to delete non existing directory " + path);
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                DeleteDirCompletely(subdir.FullName);
            }

            dir.Delete();
        }

        #endregion

        #region Write

        /// <summary>
        /// Enforced version of File.WriteAllText
        /// </summary>
        public static void WriteAllText(string filePath, string content)
        {
            string dir = ParentDir(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(filePath, content);
        }

        #endregion
    }
}