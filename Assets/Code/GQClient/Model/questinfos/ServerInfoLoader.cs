using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Util;
using GQ.Client.Conf;
using Newtonsoft.Json;
using GQ.Client.Util;
using System;
using GQ.Client.UI;

namespace GQ.Client.Model {

	public class ServerQuestInfoLoader : Download {

		public ServerQuestInfoLoader() : 
		base(
			url: ConfigurationManager.UrlPublicQuestsJSON, 
			timeout: 120000
		)
		{ }

	}
}
