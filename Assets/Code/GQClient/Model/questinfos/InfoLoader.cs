using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.Model {

	/// <summary>
	/// Abstract Strategy class for Quest Info Loaders. There are at least three concrete loaders:
	/// 
	/// - ServerQuestInfoLoader
	/// - LocalQuestInfoLoader
	/// - TestQuestInfoLoader
	/// 
	/// </summary>
	public abstract class InfoLoader {

		/// <summary>
		/// Start the loading process and inform the QuestInfoManager about feedback via callbacks.
		/// 
		/// The parameters step and totalSteps signal which step of how many steps this loading 
		/// within a larger process currently is.
		/// </summary>
		public abstract void Start ();

	}

}
