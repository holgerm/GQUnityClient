using System;
using System.Collections;
using System.Collections.Generic;
using Code.GQClient.Err;
using Code.GQClient.Util.tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace GQClient.Model
{

    /// <summary>
    /// Imports quest infos from JSON files. Either form the servers listing of all quest infos that are available, 
    /// or form the local json file which keeps track of the latest state of local and remote quest infos.
    /// 
    /// In order to import the server info, you need to use a downloader task before and 
    /// simply call the constructor of this class with 'true'). 
    /// 
    /// To load the local json file use 'false' as paraneter of the constructor. 
    /// In this case no download task is needed and if exitent its result will be ignored.
    /// </summary>
    public abstract class ImportQuestInfos : Task
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="GQ.Client.Model.ImportQuestInfosFromJSON"/> class.
        /// </summary>
        public ImportQuestInfos() : base()
        {
            InputJson = "[]";
            qim = QuestInfoManager.Instance;
        }

        protected QuestInfoManager qim;

        private string _inputJson;
        protected string InputJson
        {
            get => _inputJson;
            set
            {
                _inputJson = value;
            }
        }

        protected override void ReadInput(object input = null)
        {
            if (input != null && input is string)
            {
                InputJson = input as string;
            }
            else
            {
                Log.SignalErrorToDeveloper("ImportFromInputString task should read from Input but got no input string.");
            }
        }

        protected override IEnumerator DoTheWork()
        {
            QuestInfo[] quests;

            try
            {
                quests = JsonConvert.DeserializeObject<QuestInfo[]>(InputJson,
                    new JsonSerializerSettings
                    {
                        Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                            {
                                Log.SignalErrorToDeveloper("ERROR while deserializing from JSON: " + args.ErrorContext.Error.Message +
                                    " path: " + args.ErrorContext.Path +
                                    " inner exc: " + args.ErrorContext.Error.InnerException.Message);
                                args.ErrorContext.Handled = true;
                            },
                        Converters = { new IsoDateTimeConverter() }
                    });
            }
            catch (Exception e)
            {
                Log.SignalErrorToDeveloper(
                    "Error in JSON while trying to update quest infos: {0}\nJSON:\n{1}",
                    e.Message,
                    InputJson
                );
                yield break;
            }

            if (quests == null || quests.Length == 0)
                yield break;

            updateQuestInfoManager(quests);
        }

        protected abstract void updateQuestInfoManager(IEnumerable<QuestInfo> newQuests);

        public override object Result
        {
            get
            {
                return null;
            }
            set { }
        }
    }
}
