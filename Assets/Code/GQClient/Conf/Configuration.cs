using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GQ.Client.Conf
{

	public class Configuration : MonoBehaviour
	{
		private static Configuration _instance;

		public bool showMessageForDatasendAction = true;
		public bool showPrivacyAgreement = true;
		public int privacyAgreementVersion = -1;
		public bool showAGBs = true;
		public int agbsVersion = -1;
		public bool offlinePlayable = true;
		public string defaultlanguage = "system";
		public bool languageChangableByUser = true;
		public List<Language> languagesAvailable;
		public List<QuestMetaCategory> metaCategoryUsage;
		public Sprite defaultmarker;
		// TODO make available in portal
		public float markerScale = 1.0f;
		// TODO make available in portal
		public float storedMapZoom = 18.0f;
		// TODO make available in portal
		public bool useDefaultPositionValuesAtStart = true;
		public double defaultLongitude = 51.0d;
		// TODO make available in portal
		public double defaultLatitude = 8.0d;

		public double[] storedSimulatedPosition {
			get;
			set;
		}

		public Vector3 _storedLocationMarkerAngles {
			get;
			set;
		}

		public double[] storedMapCenter {
			get;
			set;
		}

		public bool storedMapPositionModeIsCentering {
			get;
			set;
		}

		public static Configuration instance {
			get {
				if (_instance == null) {
					_instance = GameObject.FindObjectOfType<Configuration> ();
				
					//Tell unity not to destroy this object when loading a new scene!
					DontDestroyOnLoad (_instance.gameObject);
				}
			
				return _instance;
			}
		}

		void Awake ()
		{
			if (_instance == null) {
				//If I am the first instance, make me the Singleton
				_instance = this;
				DontDestroyOnLoad (this);
			} else {
				//If a Singleton already exists and you find
				//another reference in scene, destroy it!
				if (this != _instance) {
					Destroy (this.gameObject);
				}
			}

			storedMapCenter = null;

			storedMapPositionModeIsCentering = true;
		
		}

		public void Start ()
		{
//		if (!overrideProductSettingsInInspector)
//			initProductDefinitions ();
		}


		public bool metaCategoryIsSearchable (string category)
		{

			if (metaCategoryUsage != null && metaCategoryUsage.Count > 0) {

				foreach (QuestMetaCategory qmc in metaCategoryUsage) {

					if (qmc.name.ToUpper ().Equals (category.ToUpper ()) && qmc.considerInSearch) {

						return true;

					}

				}

			}

			return false;
		}



		public QuestMetaCategory getMetaCategory (string category)
		{

			if (metaCategoryUsage != null && metaCategoryUsage.Count > 0) {

				foreach (QuestMetaCategory qmc in metaCategoryUsage) {

					if (qmc.name.ToUpper ().Equals (category.ToUpper ()) && qmc.considerInSearch) {

						return qmc;

					}

				}

			}

			return null;
		}

	}






	[System.Serializable]
	public class Language
	{
	
		public string bezeichnung;
		public string anzeigename_de;
		public Sprite sprite;
		public bool available = true;

	}




	[System.Serializable]
	public class QuestMetaCategory
	{
		public string name;

		public bool considerInSearch;
		public bool filterButton;

		public List<string> possibleValues;

		public List<string> chosenValues;


		public void addPossibleValues (string values)
		{
			if (values.Contains (",")) {

				List<string> split = new List<string> ();
				split.AddRange (values.Split (','));


				foreach (string s1 in split) {

					if (split.Any (s => s.ToUpper ().Equals (s1.ToUpper ()))) {

						// is in already

					} else {

						if (!s1.Equals ("")) {

							possibleValues.Add (s1);
						}

					}


				}

			} else {

				if (!values.Equals ("")) {

					possibleValues.Add (values);

				}


			}

		}


		public bool isChosen (string s1)
		{
			if (chosenValues.Any (s => s.Equals (s1, StringComparison.OrdinalIgnoreCase))) {

				return true;

			}

			return false;
		}



		public void chooseValue (string s1)
		{


			if (chosenValues.Any (s => s.Equals (s1, StringComparison.OrdinalIgnoreCase))) {

				// is in already

			} else {


				chosenValues.Add (s1);


			}

		}


		public void unchooseValue (string s1)
		{

			foreach (string s in chosenValues.GetRange(0,chosenValues.Count)) {


				if (s.ToUpper ().Equals (s1.ToUpper ())) {


					chosenValues.Remove (s);

				}

			}
		}

	}

}