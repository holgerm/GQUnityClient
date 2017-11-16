using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySlippyMap.Map;
using GQ.Client.UI.Foyer;

namespace GQ.Client.UI
{
	[RequireComponent (typeof(QuestMap))]
	public class PutMapInHere : MonoBehaviour
	{

		// Use this for initialization
		void Start ()
		{
			MapBehaviour.Instance.transform.SetParent (transform);
//	TODO make home of Tile Template here:		GameObject ttGo = GameObject.Find ("[Tile Template]");
		}
	
		// Update is called once per frame
		void Update ()
		{
		}
	}
}
