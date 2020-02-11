using UnityEngine;

namespace Code.GQClient.Util
{
	public class DontDestroyOnLoad : MonoBehaviour
	{

		// Use this for initialization
		void Awake ()
		{
			DontDestroyOnLoad (this);
		}
	}
}
