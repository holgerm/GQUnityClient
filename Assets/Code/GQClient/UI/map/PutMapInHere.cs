using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySlippyMap.Map;

namespace GQ.Client.UI
{
	[RequireComponent (typeof(Map))]
	public class PutMapInHere : MonoBehaviour
	{

		// Use this for initialization
		void Start ()
		{
			MapBehaviour.Instance.transform.SetParent (transform);
			GameObject ttGo = GameObject.Find ("[Tile Template]");
		}
	
		// Update is called once per frame
		void Update ()
		{
		}
	}
}
