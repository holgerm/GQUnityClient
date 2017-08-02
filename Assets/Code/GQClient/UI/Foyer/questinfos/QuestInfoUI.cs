using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using UnityEngine.UI;
using System;
using GQ.Util;

namespace GQ.Client.UI.Foyer {

	/// <summary>
	/// Represents one quest info object in a list within the foyer.
	/// </summary>
	public class QuestInfoUI : PrefabController {

		#region Content and Structure

		protected static readonly string PREFAB = "QuestInfo";

		public Text Name;
		protected const string NAME_PATH = "Name";
		protected const int MAX_NAME_LENGTH = 19;

		#endregion


		#region Runtime API

		public static GameObject Create(GameObject root) 
		{
			return PrefabController.Create (PREFAB, root);
		}

		public void SetContent(QuestInfo q) 
		{
			Name.text = q.Name;
		}

		#endregion


		#region Initialization in Editor

		public virtual void Reset()
		{
			Name = EnsurePrefabVariableIsSet<Text> (Name, "Name", NAME_PATH);
		}

		#endregion
	}

}