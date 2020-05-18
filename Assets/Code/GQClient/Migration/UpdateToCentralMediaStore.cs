using System;
using System.Collections;
using System.IO;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using Code.GQClient.Model.mgmt.quests;
using GQClient.Model;
using Newtonsoft.Json;

namespace Code.GQClient.Migration
{
    // ReSharper disable once UnusedType.Global
    public class UpdateToCentralMediaStore : Migration
    {
        protected override string Version => "2.20.05.07";
        public override string Details => "Wir müssen etwas aufräumen für die neue App Version. Das dauert ein wenig.";

        protected override IEnumerator DoTheWork()
        {
            var globalFilesDirPath = Files.CombinePath(
                QuestInfoManager.LocalQuestsPath,
                "files");
            if (!Directory.Exists(globalFilesDirPath))
                Directory.CreateDirectory(globalFilesDirPath);

            var migratedQuests = 0;

            // go through each quest folder and deserialize the game.xml
            foreach (var questPath in Directory.GetDirectories(QuestInfoManager.LocalQuestsPath))
            {
                var gameFilePath = Files.CombinePath(questPath, "game.xml");
                var questFilesDirPath = Files.CombinePath(questPath, "files");
                if (!File.Exists(gameFilePath) || !Directory.Exists(questFilesDirPath) ||
                    Files.IsEmptyDir(questFilesDirPath))
                    continue; // skip dirs without a game inside

                var gameXml = File.ReadAllText(gameFilePath);

                yield return null;

                var quest = QuestManager.DeserializeQuest(gameXml);

                yield return null;

                foreach (var mediaInfo in quest.MediaStore.Values)
                {
                    var oldFileName = mediaInfo.LocalFileName;
                    var newFileName = QuestManager.Instance.IncreaseMediaUsage(mediaInfo);
                    if (newFileName == null)
                        continue;

                    // copy media file to global file directory under the new name
                    var sourceFilePath = Files.CombinePath(questFilesDirPath, oldFileName);
                    if (File.Exists(sourceFilePath))
                    {
                        File.Copy(sourceFilePath, Files.CombinePath(globalFilesDirPath, newFileName));
                        yield return null;
                    }
                }

                Files.DeleteDirCompletely(questFilesDirPath);
                migratedQuests++;
            }

            Log.InformDeveloper($"Migrated using {GetType().Name} {migratedQuests} quests to version {Version}");

            //     last. in media.json delete all dir parts for each entry  
            var mediaList = QuestManager.Instance.GetListOfGlobalMediaInfos();

            try
            {
                var mediaJson =
                    (mediaList.Count == 0)
                        ? "[]"
                        : JsonConvert.SerializeObject(mediaList, Newtonsoft.Json.Formatting.Indented);
                Files.WriteAllText(QuestManager.GlobalMediaJsonPath, mediaJson);
                QuestManager.Instance.MediaStoreIsDirty = false;
            }
            catch (Exception e)
            {
                Log.SignalErrorToDeveloper("Error while trying to export quest info json file: " + e.Message);
                RaiseTaskFailed();
            }
        }
    }
}