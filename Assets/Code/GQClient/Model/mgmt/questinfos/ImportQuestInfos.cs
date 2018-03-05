using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Util;
using GQ.Client.Conf;
using Newtonsoft.Json;
using System;
using GQ.Client.UI;
using System.IO;
using GQ.Client.Err;
using Newtonsoft.Json.Converters;

namespace GQ.Client.Model
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
		/// <param name="importFromServer">If set to <c>true</c> import from server otherwise use the local infos.json file.</param>
		public ImportQuestInfos () : base ()
		{ 
			InputJSON = "[]";
			qim = QuestInfoManager.Instance;
		}

		protected QuestInfoManager qim;
		protected string InputJSON { get; set; }

		public override bool Run ()
		{
			QuestInfo[] quests;

			try {
				quests = JsonConvert.DeserializeObject<QuestInfo[]> (InputJSON,
					new JsonSerializerSettings {
						Error = delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
							{
								Log.SignalErrorToDeveloper("ERROR while deserializing from JSON: " + args.ErrorContext.Error.Message +
									" path: " + args.ErrorContext.Path +
									" inner exc: " + args.ErrorContext.Error.InnerException.Message);
								args.ErrorContext.Handled = true;
							},
						Converters = { new IsoDateTimeConverter() }
					});
			} catch (Exception e) {
				Log.SignalErrorToDeveloper (
					"Error in JSON while trying to update quest infos: {0}\nJSON:\n{1}",
					e.Message,
					InputJSON
				);
				return false;
			}

			if (quests == null || quests.Length == 0)
				return true;

			updateQuestInfoManager (quests);
				
			return true;
		}

		protected abstract void updateQuestInfoManager (QuestInfo[] quests);

		public override object Result {
			get {
				return null;
			}
			protected set { }
		}
	}
}
