using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySlippyMap.Map;
using GQ.Client.UI.Foyer;
using QM.Util;

namespace GQ.Client.UI
{
	[RequireComponent (typeof(FoyerMapController))]
	public class PutMapInHere : MonoBehaviour
	{

		// Use this for initialization
		void Start ()
		{
			MapBehaviour.Instance.transform.SetParent (transform);
		}
	
	}
}
