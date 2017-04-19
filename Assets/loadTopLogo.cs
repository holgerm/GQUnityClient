using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using GQ.Client.Conf;

public class loadTopLogo : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		GetComponent<Image> ().sprite = ConfigurationManager.TopLogo;
	}
	

}
