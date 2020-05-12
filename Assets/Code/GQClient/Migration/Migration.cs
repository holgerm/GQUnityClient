using System;
using System.Collections.Generic;
using Code.GQClient.Err;
using Code.GQClient.Model;
using Code.GQClient.Util.tasks;
using UnityEngine;

namespace Code.GQClient.Migration
{
    public abstract class Migration : Task
    {
        protected abstract string Version { get; }
        
        public static string Title  => "Aktualisierung der App";
        
        public abstract string Details { get; }

        private bool Applicable => string.CompareOrdinal(OldAppVersion, Version) < 0;

        private static int LowerVersionsFirst(Migration x, Migration y)
        {
            return string.CompareOrdinal(x.Version, y.Version);
        }

        private static void ApplyMigrations()
        {
            var assembly = typeof(Migration).Assembly;
            var types = assembly.GetTypes();
            var migrations = new List<Migration>();
            
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(Migration)))
                {
                    if (Activator.CreateInstance(type) is Migration migration)
                    {
                        //   var _ = new SimpleDialogBehaviour(migration, Migration.Title, migration.Details);
                        if (migration.Applicable)
                            migrations.Add(migration);
                    }
                    else
                    {
                        Log.SignalErrorToDeveloper($"Migration {type.FullName} missing default constructor.");
                    }
                }
            }
            
            migrations.Sort(Migration.LowerVersionsFirst);
            foreach (var migration in migrations)
            {
                migration.Start();
            }
        }

        private static string _currentAppVersion;

        public static string CurrentAppVersion
        {
            get
            {
                if (!string.IsNullOrEmpty(_currentAppVersion))
                    return _currentAppVersion;

                var buildtimeTextAsset = Resources.Load<TextAsset>("buildtime");
                if (buildtimeTextAsset == null)
                {
                    Log.SignalErrorToDeveloper("Buildtime not found.");
                    return "";
                }

                _currentAppVersion = buildtimeTextAsset.text;
                // just use the version number, e.g. "2.20.05.05 (05.05.2020 09:36:27)" ==> "2.20.05.05"
                _currentAppVersion = _currentAppVersion.Substring(
                    0, _currentAppVersion.IndexOf(" (", StringComparison.Ordinal));

                if (string.CompareOrdinal(OldAppVersion, _currentAppVersion) < 0)
                {
                    ApplyMigrations();
                }

                PlayerPrefs.SetString(Definitions.GQ_PLAYERPREF_BUILDTIME, _currentAppVersion);
                PlayerPrefs.Save();
                return _currentAppVersion;
            }
        }

        private static string _oldAppVersion;

        private static string OldAppVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_oldAppVersion))
                {
                    _oldAppVersion = PlayerPrefs.HasKey(Definitions.GQ_PLAYERPREF_BUILDTIME)
                        ? PlayerPrefs.GetString(Definitions.GQ_PLAYERPREF_BUILDTIME)
                        : "0.0.0.0";
                }

                return _oldAppVersion;
         }
        }
    }
    
}