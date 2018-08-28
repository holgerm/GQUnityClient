using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using GQ.Client.Conf;
using GQ.Client.Err;
using GQ.Editor.Util;
using GQTests;
using UnityEngine;

namespace GQ.Editor.Building
{
    public static class AssetAddOnManager
    {


        #region AssetAddOns

        public static void switchAssetAddOns(Config oldConfig, Config newConfig)
        {
            AssetAddOnManager.unloadAssetAddOns(oldConfig, newConfig);
            AssetAddOnManager.loadAssetAddOns(oldConfig, newConfig);
        }


        public static List<string> calculateAAOsToUnload(Config oldConfig, Config newConfig)
        {
            List<string> aaosToUnload = new List<string>();

            foreach (string oldAAO in oldConfig.assetAddOns)
            {
                if (!Array.Exists(
                    newConfig.assetAddOns,
                    newAAO => newAAO == oldAAO
                ))
                {
                    aaosToUnload.Add(oldAAO);
                }
            }

            return aaosToUnload;
        }

        public static List<string> calculateAAOsToLoad(Config oldConfig, Config newConfig)
        {
            return calculateAAOsToUnload(newConfig, oldConfig);
        }

        public static void unloadAssetAddOns(Config oldProdConfig, Config newProdConfig)
        {
            List<string> assetAddOns = calculateAAOsToUnload(oldProdConfig, newProdConfig);
            foreach (string assetAddOn in assetAddOns)
            {
                unloadAaoRecursively(assetAddOn);

                deleteAaoSectionFromGitignore(assetAddOn);
            }
        }

        private static void unloadAaoRecursively(string assetAddOn, string relPath = "")
        {
            Debug.Log("AAO UN-loading: " + relPath);

            // recursively go into every dir in the AssetAddOn tree:
            string aaoPath = Files.CombinePath(ASSET_ADD_ON_DIR_PATH, assetAddOn, relPath);
            foreach (string dir in Directory.GetDirectories(aaoPath))
            {
                unloadAaoRecursively(assetAddOn, Files.CombinePath(relPath, Files.DirName(dir)));
            }

            // set the according dir within Assets:
            string assetDir = Files.CombinePath(Application.dataPath, relPath);
            foreach (string file in Directory.GetFiles(aaoPath))
            {
                Debug.Log("AAO Deleting file: " + file);
                string assetFile = Files.CombinePath(assetDir, Files.FileName(file));
                Files.DeleteFile(assetFile);
            }

            // shall this directory be deleted?:
            if (!Directory.Exists(assetDir))
                // continue recursion if this dir does not exist (e.g. manually deleted)
                return;

            if (!Array.Exists(
                Directory.GetFiles(assetDir),
                element => Files.FileName(element).StartsWith(AAO_MARKERFILE_PREFIX, StringComparison.CurrentCulture)
            ))
            {
                // case 1: no marker file, i.e. this dir is independent of AAOs and is kept. We reduce gitignore.
                Debug.Log(string.Format("Dir {0} does NOT contain AAO Marker and is kept.", assetDir));
                //deleteAaoSectionFromGitignore(assetDir, assetAddOn);
            }
            else
            {
                Debug.Log(string.Format("Dir {0} DOES contain AAO Marker ...", assetDir));
                if (File.Exists(Files.CombinePath(assetDir, AAO_MARKERFILE_PREFIX + assetAddOn)))
                {
                    // this dir depended on the current AAO, hence we delete the according marker file:
                    bool deleted = Files.DeleteFile(Files.CombinePath(assetDir, AAO_MARKERFILE_PREFIX + assetAddOn));
                    Debug.Log(string.Format("Dir {0} contains our AAO Marker {1} and has been deleted: {2}.",
                                            assetDir, AAO_MARKERFILE_PREFIX + assetAddOn, deleted));
                }
                if (Array.Exists(
                    Directory.GetFiles(assetDir),
                    element => Files.FileName(element).StartsWith(AAO_MARKERFILE_PREFIX, StringComparison.CurrentCulture)
                ))
                {
                    // case 2: this dir depends also on other AAOs and is kept. We reduce the gitignore.
                    Debug.Log(string.Format("Dir {0} contains other AAO Markers hence we keep it.", assetDir));
                    //deleteAaoSectionFromGitignore(assetDir, assetAddOn);
                }
                else
                {
                    // case 3: this dir depended only on this AAO, hence we can delete it (including the gitignore)
                    Debug.Log(string.Format("Dir {0} contains NO other AAO Markers hence we DELETE it.", assetDir));
                    Files.DeleteDir(assetDir);
                }
            }

        }

        private static void deleteAaoSectionFromGitignore(string assetAddOn)
        {
            if (!File.Exists(Files.GIT_EXCLUDE_FILE))
            {
                File.Create(Files.GIT_EXCLUDE_FILE);
            }

            string gitSectionRegExp =
                @"([\s\S]*)(# BEGIN GQ AAO: " + assetAddOn +
                @"[\s\S]*# END GQ AAO: " + assetAddOn +
                @")([\s\S]*)";

            string gitignoreText = File.ReadAllText(Files.GIT_EXCLUDE_FILE);

            Regex regex = new Regex(gitSectionRegExp);
            Match match = regex.Match(gitignoreText);
            if (match.Success)
            {
                string before = match.Groups[1].Value;
                string after = match.Groups[3].Value;
                gitignoreText = before + "\n" + after;
                Debug.Log("deleteAaoSectionFromGitignore: MATCHES: before: " + before + ", after: " + after);
            }
            else
            {
                Debug.Log("deleteAaoSectionFromGitignore: DID NOT MATCH");
            }

            gitignoreText = gitignoreText.Trim();

            if (gitignoreText.Length > 0)
            {
                File.WriteAllText(Files.GIT_EXCLUDE_FILE, gitignoreText);
            }
        }

        public static void loadAssetAddOns(Config oldProdConfig, Config newProdConfig)
        {
            List<string> gitignorePatterns = new List<string>();

            List<string> assetAddOns = calculateAAOsToLoad(oldProdConfig, newProdConfig);
            foreach (string assetAddOn in assetAddOns)
            {
                loadAaoRecursively(assetAddOn, gitignorePatterns, true);

                if (gitignorePatterns.Count > 0)
                {
                    // Store additions to git exclude file:
                    string gitExcludeSection = "\n\n# BEGIN GQ AAO: " + assetAddOn + "\n";
                    foreach (string pattern in gitignorePatterns)
                    {
                        gitExcludeSection += pattern + "\n";
                    }
                    gitExcludeSection += "# END GQ AAO: " + assetAddOn;
                    File.AppendAllText(Files.GIT_EXCLUDE_FILE, gitExcludeSection);
                }
            }

        }

        /// <summary>
        /// Loads the aao recursively.
        /// </summary>
        /// <returns><c>true</c>, if the current directory was created by this AAO and 
        /// should therefore be included in gitignore, <c>false</c> otherwise.</returns>
        /// <param name="assetAddOn">Asset add on.</param>
        /// <param name="gitCollectIgnores">Flag controlling wether gitignores are collected (not within created and completely ignored dirs).</param>
        /// <param name="relPath">Rel path.</param>
        private static void loadAaoRecursively(string assetAddOn, List<string> gitignorePatterns, bool gitCollectIgnores, string relPath = "")
        {
            bool gitCollectIgnoresInSubdirs = gitCollectIgnores;
            Debug.Log("AAO Loading: " + relPath);
            // set the according dir within Assets:
            string assetDir = Files.CombinePath(Application.dataPath, relPath);
            // if dir does not exist in asstes create and mark it:
            if (!Directory.Exists(assetDir))
            {
                // this dir does not exist yet, we create it, mark it and collect git ignores if we are not in an already ignored subdir:
                Files.CreateDir(assetDir);
                string dirMarkerFile = Files.CombinePath(assetDir, AAO_MARKERFILE_PREFIX + assetAddOn);
                File.Create(dirMarkerFile);
                if (gitCollectIgnores)
                {
                    gitignorePatterns.Add(Assets.RelativeAssetPath(assetDir) + "/");
                    gitignorePatterns.Add(Assets.RelativeAssetPath(assetDir) + ".meta");
                    gitCollectIgnoresInSubdirs = false;
                }
            }
            else
            {
                if (Array.Exists(
                    Directory.GetFiles(assetDir),
                    element => Files.FileName(element).StartsWith(AAO_MARKERFILE_PREFIX, StringComparison.CurrentCulture)
                ))
                {
                    // this dir has been created by another aao so we add our marker file:
                    string newAAOMarkerfile = Files.CombinePath(assetDir, AAO_MARKERFILE_PREFIX + assetAddOn);
                    File.Create(newAAOMarkerfile);
                    if (gitCollectIgnores)
                        gitignorePatterns.Add(Assets.RelativeAssetPath(newAAOMarkerfile));
                }
            }
            // copy all files from corresponding aao dir into this asset dir:
            string aaoPath = Files.CombinePath(ASSET_ADD_ON_DIR_PATH, assetAddOn, relPath);
            foreach (string file in Directory.GetFiles(aaoPath))
            {
                if (file.EndsWith(".DS_Store", StringComparison.CurrentCulture))
                {
                    continue;
                }
                if (file.EndsWith(AAO_MARKERFILE_PREFIX + assetAddOn, StringComparison.CurrentCulture))
                {
                    Log.SignalErrorToDeveloper(
                        "Asset-Add-On {0} is not compatible with our add-on-system: it includes itself an .AssetAddOn file.",
                        AAO_MARKERFILE_PREFIX + assetAddOn);
                }
                if (File.Exists(Files.CombinePath(assetDir, Files.FileName(file))))
                {
                    Log.SignalErrorToDeveloper(
                        "Asset-Add-On {0} is not compatible with another add-on or our add-on-system: the file {1} already exists and will not be overridden.",
                        assetAddOn,
                        Files.CombinePath(assetDir, Files.FileName(file))
                    );
                }
                Debug.Log("AAO File Copy: " + file);
                Files.CopyFile(
                    fromFilePath: file,
                    toDirPath: assetDir,
                    overwrite: false
                );
                if (gitCollectIgnores)
                {
                    gitignorePatterns.Add(
                    Files.CombinePath(Assets.RelativeAssetPath(assetDir), Files.FileName(file)));
                    gitignorePatterns.Add(
                        Files.CombinePath(Assets.RelativeAssetPath(assetDir), Files.FileName(file) + ".meta"));
                }
            }
            // recursively go into every dir in the AssetAddOn tree:
            foreach (string dir in Directory.GetDirectories(aaoPath))
            {
                loadAaoRecursively(assetAddOn, gitignorePatterns, gitCollectIgnoresInSubdirs, Files.CombinePath(relPath, Files.DirName(dir)));
            }
        }

        /// <summary>
        /// In this directory all defined AsssetAddOns are stored.
        /// </summary>
        private static string ASSET_ADD_ON_DIR_PATH = Files.CombinePath(GQAssert.PROJECT_PATH, "Production/AssetAddOns/");

        private static string AAO_MARKERFILE_PREFIX = ".AssetAddOn_";

        #endregion
    }
}