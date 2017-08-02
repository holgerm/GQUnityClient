using UnityEngine;
using System.Collections;
using System.Globalization;
using System.Threading;
using GQ.Client.Err;

public class Base : MonoBehaviour
{

	public const string BASE = "Base";

	private static Base _instance = null;

	public static Base Instance {
		get {
			if (_instance == null) {
				GameObject baseGO = GameObject.Find (BASE);

				if (baseGO == null) {
					baseGO = new GameObject (BASE);
					Init ();
				}

				if (baseGO.GetComponent (typeof(Base)) == null)
					baseGO.AddComponent (typeof(Base));

				_instance = (Base)baseGO.GetComponent (typeof(Base));
			}
			return _instance;
		}
	}

	public static void Init ()
	{
		Thread.CurrentThread.CurrentCulture = new CultureInfo ("de-DE");
	}

	void Awake ()
	{
		DontDestroyOnLoad (Instance);
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
