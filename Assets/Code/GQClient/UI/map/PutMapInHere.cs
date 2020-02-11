using Code.UnitySlippyMap.Map;
using UnityEngine;

namespace Code.GQClient.UI.map
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
