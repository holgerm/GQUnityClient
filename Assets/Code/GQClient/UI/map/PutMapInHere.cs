using UnityEngine;
using UnitySlippyMap.Map;
using GQ.Client.UI.Foyer;

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
