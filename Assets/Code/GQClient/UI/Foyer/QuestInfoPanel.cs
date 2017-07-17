using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using UnityEngine.UI;

namespace GQ.Client.UI.Foyer {

	/// <summary>
	/// Shows all Quest Info objects, e.g. in a scrollable list within the foyer.
	/// </summary>
	public class QuestInfoPanel : MonoBehaviour {

		// Use this for initialization
		void Start () 
		{
			
		}

		public void SetUp(QuestInfo qi)
		{
			transform.Find("Name").gameObject.GetComponent<Text>().text = qi.Name;
		}
			
	}

}