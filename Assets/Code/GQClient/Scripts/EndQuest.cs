using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;

namespace GQ.Client.Scripts
{

	public class EndQuest : MonoBehaviour
	{

		public void DoEndQuest ()
		{
			QuestManager.Instance.CurrentQuest.End ();
		}

	}
}
