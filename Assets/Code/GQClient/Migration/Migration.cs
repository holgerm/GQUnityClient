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

        private bool Applicable => string.CompareOrdinal(BuildTimeText, Version) < 0;

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

        private static string _buildTimeText;

        public static string BuildTimeText
        {
            get
            {
                if (!String.IsNullOrEmpty(_buildTimeText))
                    return _buildTimeText;

                var buildtimeTextAsset = Resources.Load<TextAsset>("buildtime");
                if (buildtimeTextAsset == null)
                {
                    Log.SignalErrorToDeveloper("Buildtime not found.");
                    return "";
                }

                _buildTimeText = buildtimeTextAsset.text;
                // just use the version number, e.g. "2.20.05.05 (05.05.2020 09:36:27)" ==> "2.20.05.05"
                _buildTimeText = _buildTimeText.Substring(
                    0, _buildTimeText.IndexOf(" (", StringComparison.Ordinal));

                var oldBuildTime = PlayerPrefs.HasKey(Definitions.GQ_PLAYERPREF_BUILDTIME)
                    ? PlayerPrefs.GetString(Definitions.GQ_PLAYERPREF_BUILDTIME)
                    : "0.0.0.0";
                if (string.CompareOrdinal(oldBuildTime, _buildTimeText) < 0)
                {
                    Migration.ApplyMigrations();
                }

                PlayerPrefs.SetString(Definitions.GQ_PLAYERPREF_BUILDTIME, _buildTimeText);
                PlayerPrefs.Save();
                return _buildTimeText;
            }
        }
    }
    
}