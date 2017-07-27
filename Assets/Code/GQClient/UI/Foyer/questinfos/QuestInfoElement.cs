using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using UnityEngine.UI;
using System;

namespace GQ.Client.UI.Foyer {

	/// <summary>
	/// Represents one quest info object in a list within the foyer.
	/// </summary>
	public class QuestInfoElement : PrefabController {

		#region Content and Structure

		public Text Name;
		protected const string NAME_PATH = "Name";

		#endregion


		#region Initialization in Editor

		public virtual void Reset()
		{
			Name = EnsurePrefabVariableIsSet<Text> (Name, "Name", NAME_PATH);
		}

		#endregion
	}

}