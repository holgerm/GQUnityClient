using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

namespace GQ.Lang {

	public sealed class GQDictionary {

		private static GQDictionary instance;
		private static readonly object accessLock = new object();

		/// <summary>
		/// Access to the single GQ dictionary instance (thread safe).
		/// </summary>
		/// <value>The instance.</value>
		public static GQDictionary Instance {
			get {
				lock ( accessLock ) {
					if ( instance == null ) {
						instance = new GQDictionary();
					}
					return instance;
				}
			}
		}

		public string language = "de";
		public List<Translation> translations;
		public string oldlanguage = "de";

		public void setLanguage (string lang) {


			oldlanguage = language;
			language = lang;


			PlayerPrefs.SetString("gq_language", language);
			// save to PlayerPrefs

		}

		void Awake () {


			//read from PlayerPrefs

			if ( PlayerPrefs.HasKey("gq_language") ) {


				language = PlayerPrefs.GetString("gq_language");

			}
			else {

				if ( Configuration.instance.defaultlanguage == "system" ) {
					// System Language
					if ( Application.systemLanguage == SystemLanguage.German ) {

						language = "de";

					}
					else
					if ( Application.systemLanguage == SystemLanguage.English ) {
					
						language = "en";
					
					}
					else {

						language = "en";

					}
				}
				else {

					language = Configuration.instance.defaultlanguage;

				}

			}




			translations = new List<Translation>();
			translations.Add(new Translation("Beenden", "Exit"));
			translations.Add(new Translation("Impressum", "Imprint"));
			translations.Add(new Translation("Alle Daten löschen", "Delete all files"));
			translations.Add(new Translation("Liste", "List"));
			translations.Add(new Translation("Lokale Quests", "Local Quests"));
			translations.Add(new Translation("Alle Quests", "All Quests"));
			translations.Add(new Translation("Cloud Quests", "Cloud Quests"));
			translations.Add(new Translation("Durchsuchen...", "Search..."));
			translations.Add(new Translation("Version", "version"));
			translations.Add(new Translation("Nochmal versuchen", "Try again"));
			translations.Add(new Translation("sortieren nach:", "sort by:"));
			translations.Add(new Translation("Erstellungsdatum", "creation date"));
			translations.Add(new Translation("Name", "name"));
			translations.Add(new Translation("Nach Namen sortieren", "sort by name"));
			translations.Add(new Translation("Karte", "Map"));
			translations.Add(new Translation("Schließen", "Exit"));
			translations.Add(new Translation("Es ist ein Fehler aufgetreten.", "An error occured."));
			translations.Add(new Translation("Nochmal aufnehmen", "Record again"));
			translations.Add(new Translation("Abspielen", "Play"));
			translations.Add(new Translation("Aufname starten", "Start recording"));
			translations.Add(new Translation("Foto machen!", "Take photo!"));
			translations.Add(new Translation("Zurück", "Back"));
			translations.Add(new Translation("Antwort eingeben...", "Type answer..."));
			translations.Add(new Translation("Antwort abschicken", "Send answer"));
			translations.Add(new Translation("Weiter", "Continue"));
			translations.Add(new Translation("Sprache", "Language"));
			translations.Add(new Translation("Datenschutzvereinbarung", "Privacy Agreement"));
			translations.Add(new Translation("Allgemeine Geschäftsbedingungen", "Terms and Conditions"));
			translations.Add(new Translation("AGBs", "Terms and Conditions"));
			translations.Add(new Translation("Akzeptieren", "Accept"));




		}

		public string getTranslation (string s) {


			bool foundTranslation = false;

			if ( language != oldlanguage ) {
			
		

				foreach ( Translation t in translations ) {


					if ( oldlanguage == "de" ) {

						if ( t.german.Equals(s) && !foundTranslation ) {

							foundTranslation = true;

							if ( language == "en" ) {

								return t.english;

							}

						}
					}


					if ( oldlanguage == "en" ) {
						
						if ( t.english.Equals(s) && !foundTranslation ) {
							
							foundTranslation = true;
							
							if ( language == "de" ) {
								
								return t.german;
								
							}
							
						}
				


					}
				}
			}



			Debug.Log("no translation found: " + s);

			return s;



		}




	}

}
