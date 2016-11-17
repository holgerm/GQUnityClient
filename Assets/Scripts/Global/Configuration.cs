using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GQ.Client.Conf {

	public class Configuration : MonoBehaviour {
		private static Configuration _instance;

		public enum ProductIDs {
			Public,
			CarlBenz,
			ErzbistumKoeln,
			LWL,
			WikiCultureCity,
			Intern,
			Demos,
			Odysseum,
			SLSSpiele,
			Spielpunkte,
			EduQuest,
			Bienenlehrpfad
		}

		public ProductIDs product;
		public bool overrideProductSettingsInInspector = false;
		public string colorProfile = "default";
		public int downloadTimeOutSeconds = 300;
		public Sprite toplogo;
		public string nameForQuest = "Quest";
		public bool showMessageForDatasendAction = true;
		public bool showPrivacyAgreement = true;
		public string privacyAgreement = "";
		public int privacyAgreementVersion = -1;
		public bool showAGBs = true;
		public string agbs = "";
		public int agbsVersion = -1;
		public bool questvisualizationchangable = false;
		public bool offlinePlayable = true;
		public bool downloadAllCloudQuestOnStart = false;
		public bool localQuestsDeletable = true;
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
		// TODO make available in portal
		public bool checkForAppversion = true;
		public string appVersionURL = "";
		public bool hasMenuWithinQuests = true;

		/// <summary>
		/// This version numbers controls wether the app is compatible with the portal. 
		/// If it is lower than the numer provided by the appVersionURL the app shall or 
		/// even can not be used without updating it.
		/// </summary>
		public int appVersion = 0;

		/// <summary>
		/// This version number is the subversion numbering that we as developers give to the build. 
		/// The user is shown the apVersion number and this subversionNumber additionally to the .
		/// </summary>
		public string subversionNumber = "0.1";

	
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
				if ( _instance == null ) {
					_instance = GameObject.FindObjectOfType<Configuration>();
				
					//Tell unity not to destroy this object when loading a new scene!
					DontDestroyOnLoad(_instance.gameObject);
				}
			
				return _instance;
			}
		}

		void Awake () {
			if ( _instance == null ) {
				//If I am the first instance, make me the Singleton
				_instance = this;
				DontDestroyOnLoad(this);
			}
			else {
				//If a Singleton already exists and you find
				//another reference in scene, destroy it!
				if ( this != _instance ) {
					Destroy(this.gameObject);
				}
			}

			if ( !overrideProductSettingsInInspector ) {
				initProductDefinitions();
			}

			storedMapCenter = null;

			storedMapPositionModeIsCentering = true;
		
		}

		public void Start () {
//		if (!overrideProductSettingsInInspector)
//			initProductDefinitions ();
		}

		private void initProductDefinitions () {

//			ConfigurationManager.deserialize();

			switch ( product ) {
				case ProductIDs.CarlBenz:
					downloadTimeOutSeconds = 600;
					colorProfile = "carlbenz";
					break;
				case ProductIDs.WikiCultureCity:
					downloadTimeOutSeconds = 60;
					colorProfile = "wcc";
					agbs = "Anbieter dieser App ist die WikiCultureCity GbR, Sebastianstr. 39, D-53115 Bonn. Der Anbieter dieser App betreibt ein Webportal, auf dem registrierte Mitglieder (nachfolgend „Autoren“) mithilfe des Global Position Systems (GPS) interaktive Touren zu verschiedenen Themenbereichen erstellen können (nachfolgend „GeoQuests“). Hierzu erstellt der Autor anhand von GPS-Koordinaten eine Route die sich unter Zuhilfenahme von Bildern, Texten, Videoclips und Voice-Messages mit verschiedenen Themenbereichen auseinandersetzt. Die GeoQuests werden sowohl registrierten, als auch nicht registrierten Nutzern über eine Smartphone-Applikation (nachfolgend „App“) zur Verfügung gestellt.<br><br>1.\tGeltungsbereich der Nutzungsbedingungen<br>Diese Nutzungsbedingungen gelten für alle Inhalte und Dienste, die der Anbieter im Rahmen des Webportals und der App angeboten werden. Die Nutzungsbedingungen sind jederzeit abrufbar.   <br>Entgegenstehende oder diesen Nutzungsbedingungen abweichende Bedingungen des Nutzer haben keine Geltung.  <br><br>1.\tBereitstellungsbedingungen<br>1.1.\tDurch die unentgeltliche Bereitstellung sowie durch einen entsprechenden Abruf der GeoQuests seitens der nicht registrierten Nutzer wird kein Vertragsverhältnis zwischen dem Anbieter und dem jeweiligen Nutzer begründet. <br>1.2.\tDer Anbieter ist bemüht, einen ordnungsgemäßen Betrieb des Webportals sicherzustellen, steht jedoch nicht für die ununterbrochene Nutzbarkeit bzw. Erreichbarkeit des Webportals ein. Dies gilt insbesondere für technisch bedingte Verzögerungen im Rahmen von Wartungsarbeiten oder Weiterentwicklungen, Unterbrechungen oder Ausfälle der Angebote, des Internets oder des Zugangs zum Internet. <br>1.3.\tDer Anbieter behält sich vor, Teile der Angebote, einzelne Angebote oder alle Angebote als Ganzes ohne gesonderte Vorankündigung zu verändern oder die Veröffentlichung zeitweise oder endgültig einzustellen. Entschädigungsansprüche des Nutzers entstehen hieraus nicht. <br><br>2.\tHaftung und Haftungsbeschränkung <br>2.1.\tDer Anbieter ist nicht verpflichtet und auch nicht in der Lage, die Rechtmäßigkeit der von Nutzern oder den Autoren hochgeladenen oder publizierten Inhalte umfassend zu prüfen, zu überwachen und/oder nach Umständen zu forschen, die auf eine rechtswidrige Tätigkeit hinweisen. Das gleiche gilt, wenn und soweit von den Angeboten des Anbieters auf Webseiten Dritter verlinkt oder verwiesen wird. Der Anbieter macht sich die von Nutzern hochgeladenen oder publizierten Inhalte sowie die auf den Webseiten Dritter liegenden, durch Link verknüpften Inhalte nicht zu Eigen. Der Anbieter steht nicht dafür ein, dass diese Inhalte rechtmäßig, korrekt, aktuell und/oder vollständig sind. Für Schäden, die Aufgrund der Nutzung dieser Inhalte entstehen haftet der Anbieter nicht.<br>2.3.\tWenn der Anbieter Hinweise auf Gesetzesverstöße oder Rechtsverletzungen durch fremde oder verlinkte Inhalte erhält, wird der Anbieter soweit eine Pflicht besteht die Inhalte überprüfen und erforderlichenfalls sperren und löschen. <br>2.4.\tDer Anbieter haftet für Schadensersatz oder Ersatz vergeblicher Aufwendungen unbeschränkt bei Vorsatz oder grober Fahrlässigkeit, für die Verletzung von Leben, Leib oder Gesundheit, nach den Vorschriften des Produkthaftungsgesetzes.<br>2.5.\tBei leicht fahrlässiger Verletzung einer Pflicht, die wesentlich für die Erreichung des Vertragszwecks ist (Kardinalpflicht), ist die Haftung des Anbieters der Höhe nach begrenzt auf den Schaden, der nach der Art des fraglichen Geschäfts vorhersehbar und typisch ist.<br>2.6.\tDie vorstehende Haftungsbeschränkung der Ziff. 2.4. – 2.5. gilt auch für die persönliche Haftung der Mitarbeiter, Vertreter und Organe des Anbieters. <br>2.7.\tEine darüber hinausgehende Haftung besteht nicht. <br><br>3.\tOffenlegung von Informationen <br>Der Anbieter ist berechtigt, vorhandene Informationen von und über Nutzer ohne dessen ausdrückliches Einverständnis an Dritte herauszugeben, soweit er hierzu verpflichtet oder dies nach pflichtgemäßen Ermessen notwendig und rechtlich zulässig ist, um gesetzliche Bestimmungen oder richterliche oder behördliche Anordnungen zu erfüllen. <br><br>4.\tAnwendbares Recht, Gerichtsstand<br>4.1.\tEs gilt das Recht der Bundesrepublik Deutschland unter Ausschluss der Normen des Kollisionsrechts und des UN-Kaufrechts.<br><br>4.2.\tGerichtsstand für alle Streitigkeiten ist – soweit gesetzlich zulässig – der Sitz des Anbieters. <br>5.\tSalvatorische Klausel <br>Sollten einzelne Regelungen der Nutzungsbedingungen unwirksam sein oder werden, wird dadurch die Wirksamkeit der übrigen Regelungen nicht berührt. Sofern Teile der Nutzungsbedingungen der geltenden Rechtslage nicht, nicht mehr oder nicht vollständig entsprechen sollten, bleiben die übrigen Teile in ihrem Inhalt und ihrer Gültigkeit davon unberührt und der ungültige Teil gilt als durch einen solchen ersetzt, der dem Sinn und Zweck des unwirksamen Teils bzw. dem Parteiwillen in rechtswirksamer Weise wirtschaftlich am nächsten kommt. <br>6.\tÄnderungsvorbehalt<br>Der Anbieter hat das Recht, Bestimmungen dieser Nutzungsbedingungen zu ändern, wenn ein wichtiger Grund hierfür vorliegt. <br>";
					privacyAgreement = "Wir freuen uns über Ihr Interesse an unserer App. Als Anbieter dieser App ist uns der sichere Umgang mit Ihren Daten besonders wichtig. Die Erhebung, Verarbeitung und Nutzung Ihrer personenbezogenen Daten geschieht ausschließlich unter Beachtung der geltenden datenschutzrechtlichen Bestimmungen und des Telemediengesetzes (TMG). Ohne Ihre Zustimmung werden wir Ihre personenbezogenen Daten nicht auf anderem Wege nutzen, als er sich aus oder im Zusammenhang mit dieser Datenschutzerklärung ergibt. Wir möchten Ihnen nachfolgend erläutern, welche Daten wir wann und zu welchem Zweck erheben, verarbeiten und nutzen.<br><br>1. Erhebung, Verarbeitung und Nutzung personenbezogener Daten<br>Personenbezogene Daten sind Einzelangaben über persönliche und sachliche Verhältnisse einer bestimmten oder bestimmbaren Person also Daten die Rückschlüsse über eine Person zulassen. <br>Mit der Installation von Tap-Erlebnis erheben wir folgende Daten:<br>a. Registrierung und Nutzung <br>Die Nutzung dieser App ist ohne Registrierung möglich. <br>b. Standortdaten<br>Wir benötigen den Zugriff auf den Standort Ihres Gerätes. Ihre GPS-Daten werden erhoben, gespeichert und an den Anbieter übermittelt. Die GPS-Daten sind notwendig, damit der Nutzer über Inhalte in seiner näheren Umgebung informiert wird. Daten zu Ihrem Standort werden nur für diese Bearbeitung Ihrer Anfrage genutzt. Ihre Standortdaten werden nach Beendigung Ihrer Anfrage nicht gespeichert und nicht weitergegeben.<br>c. Aktives Kontaktieren<br>Der Nutzer gestattet, dass der Betreiber die App aktiv kontaktiert, sofern diese online genutzt wird, um über Aktualisierungen der Datenschutzerklärung zu informieren.<br><br>II. Datensicherheit<br>Wir sichern unsere Systeme durch technische und organisatorische Maßnahmen gegen Verlust, Zerstörung, Zugriff, Veränderung oder Verbreitung Ihrer Daten durch unbefugte Personen. Trotz regelmäßiger Kontrollen ist ein vollständiger Schutz gegen alle Gefahren jedoch nicht möglich.<br>GeoQuest verwendet an manchen Stellen zur Verschlüsselung den Industriestandard SSL (Secure Sockets Layer). Dadurch wird die Vertraulichkeit Ihrer persönlichen Angaben über das Internet gewährleistet.<br><br>III. Bekanntmachung von Veränderungen<br>Gesetzesänderungen oder Änderungen unserer unternehmensinternen Prozesse können eine Anpassung dieser Datenschutzerklärung erforderlich machen. Für den Fall einer solchen Änderung wird die App, sofern Sie online gehen, Sie informieren.<br>";
					break;
				case ProductIDs.ErzbistumKoeln:
					downloadTimeOutSeconds = 600;
					colorProfile = "ebk";
					agbs = "Nutzungsbedingungen \nDas Erzbistum Köln, Marzellenstr. 32, 50668 Köln vertreten durch den Generalvikar Dr. Dominik Meiering (nachfolgend „Erzbistum Köln“) betreibt eine Webportal, auf dem registrierte ehrenamtliche Mitglieder (nachfolgend „Autoren“) mithilfe des Global Position Systems (GPS) interaktive Touren zu verschiedenen Themenbereichen erstellen können (nachfolgend „GeoQuests“). Hierzu erstellt der  Autor anhand von GPS-Koordinaten eine Route die sich unter Zuhilfenahme von Bildern, Texten, Videoclips und Voice-Messages mit verschiedenen Themenbereichen auseinandersetzt. Die GeoQuests werden sowohl registrierten, als auch nicht registrierten Nutzern über eine Smartphone-Applikation (nachfolgend „App“) zur Verfügung gestellt.\n\n1.\tGeltungsbereich der Nutzungsbedingungen\nDiese Nutzungsbedingungen gelten für alle Inhalte und Dienste, die durch das Erzbistum Köln im Rahmen des Webportals unter www.tap-erlebnis.de und der App angeboten werden. Die Nutzungsbedingungen sind jederzeit abrufbar. Es besteht die Möglichkeit, dass die Nutzungsbedingungen ausgedruckt werden.  \nEntgegenstehende oder diesen Nutzungsbedingungen abweichende Bedingungen des Nutzer haben keine Geltung.  \n\n2.\tBereitstellungsbedingungen\n2.1.\tDurch die unentgeltliche Bereitstellung sowie durch einen entsprechenden Abruf der GeoQuests seitens der nicht registrierten Nutzer wird kein Vertragsverhältnis zwischen dem Erzbistum Köln und dem jeweiligen Nutzer begründet. \n2.2.\tDas Erzbistum Köln ist bemüht, einen ordnungsgemäßen Betrieb des Webportals sicherzustellen, steht jedoch nicht für die ununterbrochene Nutzbarkeit bzw. Erreichbarkeit des Webportals ein. Dies gilt insbesondere für technisch bedingte Verzögerungen im Rahmen von Wartungsarbeiten oder Weiterentwicklungen, Unterbrechungen oder Ausfälle der Angebote, des Internets oder des Zugangs zum Internet. \n2.3.\tDas Erzbistum Köln behält sich vor, Teile der Angebote, einzelne Angebote oder alle Angebote als Ganzes ohne gesonderte Vorankündigung zu verändern oder die Veröffentlichung zeitweise oder endgültig einzustellen. Entschädigungsansprüche des Nutzers entstehen hieraus nicht. \n\n3.\tRegistrierung \n3.1.\tDie Nutzung des Webportals ist erst nach erfolgreicher Registrierung des Autors möglich. Registrieren dürfen sich nur geschäftsfähige Personen oder Personen, die mit Zustimmung ihrer gesetzlichen Vertretungsberechtigten handeln.  \n3.2.\tDie Registrierung erfolgt über das Webportal. Der Autor kann sich durch die Eingabe eines Nicknamen und einer E-Mail-Adresse registrieren. Nach der Registrierung erhält der Kunde eine Bestätigung über die Erstellung des Accounts per E-Mail.\n3.3.\tDer Autor ist verpflichtet, im Rahmen des Registrierungsprozesses wahrheitsgemäße Angaben zu machen und seine Angaben soweit erforderlich zu aktualisieren. Insbesondere dürfen durch die Registrierung keine Rechte Dritter verletzt werden (z.B. Verwendung von personenbezogenen Daten Dritter).\n3.4.\tDer Autor hat ein dem Stand der Technik nach sicheres Passwort zu wählen. Das Passwort zeichnet sich dadurch aus, dass es mindestens aus 12 Zeichen (darunter Groß- und Kleinbuchstaben, Ziffern und Sonderzeichen) besteht. Der Autor ist verpflichtet dieses Passwort geheim zu halten. \n3.5.\tEin Anspruch auf Registrierung besteht nicht. \n\n4.\tRechte an den Inhalten \n4.1.\tDas Webportal enthält Inhalte, die sowohl von dem Erzbistum Köln, als auch von den Nutzern erstellt wurden. Das gesamte Material, das auf dem Webportal veröffentlicht ist, insbesondere Texte, Fotografien, Grafiken und Videoclips, unterliegt dem jeweils geltenden gesetzlichen Schutz, insbesondere dem Marken-, Urheber-, Leistungsschutz- und Wettbewerbsrecht. Die Vervielfältigung, öffentliche Wiedergabe oder die sonstige Nutzung oder Verwertung derart geschützter Inhalte ist ohne Zustimmung des jeweiligen Rechteinhabers in der Regel unzulässig. \n4.2.\tMit dem Hochladen von Texten, Bildern, Screenshots oder anderen urheberrechtlich geschützten Inhalten auf das Webportal gewährt der Nutzer und/oder Autor dem Erzbistum Köln unentgeltlich ein übertragbares, nicht ausschließliches, räumlich und zeitlich unbeschränktes Recht diese Inhalte zu nutzen. Hiervon umfasst ist auch das Recht zur Bearbeitung und Übersetzung der zur Verfügung gestellten Werke. \n4.3.\tMit Veröffentlichung der GeoQuests auf dem Webportal gewährt der Autor dem Erzbistum Köln unentgeltlich ein übertragbares, nicht ausschließliches, räumlich und zeitlich unbeschränktes Recht diese Inhalte zu nutzen. Hiervon ist auch das Recht zur Bearbeitung der GeoQuests umfasst. \n4.4.\tSofern der Autor Bilder, Texte, Lieder usw. hochlädt, hat er die Möglichkeit die urheberrechtlich geschützten Inhalte jederzeit zu löschen. Dies gilt auch für alle von ihm erstellten GeoQuests. Die mit Ziff. 1.3.2 und Ziff. 1.3.3. eingeräumten Nutzungsrechte erlöschen in diesem Fall.\n4.5.\tDas Erzbistum Köln behält sich das Recht vor, Inhalte auch ohne Angabe von Gründen nicht oder nur in einen begrenzten Zeitraum auf dem Webportal bereitzuhalten sowie GeoQuests zu ändern oder zu löschen. \n4.6.\tDer Autor ist angehalten nur solche Inhalte auf dem Webportal einzustellen, an den er die notwendigen Rechte hat. In der Regel besitzt der Autor die erforderlichen Rechte nur, wenn er die Inhalte selbst erstellt oder die Zustimmung vom Rechteinhaber eingeholt hat. Dies gilt insbesondere bei von ihm eingestelltem Bildmaterial. Falls auf dem Bildmaterial Personen abgebildet werden, ist darüber hinaus sicherzustellen, dass die Einwilligung der abgebildeten Person vorliegt. \n4.7.\tDer Autor stellt das Erzbistum Köln von Ansprüchen Dritter, insbesondere von Ansprüchen wegen schuldhaft verursachten Urheberrechts-, Wettbewerbs-, Markenrechts- und Persönlichkeitsrechtsverletzungen, die gegen das Erzbistum Köln im Zusammenhang mit vom Autor erstellten GeoQuests geltend gemacht werden, auf erstes Anfordern hin frei. Dem Autor bekannt werdende Beeinträchtigungen der vertragsgegenständlichen Rechte hat dieser dem Erzbistum Köln unverzüglich mitzuteilen. \n4.8.\tDer Autor ist berechtigt, selbst geeignete Maßnahmen zur Abwehr von Ansprüchen Dritter oder zur Verfolgung seiner Rechte vorzunehmen. Eigene Maßnahmen des Autors hat dieser im Vorwege mit dem Erzbistum Köln abzustimmen. Die Freistellung beinhaltet auch den Ersatz der Kosten, die dem Erzbistum Köln durch die Rechtsverfolgung/-verteidigung entstehen bzw. entstanden sind. Das Erzbistum Köln wird den Autor unverzüglich von vorzunehmenden Maßnahmen der Rechtsverfolgung/-verteidigung in Kenntnis setzen. \n\n5.\tVerbotene Inhalte \n5.1.\tInhalte, die gegen geltende Gesetze verstoßen oder in sonstiger Weise rechtswidrige Inhalt haben, sind verboten. Ebenso sind Inhalte verboten, die schädigend, bedrohend, missbräuchlich, belästigend, verleumderisch, diskriminierend, ehrverletzend, sexistisch, pornographisch, Gewalt verherrlichend oder verharmlosend, rassistisch, extremistisch, für terroristische oder extremistische politische Vereinigung werbend, zu einer Straftat auffordernd,  oder auf sonstige Weise jugendgefährdend sind. Die Strafgesetze und Jugendschutzbestimmungen sind zu beachten. \n5.2.\tUnzulässig sind auch Inhalte, die Informationen über illegale Aktivitäten verbreiten(z.B. das Thematisieren von illegalen Programmen, Cracks, illegalen Downloads, Emulatoren).\n5.3.\tInhalte, die Rechte Dritter verletzen, insbesondere Patente, Marken-, Urheber- oder Leistungsschutzrechte, Geschäftsgeheimnisse, Persönlichkeitsrechte oder Eigentumsrechte, dürfen nicht hochgeladen und zum Abruf bereit gehalten werden. \n5.4.\tDie Veröffentlichung von personenbezogenen Daten Dritter (z.B. Name, Adresse, Telefonnummer) ohne ausdrückliche schriftliche Zustimmung des Betroffenen ist nicht erlaubt. \n5.5.\tInhalte, die Waren oder Dienstleistungen anbieten bzw. sonstigen werblichen oder kommerziellen Zwecken dienen, sind nicht gestattet. Unter dieses Verbot fallen auch Inhalte wie ein „Schneeballsystem“.\n5.6.\tVerlinkungen, die direkt zu einem Download oder einer Datei führen, die in keinerlei Zusammenhang mit dem Webportal stehen, sind untersagt. \n5.7.\tMehrfach erstellte GeoQuests mit gleichem Inhalt, Beiträge mit sinnfreiem Inhalt oder Beiträge, die einer Diskussion nicht zuträglich sind, werden als Spam eingestuft und sind ebenfalls zu unterlassen.\n\n6.\tNetiquette \nDie Nutzer sind gehalten einen friedlichen Umgangston zu wahren. Das Webportal soll durch die erstellten GeoQuests interessante und gewinnbringende Debatten zu spirituellen, religiösen, katechetischen Inhalten fördern. Persönliche Angriffe gegen andere Nutzer, Beleidigungen und Diskriminierungen, sind daher ausdrücklich nicht gestattet und werden nicht geduldet. Auf zynische und ironische Äußerungen sollte verzichtet werden. \n\n7.\tHaftung und Haftungsbeschränkung \n7.1.\tDas Erzbistum Köln ist – soweit sich dies aus dem Impressum des jeweiligen Angebots ergibt – Diensteanbieter i.S.d. § 7 Abs. 1 TMG und für eigene Inhalte, die im Rahmen des Webportals abrufbar sind, nach den allgemeinen Gesetzen verantwortlich. Das Erzbistum Köln übernimmt keine Gewährleistung bezüglich der Ergebnisse, die durch die Nutzung der Angebote erzielt werden können, oder bezüglich der Richtigkeit und Zuverlässigkeit der im Rahmen der Angebote erhältlichen Informationen und Anwendungen. \n7.2.\tDas Erzbistum Köln ist nicht verpflichtet und auch nicht in der Lage, die Rechtmäßigkeit der von Nutzern oder den Autorn hochgeladenen oder publizierten Inhalte umfassend zu prüfen, zu überwachen und/oder nach Umständen zu forschen, die auf eine rechtswidrige Tätigkeit hinweisen. Das gleiche gilt, wenn und soweit von den Angeboten des Erzbistum Köln auf Webseiten Dritter verlinkt oder verwiesen wird. Das Erzbistum Köln macht sich die von Nutzern hochgeladenen oder publizierten Inhalte sowie die auf den Webseiten Dritter liegenden, durch Link verknüpften Inhalte nicht zu Eigen. Das Erzbistum Köln steht nicht dafür ein, dass diese Inhalte rechtmäßig, korrekt, aktuell und/oder vollständig sind. Für Schäden, die Aufgrund der Nutzung dieser Inhalte entstehen haftet das Erzbistum Köln nicht.\n7.3.\tWenn das Erzbistum Köln Hinweise auf Gesetzesverstöße oder Rechtsverletzungen durch fremde oder verlinkte Inhalte erhält, wird das Erzbistum Köln soweit eine Pflicht besteht die Inhalte überprüfen und erforderlichenfalls sperren und löschen. \n7.4.\tDas Erzbistum Köln haftet für Schadensersatz oder Ersatz vergeblicher Aufwendungen unbeschränkt bei Vorsatz oder grober Fahrlässigkeit, für die Verletzung von Leben, Leib oder Gesundheit, nach den Vorschriften des Produkthaftungsgesetzes sowie im Umfang einer vom Erzbistum Köln übernommenen Garantie.\n7.5.\tBei leicht fahrlässiger Verletzung einer Pflicht, die wesentlich für die Erreichung des Vertragszwecks ist (Kardinalpflicht), ist die Haftung vom Erzbistum Köln der Höhe nach begrenzt auf den Schaden, der nach der Art des fraglichen Geschäfts vorhersehbar und typisch ist.\n7.6.\tDie vorstehende Haftungsbeschränkung der Ziff. 1.6.4. – 1.6.5. gilt auch für die persönliche Haftung der Mitarbeiter, Vertreter und Organe des Erzbistums Köln. \n7.7.\tDas Erzbistum Köln haftet für den Verlust von Daten nur bis zu dem Betrag, der bei ordnungsgemäßer und regelmäßiger, gefahrentsprechender Sicherung der Daten zu deren Wiederherstellung angefallen wäre.\n7.8.\tEine darüber hinausgehende Haftung besteht nicht. \n\n8.\tDatenschutz\nWir nehmen den Schutz Ihrer Daten sehr ernst. Das Erzbistum Köln wird die personenbezogenen Daten, die wir von den Nutzern und Autorn erhalten nicht unbefugt an Dritte weitergeben oder zur Kenntnis bringen. Einzelheiten zur Datenerhebung und Datenverarbeitung sind den Datenschutzbestimmungen zu entnehmen.\n\n9.\tOffenlegung von Informationen \nDas Erzbistum Köln ist berechtigt, vorhandene Informationen von und über Nutzer ohne dessen ausdrückliches Einverständnis an Dritte herauszugeben, soweit es hierzu verpflichtet oder nach pflichtgemäßen Ermessen notwendig und rechtlich zulässig ist, um gesetzliche Bestimmungen oder richterliche oder behördliche Anordnungen zu erfüllen. \n\n10.\tAnwendbares Recht, Gerichtsstand\n9.1.\tEs gilt das Recht der Bundesrepublik Deutschland unter Ausschluss der Normen des Kollisionsrechts und des UN-Kaufrechts.\n9.2.\tGerichtsstand für alle Streitigkeiten ist – soweit gesetzlich zulässig – der Sitz des Erzbistums Köln. \n\n11.\tSalvatorische Klausel \nSollten einzelne Regelungen der Nutzungsbedingungen unwirksam sein oder werden, wird dadurch die Wirksamkeit der übrigen Regelungen nicht berührt. Sofern Teile der Nutzungsbedingungen der geltenden Rechtslage nicht, nicht mehr oder nicht vollständig entsprechen sollten, bleiben die übrigen Teile in ihrem Inhalt und ihrer Gültigkeit davon unberührt und der ungültige Teil gilt als durch einen solchen ersetzt, der dem Sinn und Zweck des unwirksamen Teils bzw. dem Parteiwillen in rechtswirksamer Weise wirtschaftlich am nächsten kommt. \n\n12.\tÄnderungsvorbehalt\nDas Erzbistum Köln hat das Recht, Bestimmungen dieser Nutzungsbedingungen zu ändern, wenn ein wichtiger Grund hierfür vorliegt. Das Erzbistum Köln wird den Nutzern die Änderung per E-Mail mitteilen. Widerspricht der Nutzer nicht innerhalb einer Frist von 6 Wochen nach Erhalt der E-Mail, gelten die geänderten Nutzungsbedingungen als vereinbart. Der Nutzer wird auf diese Rechtsfolge hingewiesen. Widerspricht der Nutzer den neuen Nutzungsbedingungen, kann das Erzbistum Köln den Vertrag nach Maßgabe ordentlich kündigen\n\nStand:  Dezember 2015 \n";
					privacyAgreement = "Datenschutzerklärung und Einwilligung \nWir, das Erzbistum Köln, Körperschaft des öffentlichen Rechts (KdöR), Marzellenstraße 32, 50668 Köln, Deutschland, freuen uns über Ihr Interesse an unserer App Tap-Erlebnis. Als Anbieter dieser App ist uns der sichere Umgang mit Ihren Daten besonders wichtig. Die Erhebung, Verarbeitung und Nutzung Ihrer personenbezogenen Daten geschieht ausschließlich unter Beachtung der geltenden datenschutzrechtlichen Bestimmungen und des Telemediengesetzes (TMG). Ohne Ihre Zustimmung werden wir Ihre personenbezogenen Daten nicht auf anderem Wege nutzen, als er sich aus oder im Zusammenhang mit dieser Datenschutzerklärung ergibt. Diese Datenschutzerklärung kann jederzeit unter der URL www.erzbistum-koeln.de/xxx abgerufen, abgespeichert und ausgedruckt werden. Wir möchten Ihnen nachfolgend erläutern, welche Daten wir wann und zu welchem Zweck erheben, verarbeiten und nutzen.\n\n1. Erhebung, Verarbeitung und Nutzung personenbezogener Daten\nPersonenbezogene Daten sind Einzelangaben über persönliche und sachliche Verhältnisse einer bestimmten oder bestimmbaren Person also Daten die Rückschlüsse über eine Person zulassen. \nMit der Installation von Tap-Erlebnis erheben wir folgende Daten:\na. Registrierung und Nutzung \nDie Nutzung von Tap-Erlebnis ist ohne Registrierung möglich. \nb. Standortdaten\nWir benötigen den Zugriff auf den Standort Ihres Gerätes. Ihre GPS-Daten werden erhoben, gespeichert und an den Anbieter übermittelt. Die GPS-Daten sind notwendig, damit der Nutzer GeoQuests absolvieren kann und über GeoQuests in seiner näheren Umgebung informiert wird. Daten zu Ihrem Standort werden nur für diese Bearbeitung Ihrer Anfrage genutzt. Die Übertragung Ihrer Standortdaten erfolgt über eine verschlüsselte Verbindung. Ihre Standortdaten werden nach Beendigung Ihrer Anfrage nicht gespeichert und nicht weitergegeben.\nc. Aktives Kontaktieren\nDer Nutzer gestattet, dass der Betreiber die App aktiv kontaktiert, sofern diese online genutzt wird, um über Aktualisierungen der Datenschutzerklärung zu informieren.\n\n2. Datensicherheit\nWir sichern unsere Systeme durch technische und organisatorische Maßnahmen gegen Verlust, Zerstörung, Zugriff, Veränderung oder Verbreitung Ihrer Daten durch unbefugte Personen. Trotz regelmäßiger Kontrollen ist ein vollständiger Schutz gegen alle Gefahren jedoch nicht möglich.\nGeoQuest verwendet an manchen Stellen zur Verschlüsselung den Industriestandard SSL (Secure Sockets Layer). Dadurch wird die Vertraulichkeit Ihrer persönlichen Angaben über das Internet gewährleistet.\n\n3. Bekanntmachung von Veränderungen\nGesetzesänderungen oder Änderungen unserer unternehmensinternen Prozesse können eine Anpassung dieser Datenschutzerklärung erforderlich machen. Für den Fall einer solchen Änderung wird die App, sofern Sie online gehen, Sie informieren.\n\n4. Auskunftsrecht\nWir speichern keine Daten von Ihnen. Sollten Sie dennoch Fragen haben, steht Ihnen unser Datenschutzbeauftragter unter:\n\nErzbistum Köln\nKörperschaft des öffentlichen Rechts (KdöR)\nMarzellenstraße 32, 50668 Köln\ndatenschutzbeauftragte@erzbistum-koeln.de\ngerne zur Verfügung.\n";
					break;
				case ProductIDs.LWL:
					downloadTimeOutSeconds = 60;
					colorProfile = "lwl";
					break;
				case ProductIDs.Public:
					downloadTimeOutSeconds = 60;
					colorProfile = "default";
					agbs = "Anbieter dieser App ist die QuestMill GmbH, Clostermannstr. 1, 51065 Köln. Der Anbieter dieser App betreibt ein Webportal, auf dem registrierte Mitglieder (nachfolgend „Autoren“) mithilfe des Global Position Systems (GPS) interaktive Touren zu verschiedenen Themenbereichen erstellen können (nachfolgend „GeoQuests“). Hierzu erstellt der Autor anhand von GPS-Koordinaten eine Route die sich unter Zuhilfenahme von Bildern, Texten, Videoclips und Voice-Messages mit verschiedenen Themenbereichen auseinandersetzt. Die GeoQuests werden sowohl registrierten, als auch nicht registrierten Nutzern über eine Smartphone-Applikation (nachfolgend „App“) zur Verfügung gestellt.<br><br>1.\tGeltungsbereich der Nutzungsbedingungen<br>Diese Nutzungsbedingungen gelten für alle Inhalte und Dienste, die der Anbieter im Rahmen des Webportals und der App angeboten werden. Die Nutzungsbedingungen sind jederzeit abrufbar.   <br>Entgegenstehende oder diesen Nutzungsbedingungen abweichende Bedingungen des Nutzer haben keine Geltung.  <br><br>1.\tBereitstellungsbedingungen<br>1.1.\tDurch die unentgeltliche Bereitstellung sowie durch einen entsprechenden Abruf der GeoQuests seitens der nicht registrierten Nutzer wird kein Vertragsverhältnis zwischen dem Anbieter und dem jeweiligen Nutzer begründet. <br>1.2.\tDer Anbieter ist bemüht, einen ordnungsgemäßen Betrieb des Webportals sicherzustellen, steht jedoch nicht für die ununterbrochene Nutzbarkeit bzw. Erreichbarkeit des Webportals ein. Dies gilt insbesondere für technisch bedingte Verzögerungen im Rahmen von Wartungsarbeiten oder Weiterentwicklungen, Unterbrechungen oder Ausfälle der Angebote, des Internets oder des Zugangs zum Internet. <br>1.3.\tDer Anbieter behält sich vor, Teile der Angebote, einzelne Angebote oder alle Angebote als Ganzes ohne gesonderte Vorankündigung zu verändern oder die Veröffentlichung zeitweise oder endgültig einzustellen. Entschädigungsansprüche des Nutzers entstehen hieraus nicht. <br><br>2.\tHaftung und Haftungsbeschränkung <br>2.1.\tDer Anbieter ist nicht verpflichtet und auch nicht in der Lage, die Rechtmäßigkeit der von Nutzern oder den Autoren hochgeladenen oder publizierten Inhalte umfassend zu prüfen, zu überwachen und/oder nach Umständen zu forschen, die auf eine rechtswidrige Tätigkeit hinweisen. Das gleiche gilt, wenn und soweit von den Angeboten des Anbieters auf Webseiten Dritter verlinkt oder verwiesen wird. Der Anbieter macht sich die von Nutzern hochgeladenen oder publizierten Inhalte sowie die auf den Webseiten Dritter liegenden, durch Link verknüpften Inhalte nicht zu Eigen. Der Anbieter steht nicht dafür ein, dass diese Inhalte rechtmäßig, korrekt, aktuell und/oder vollständig sind. Für Schäden, die Aufgrund der Nutzung dieser Inhalte entstehen haftet der Anbieter nicht.<br>2.3.\tWenn der Anbieter Hinweise auf Gesetzesverstöße oder Rechtsverletzungen durch fremde oder verlinkte Inhalte erhält, wird der Anbieter soweit eine Pflicht besteht die Inhalte überprüfen und erforderlichenfalls sperren und löschen. <br>2.4.\tDer Anbieter haftet für Schadensersatz oder Ersatz vergeblicher Aufwendungen unbeschränkt bei Vorsatz oder grober Fahrlässigkeit, für die Verletzung von Leben, Leib oder Gesundheit, nach den Vorschriften des Produkthaftungsgesetzes.<br>2.5.\tBei leicht fahrlässiger Verletzung einer Pflicht, die wesentlich für die Erreichung des Vertragszwecks ist (Kardinalpflicht), ist die Haftung des Anbieters der Höhe nach begrenzt auf den Schaden, der nach der Art des fraglichen Geschäfts vorhersehbar und typisch ist.<br>2.6.\tDie vorstehende Haftungsbeschränkung der Ziff. 2.4. – 2.5. gilt auch für die persönliche Haftung der Mitarbeiter, Vertreter und Organe des Anbieters. <br>2.7.\tEine darüber hinausgehende Haftung besteht nicht. <br><br>3.\tOffenlegung von Informationen <br>Der Anbieter ist berechtigt, vorhandene Informationen von und über Nutzer ohne dessen ausdrückliches Einverständnis an Dritte herauszugeben, soweit er hierzu verpflichtet oder dies nach pflichtgemäßen Ermessen notwendig und rechtlich zulässig ist, um gesetzliche Bestimmungen oder richterliche oder behördliche Anordnungen zu erfüllen. <br><br>4.\tAnwendbares Recht, Gerichtsstand<br>4.1.\tEs gilt das Recht der Bundesrepublik Deutschland unter Ausschluss der Normen des Kollisionsrechts und des UN-Kaufrechts.<br><br>4.2.\tGerichtsstand für alle Streitigkeiten ist – soweit gesetzlich zulässig – der Sitz des Anbieters. <br>5.\tSalvatorische Klausel <br>Sollten einzelne Regelungen der Nutzungsbedingungen unwirksam sein oder werden, wird dadurch die Wirksamkeit der übrigen Regelungen nicht berührt. Sofern Teile der Nutzungsbedingungen der geltenden Rechtslage nicht, nicht mehr oder nicht vollständig entsprechen sollten, bleiben die übrigen Teile in ihrem Inhalt und ihrer Gültigkeit davon unberührt und der ungültige Teil gilt als durch einen solchen ersetzt, der dem Sinn und Zweck des unwirksamen Teils bzw. dem Parteiwillen in rechtswirksamer Weise wirtschaftlich am nächsten kommt. <br>6.\tÄnderungsvorbehalt<br>Der Anbieter hat das Recht, Bestimmungen dieser Nutzungsbedingungen zu ändern, wenn ein wichtiger Grund hierfür vorliegt. <br>";
					privacyAgreement = "Wir freuen uns über Ihr Interesse an unserer App. Als Anbieter dieser App ist uns der sichere Umgang mit Ihren Daten besonders wichtig. Die Erhebung, Verarbeitung und Nutzung Ihrer personenbezogenen Daten geschieht ausschließlich unter Beachtung der geltenden datenschutzrechtlichen Bestimmungen und des Telemediengesetzes (TMG). Ohne Ihre Zustimmung werden wir Ihre personenbezogenen Daten nicht auf anderem Wege nutzen, als er sich aus oder im Zusammenhang mit dieser Datenschutzerklärung ergibt. Wir möchten Ihnen nachfolgend erläutern, welche Daten wir wann und zu welchem Zweck erheben, verarbeiten und nutzen.<br><br>1. Erhebung, Verarbeitung und Nutzung personenbezogener Daten<br>Personenbezogene Daten sind Einzelangaben über persönliche und sachliche Verhältnisse einer bestimmten oder bestimmbaren Person also Daten die Rückschlüsse über eine Person zulassen. <br>Mit der Installation von Tap-Erlebnis erheben wir folgende Daten:<br>a. Registrierung und Nutzung <br>Die Nutzung dieser App ist ohne Registrierung möglich. <br>b. Standortdaten<br>Wir benötigen den Zugriff auf den Standort Ihres Gerätes. Ihre GPS-Daten werden erhoben, gespeichert und an den Anbieter übermittelt. Die GPS-Daten sind notwendig, damit der Nutzer über Inhalte in seiner näheren Umgebung informiert wird. Daten zu Ihrem Standort werden nur für diese Bearbeitung Ihrer Anfrage genutzt. Ihre Standortdaten werden nach Beendigung Ihrer Anfrage nicht gespeichert und nicht weitergegeben.<br>c. Aktives Kontaktieren<br>Der Nutzer gestattet, dass der Betreiber die App aktiv kontaktiert, sofern diese online genutzt wird, um über Aktualisierungen der Datenschutzerklärung zu informieren.<br><br>II. Datensicherheit<br>Wir sichern unsere Systeme durch technische und organisatorische Maßnahmen gegen Verlust, Zerstörung, Zugriff, Veränderung oder Verbreitung Ihrer Daten durch unbefugte Personen. Trotz regelmäßiger Kontrollen ist ein vollständiger Schutz gegen alle Gefahren jedoch nicht möglich.<br>GeoQuest verwendet an manchen Stellen zur Verschlüsselung den Industriestandard SSL (Secure Sockets Layer). Dadurch wird die Vertraulichkeit Ihrer persönlichen Angaben über das Internet gewährleistet.<br><br>III. Bekanntmachung von Veränderungen<br>Gesetzesänderungen oder Änderungen unserer unternehmensinternen Prozesse können eine Anpassung dieser Datenschutzerklärung erforderlich machen. Für den Fall einer solchen Änderung wird die App, sofern Sie online gehen, Sie informieren.<br>";
					break;
				case ProductIDs.Demos:
					downloadTimeOutSeconds = 60;
					colorProfile = "default";
					agbs = "Anbieter dieser App ist die QuestMill GmbH, Clostermannstr. 1, 51065 Köln. Der Anbieter dieser App betreibt ein Webportal, auf dem registrierte Mitglieder (nachfolgend „Autoren“) mithilfe des Global Position Systems (GPS) interaktive Touren zu verschiedenen Themenbereichen erstellen können (nachfolgend „GeoQuests“). Hierzu erstellt der Autor anhand von GPS-Koordinaten eine Route die sich unter Zuhilfenahme von Bildern, Texten, Videoclips und Voice-Messages mit verschiedenen Themenbereichen auseinandersetzt. Die GeoQuests werden sowohl registrierten, als auch nicht registrierten Nutzern über eine Smartphone-Applikation (nachfolgend „App“) zur Verfügung gestellt.<br><br>1.\tGeltungsbereich der Nutzungsbedingungen<br>Diese Nutzungsbedingungen gelten für alle Inhalte und Dienste, die der Anbieter im Rahmen des Webportals und der App angeboten werden. Die Nutzungsbedingungen sind jederzeit abrufbar.   <br>Entgegenstehende oder diesen Nutzungsbedingungen abweichende Bedingungen des Nutzer haben keine Geltung.  <br><br>1.\tBereitstellungsbedingungen<br>1.1.\tDurch die unentgeltliche Bereitstellung sowie durch einen entsprechenden Abruf der GeoQuests seitens der nicht registrierten Nutzer wird kein Vertragsverhältnis zwischen dem Anbieter und dem jeweiligen Nutzer begründet. <br>1.2.\tDer Anbieter ist bemüht, einen ordnungsgemäßen Betrieb des Webportals sicherzustellen, steht jedoch nicht für die ununterbrochene Nutzbarkeit bzw. Erreichbarkeit des Webportals ein. Dies gilt insbesondere für technisch bedingte Verzögerungen im Rahmen von Wartungsarbeiten oder Weiterentwicklungen, Unterbrechungen oder Ausfälle der Angebote, des Internets oder des Zugangs zum Internet. <br>1.3.\tDer Anbieter behält sich vor, Teile der Angebote, einzelne Angebote oder alle Angebote als Ganzes ohne gesonderte Vorankündigung zu verändern oder die Veröffentlichung zeitweise oder endgültig einzustellen. Entschädigungsansprüche des Nutzers entstehen hieraus nicht. <br><br>2.\tHaftung und Haftungsbeschränkung <br>2.1.\tDer Anbieter ist nicht verpflichtet und auch nicht in der Lage, die Rechtmäßigkeit der von Nutzern oder den Autoren hochgeladenen oder publizierten Inhalte umfassend zu prüfen, zu überwachen und/oder nach Umständen zu forschen, die auf eine rechtswidrige Tätigkeit hinweisen. Das gleiche gilt, wenn und soweit von den Angeboten des Anbieters auf Webseiten Dritter verlinkt oder verwiesen wird. Der Anbieter macht sich die von Nutzern hochgeladenen oder publizierten Inhalte sowie die auf den Webseiten Dritter liegenden, durch Link verknüpften Inhalte nicht zu Eigen. Der Anbieter steht nicht dafür ein, dass diese Inhalte rechtmäßig, korrekt, aktuell und/oder vollständig sind. Für Schäden, die Aufgrund der Nutzung dieser Inhalte entstehen haftet der Anbieter nicht.<br>2.3.\tWenn der Anbieter Hinweise auf Gesetzesverstöße oder Rechtsverletzungen durch fremde oder verlinkte Inhalte erhält, wird der Anbieter soweit eine Pflicht besteht die Inhalte überprüfen und erforderlichenfalls sperren und löschen. <br>2.4.\tDer Anbieter haftet für Schadensersatz oder Ersatz vergeblicher Aufwendungen unbeschränkt bei Vorsatz oder grober Fahrlässigkeit, für die Verletzung von Leben, Leib oder Gesundheit, nach den Vorschriften des Produkthaftungsgesetzes.<br>2.5.\tBei leicht fahrlässiger Verletzung einer Pflicht, die wesentlich für die Erreichung des Vertragszwecks ist (Kardinalpflicht), ist die Haftung des Anbieters der Höhe nach begrenzt auf den Schaden, der nach der Art des fraglichen Geschäfts vorhersehbar und typisch ist.<br>2.6.\tDie vorstehende Haftungsbeschränkung der Ziff. 2.4. – 2.5. gilt auch für die persönliche Haftung der Mitarbeiter, Vertreter und Organe des Anbieters. <br>2.7.\tEine darüber hinausgehende Haftung besteht nicht. <br><br>3.\tOffenlegung von Informationen <br>Der Anbieter ist berechtigt, vorhandene Informationen von und über Nutzer ohne dessen ausdrückliches Einverständnis an Dritte herauszugeben, soweit er hierzu verpflichtet oder dies nach pflichtgemäßen Ermessen notwendig und rechtlich zulässig ist, um gesetzliche Bestimmungen oder richterliche oder behördliche Anordnungen zu erfüllen. <br><br>4.\tAnwendbares Recht, Gerichtsstand<br>4.1.\tEs gilt das Recht der Bundesrepublik Deutschland unter Ausschluss der Normen des Kollisionsrechts und des UN-Kaufrechts.<br><br>4.2.\tGerichtsstand für alle Streitigkeiten ist – soweit gesetzlich zulässig – der Sitz des Anbieters. <br>5.\tSalvatorische Klausel <br>Sollten einzelne Regelungen der Nutzungsbedingungen unwirksam sein oder werden, wird dadurch die Wirksamkeit der übrigen Regelungen nicht berührt. Sofern Teile der Nutzungsbedingungen der geltenden Rechtslage nicht, nicht mehr oder nicht vollständig entsprechen sollten, bleiben die übrigen Teile in ihrem Inhalt und ihrer Gültigkeit davon unberührt und der ungültige Teil gilt als durch einen solchen ersetzt, der dem Sinn und Zweck des unwirksamen Teils bzw. dem Parteiwillen in rechtswirksamer Weise wirtschaftlich am nächsten kommt. <br>6.\tÄnderungsvorbehalt<br>Der Anbieter hat das Recht, Bestimmungen dieser Nutzungsbedingungen zu ändern, wenn ein wichtiger Grund hierfür vorliegt. <br>";
					privacyAgreement = "Wir freuen uns über Ihr Interesse an unserer App. Als Anbieter dieser App ist uns der sichere Umgang mit Ihren Daten besonders wichtig. Die Erhebung, Verarbeitung und Nutzung Ihrer personenbezogenen Daten geschieht ausschließlich unter Beachtung der geltenden datenschutzrechtlichen Bestimmungen und des Telemediengesetzes (TMG). Ohne Ihre Zustimmung werden wir Ihre personenbezogenen Daten nicht auf anderem Wege nutzen, als er sich aus oder im Zusammenhang mit dieser Datenschutzerklärung ergibt. Wir möchten Ihnen nachfolgend erläutern, welche Daten wir wann und zu welchem Zweck erheben, verarbeiten und nutzen.<br><br>1. Erhebung, Verarbeitung und Nutzung personenbezogener Daten<br>Personenbezogene Daten sind Einzelangaben über persönliche und sachliche Verhältnisse einer bestimmten oder bestimmbaren Person also Daten die Rückschlüsse über eine Person zulassen. <br>Mit der Installation von Tap-Erlebnis erheben wir folgende Daten:<br>a. Registrierung und Nutzung <br>Die Nutzung dieser App ist ohne Registrierung möglich. <br>b. Standortdaten<br>Wir benötigen den Zugriff auf den Standort Ihres Gerätes. Ihre GPS-Daten werden erhoben, gespeichert und an den Anbieter übermittelt. Die GPS-Daten sind notwendig, damit der Nutzer über Inhalte in seiner näheren Umgebung informiert wird. Daten zu Ihrem Standort werden nur für diese Bearbeitung Ihrer Anfrage genutzt. Ihre Standortdaten werden nach Beendigung Ihrer Anfrage nicht gespeichert und nicht weitergegeben.<br>c. Aktives Kontaktieren<br>Der Nutzer gestattet, dass der Betreiber die App aktiv kontaktiert, sofern diese online genutzt wird, um über Aktualisierungen der Datenschutzerklärung zu informieren.<br><br>II. Datensicherheit<br>Wir sichern unsere Systeme durch technische und organisatorische Maßnahmen gegen Verlust, Zerstörung, Zugriff, Veränderung oder Verbreitung Ihrer Daten durch unbefugte Personen. Trotz regelmäßiger Kontrollen ist ein vollständiger Schutz gegen alle Gefahren jedoch nicht möglich.<br>GeoQuest verwendet an manchen Stellen zur Verschlüsselung den Industriestandard SSL (Secure Sockets Layer). Dadurch wird die Vertraulichkeit Ihrer persönlichen Angaben über das Internet gewährleistet.<br><br>III. Bekanntmachung von Veränderungen<br>Gesetzesänderungen oder Änderungen unserer unternehmensinternen Prozesse können eine Anpassung dieser Datenschutzerklärung erforderlich machen. Für den Fall einer solchen Änderung wird die App, sofern Sie online gehen, Sie informieren.<br>";
					break;
				case ProductIDs.Odysseum:
					downloadTimeOutSeconds = 60;
					colorProfile = "odysseum";
					break;
				case ProductIDs.SLSSpiele:
					downloadTimeOutSeconds = 60;
					colorProfile = "default";
					break;
				case ProductIDs.EduQuest:
					downloadTimeOutSeconds = 600;
					colorProfile = "eduquest";
					agbs = "Anbieter dieser App ist die QuestMill GmbH, Clostermannstr. 1, 51065 Köln. Der Anbieter dieser App betreibt ein Webportal, auf dem registrierte Mitglieder (nachfolgend „Autoren“) mithilfe des Global Position Systems (GPS) interaktive Touren zu verschiedenen Themenbereichen erstellen können (nachfolgend „GeoQuests“). Hierzu erstellt der Autor anhand von GPS-Koordinaten eine Route die sich unter Zuhilfenahme von Bildern, Texten, Videoclips und Voice-Messages mit verschiedenen Themenbereichen auseinandersetzt. Die GeoQuests werden sowohl registrierten, als auch nicht registrierten Nutzern über eine Smartphone-Applikation (nachfolgend „App“) zur Verfügung gestellt.<br><br>1.\tGeltungsbereich der Nutzungsbedingungen<br>Diese Nutzungsbedingungen gelten für alle Inhalte und Dienste, die der Anbieter im Rahmen des Webportals und der App angeboten werden. Die Nutzungsbedingungen sind jederzeit abrufbar.   <br>Entgegenstehende oder diesen Nutzungsbedingungen abweichende Bedingungen des Nutzer haben keine Geltung.  <br><br>1.\tBereitstellungsbedingungen<br>1.1.\tDurch die unentgeltliche Bereitstellung sowie durch einen entsprechenden Abruf der GeoQuests seitens der nicht registrierten Nutzer wird kein Vertragsverhältnis zwischen dem Anbieter und dem jeweiligen Nutzer begründet. <br>1.2.\tDer Anbieter ist bemüht, einen ordnungsgemäßen Betrieb des Webportals sicherzustellen, steht jedoch nicht für die ununterbrochene Nutzbarkeit bzw. Erreichbarkeit des Webportals ein. Dies gilt insbesondere für technisch bedingte Verzögerungen im Rahmen von Wartungsarbeiten oder Weiterentwicklungen, Unterbrechungen oder Ausfälle der Angebote, des Internets oder des Zugangs zum Internet. <br>1.3.\tDer Anbieter behält sich vor, Teile der Angebote, einzelne Angebote oder alle Angebote als Ganzes ohne gesonderte Vorankündigung zu verändern oder die Veröffentlichung zeitweise oder endgültig einzustellen. Entschädigungsansprüche des Nutzers entstehen hieraus nicht. <br><br>2.\tHaftung und Haftungsbeschränkung <br>2.1.\tDer Anbieter ist nicht verpflichtet und auch nicht in der Lage, die Rechtmäßigkeit der von Nutzern oder den Autoren hochgeladenen oder publizierten Inhalte umfassend zu prüfen, zu überwachen und/oder nach Umständen zu forschen, die auf eine rechtswidrige Tätigkeit hinweisen. Das gleiche gilt, wenn und soweit von den Angeboten des Anbieters auf Webseiten Dritter verlinkt oder verwiesen wird. Der Anbieter macht sich die von Nutzern hochgeladenen oder publizierten Inhalte sowie die auf den Webseiten Dritter liegenden, durch Link verknüpften Inhalte nicht zu Eigen. Der Anbieter steht nicht dafür ein, dass diese Inhalte rechtmäßig, korrekt, aktuell und/oder vollständig sind. Für Schäden, die Aufgrund der Nutzung dieser Inhalte entstehen haftet der Anbieter nicht.<br>2.3.\tWenn der Anbieter Hinweise auf Gesetzesverstöße oder Rechtsverletzungen durch fremde oder verlinkte Inhalte erhält, wird der Anbieter soweit eine Pflicht besteht die Inhalte überprüfen und erforderlichenfalls sperren und löschen. <br>2.4.\tDer Anbieter haftet für Schadensersatz oder Ersatz vergeblicher Aufwendungen unbeschränkt bei Vorsatz oder grober Fahrlässigkeit, für die Verletzung von Leben, Leib oder Gesundheit, nach den Vorschriften des Produkthaftungsgesetzes.<br>2.5.\tBei leicht fahrlässiger Verletzung einer Pflicht, die wesentlich für die Erreichung des Vertragszwecks ist (Kardinalpflicht), ist die Haftung des Anbieters der Höhe nach begrenzt auf den Schaden, der nach der Art des fraglichen Geschäfts vorhersehbar und typisch ist.<br>2.6.\tDie vorstehende Haftungsbeschränkung der Ziff. 2.4. – 2.5. gilt auch für die persönliche Haftung der Mitarbeiter, Vertreter und Organe des Anbieters. <br>2.7.\tEine darüber hinausgehende Haftung besteht nicht. <br><br>3.\tOffenlegung von Informationen <br>Der Anbieter ist berechtigt, vorhandene Informationen von und über Nutzer ohne dessen ausdrückliches Einverständnis an Dritte herauszugeben, soweit er hierzu verpflichtet oder dies nach pflichtgemäßen Ermessen notwendig und rechtlich zulässig ist, um gesetzliche Bestimmungen oder richterliche oder behördliche Anordnungen zu erfüllen. <br><br>4.\tAnwendbares Recht, Gerichtsstand<br>4.1.\tEs gilt das Recht der Bundesrepublik Deutschland unter Ausschluss der Normen des Kollisionsrechts und des UN-Kaufrechts.<br><br>4.2.\tGerichtsstand für alle Streitigkeiten ist – soweit gesetzlich zulässig – der Sitz des Anbieters. <br>5.\tSalvatorische Klausel <br>Sollten einzelne Regelungen der Nutzungsbedingungen unwirksam sein oder werden, wird dadurch die Wirksamkeit der übrigen Regelungen nicht berührt. Sofern Teile der Nutzungsbedingungen der geltenden Rechtslage nicht, nicht mehr oder nicht vollständig entsprechen sollten, bleiben die übrigen Teile in ihrem Inhalt und ihrer Gültigkeit davon unberührt und der ungültige Teil gilt als durch einen solchen ersetzt, der dem Sinn und Zweck des unwirksamen Teils bzw. dem Parteiwillen in rechtswirksamer Weise wirtschaftlich am nächsten kommt. <br>6.\tÄnderungsvorbehalt<br>Der Anbieter hat das Recht, Bestimmungen dieser Nutzungsbedingungen zu ändern, wenn ein wichtiger Grund hierfür vorliegt. <br>";
					privacyAgreement = "Wir freuen uns über Ihr Interesse an unserer App. Als Anbieter dieser App ist uns der sichere Umgang mit Ihren Daten besonders wichtig. Die Erhebung, Verarbeitung und Nutzung Ihrer personenbezogenen Daten geschieht ausschließlich unter Beachtung der geltenden datenschutzrechtlichen Bestimmungen und des Telemediengesetzes (TMG). Ohne Ihre Zustimmung werden wir Ihre personenbezogenen Daten nicht auf anderem Wege nutzen, als er sich aus oder im Zusammenhang mit dieser Datenschutzerklärung ergibt. Wir möchten Ihnen nachfolgend erläutern, welche Daten wir wann und zu welchem Zweck erheben, verarbeiten und nutzen.<br><br>1. Erhebung, Verarbeitung und Nutzung personenbezogener Daten<br>Personenbezogene Daten sind Einzelangaben über persönliche und sachliche Verhältnisse einer bestimmten oder bestimmbaren Person also Daten die Rückschlüsse über eine Person zulassen. <br>Mit der Installation von Tap-Erlebnis erheben wir folgende Daten:<br>a. Registrierung und Nutzung <br>Die Nutzung dieser App ist ohne Registrierung möglich. <br>b. Standortdaten<br>Wir benötigen den Zugriff auf den Standort Ihres Gerätes. Ihre GPS-Daten werden erhoben, gespeichert und an den Anbieter übermittelt. Die GPS-Daten sind notwendig, damit der Nutzer über Inhalte in seiner näheren Umgebung informiert wird. Daten zu Ihrem Standort werden nur für diese Bearbeitung Ihrer Anfrage genutzt. Ihre Standortdaten werden nach Beendigung Ihrer Anfrage nicht gespeichert und nicht weitergegeben.<br>c. Aktives Kontaktieren<br>Der Nutzer gestattet, dass der Betreiber die App aktiv kontaktiert, sofern diese online genutzt wird, um über Aktualisierungen der Datenschutzerklärung zu informieren.<br><br>II. Datensicherheit<br>Wir sichern unsere Systeme durch technische und organisatorische Maßnahmen gegen Verlust, Zerstörung, Zugriff, Veränderung oder Verbreitung Ihrer Daten durch unbefugte Personen. Trotz regelmäßiger Kontrollen ist ein vollständiger Schutz gegen alle Gefahren jedoch nicht möglich.<br>GeoQuest verwendet an manchen Stellen zur Verschlüsselung den Industriestandard SSL (Secure Sockets Layer). Dadurch wird die Vertraulichkeit Ihrer persönlichen Angaben über das Internet gewährleistet.<br><br>III. Bekanntmachung von Veränderungen<br>Gesetzesänderungen oder Änderungen unserer unternehmensinternen Prozesse können eine Anpassung dieser Datenschutzerklärung erforderlich machen. Für den Fall einer solchen Änderung wird die App, sofern Sie online gehen, Sie informieren.<br>";
					break;
				case ProductIDs.Spielpunkte:
					downloadTimeOutSeconds = 60;
					colorProfile = "default";
					agbs = "Anbieter dieser App ist die QuestMill GmbH, Clostermannstr. 1, 51065 Köln. Der Anbieter dieser App betreibt ein Webportal, auf dem registrierte Mitglieder (nachfolgend „Autoren“) mithilfe des Global Position Systems (GPS) interaktive Touren zu verschiedenen Themenbereichen erstellen können (nachfolgend „GeoQuests“). Hierzu erstellt der Autor anhand von GPS-Koordinaten eine Route die sich unter Zuhilfenahme von Bildern, Texten, Videoclips und Voice-Messages mit verschiedenen Themenbereichen auseinandersetzt. Die GeoQuests werden sowohl registrierten, als auch nicht registrierten Nutzern über eine Smartphone-Applikation (nachfolgend „App“) zur Verfügung gestellt.<br><br>1.\tGeltungsbereich der Nutzungsbedingungen<br>Diese Nutzungsbedingungen gelten für alle Inhalte und Dienste, die der Anbieter im Rahmen des Webportals und der App angeboten werden. Die Nutzungsbedingungen sind jederzeit abrufbar.   <br>Entgegenstehende oder diesen Nutzungsbedingungen abweichende Bedingungen des Nutzer haben keine Geltung.  <br><br>1.\tBereitstellungsbedingungen<br>1.1.\tDurch die unentgeltliche Bereitstellung sowie durch einen entsprechenden Abruf der GeoQuests seitens der nicht registrierten Nutzer wird kein Vertragsverhältnis zwischen dem Anbieter und dem jeweiligen Nutzer begründet. <br>1.2.\tDer Anbieter ist bemüht, einen ordnungsgemäßen Betrieb des Webportals sicherzustellen, steht jedoch nicht für die ununterbrochene Nutzbarkeit bzw. Erreichbarkeit des Webportals ein. Dies gilt insbesondere für technisch bedingte Verzögerungen im Rahmen von Wartungsarbeiten oder Weiterentwicklungen, Unterbrechungen oder Ausfälle der Angebote, des Internets oder des Zugangs zum Internet. <br>1.3.\tDer Anbieter behält sich vor, Teile der Angebote, einzelne Angebote oder alle Angebote als Ganzes ohne gesonderte Vorankündigung zu verändern oder die Veröffentlichung zeitweise oder endgültig einzustellen. Entschädigungsansprüche des Nutzers entstehen hieraus nicht. <br><br>2.\tHaftung und Haftungsbeschränkung <br>2.1.\tDer Anbieter ist nicht verpflichtet und auch nicht in der Lage, die Rechtmäßigkeit der von Nutzern oder den Autoren hochgeladenen oder publizierten Inhalte umfassend zu prüfen, zu überwachen und/oder nach Umständen zu forschen, die auf eine rechtswidrige Tätigkeit hinweisen. Das gleiche gilt, wenn und soweit von den Angeboten des Anbieters auf Webseiten Dritter verlinkt oder verwiesen wird. Der Anbieter macht sich die von Nutzern hochgeladenen oder publizierten Inhalte sowie die auf den Webseiten Dritter liegenden, durch Link verknüpften Inhalte nicht zu Eigen. Der Anbieter steht nicht dafür ein, dass diese Inhalte rechtmäßig, korrekt, aktuell und/oder vollständig sind. Für Schäden, die Aufgrund der Nutzung dieser Inhalte entstehen haftet der Anbieter nicht.<br>2.3.\tWenn der Anbieter Hinweise auf Gesetzesverstöße oder Rechtsverletzungen durch fremde oder verlinkte Inhalte erhält, wird der Anbieter soweit eine Pflicht besteht die Inhalte überprüfen und erforderlichenfalls sperren und löschen. <br>2.4.\tDer Anbieter haftet für Schadensersatz oder Ersatz vergeblicher Aufwendungen unbeschränkt bei Vorsatz oder grober Fahrlässigkeit, für die Verletzung von Leben, Leib oder Gesundheit, nach den Vorschriften des Produkthaftungsgesetzes.<br>2.5.\tBei leicht fahrlässiger Verletzung einer Pflicht, die wesentlich für die Erreichung des Vertragszwecks ist (Kardinalpflicht), ist die Haftung des Anbieters der Höhe nach begrenzt auf den Schaden, der nach der Art des fraglichen Geschäfts vorhersehbar und typisch ist.<br>2.6.\tDie vorstehende Haftungsbeschränkung der Ziff. 2.4. – 2.5. gilt auch für die persönliche Haftung der Mitarbeiter, Vertreter und Organe des Anbieters. <br>2.7.\tEine darüber hinausgehende Haftung besteht nicht. <br><br>3.\tOffenlegung von Informationen <br>Der Anbieter ist berechtigt, vorhandene Informationen von und über Nutzer ohne dessen ausdrückliches Einverständnis an Dritte herauszugeben, soweit er hierzu verpflichtet oder dies nach pflichtgemäßen Ermessen notwendig und rechtlich zulässig ist, um gesetzliche Bestimmungen oder richterliche oder behördliche Anordnungen zu erfüllen. <br><br>4.\tAnwendbares Recht, Gerichtsstand<br>4.1.\tEs gilt das Recht der Bundesrepublik Deutschland unter Ausschluss der Normen des Kollisionsrechts und des UN-Kaufrechts.<br><br>4.2.\tGerichtsstand für alle Streitigkeiten ist – soweit gesetzlich zulässig – der Sitz des Anbieters. <br>5.\tSalvatorische Klausel <br>Sollten einzelne Regelungen der Nutzungsbedingungen unwirksam sein oder werden, wird dadurch die Wirksamkeit der übrigen Regelungen nicht berührt. Sofern Teile der Nutzungsbedingungen der geltenden Rechtslage nicht, nicht mehr oder nicht vollständig entsprechen sollten, bleiben die übrigen Teile in ihrem Inhalt und ihrer Gültigkeit davon unberührt und der ungültige Teil gilt als durch einen solchen ersetzt, der dem Sinn und Zweck des unwirksamen Teils bzw. dem Parteiwillen in rechtswirksamer Weise wirtschaftlich am nächsten kommt. <br>6.\tÄnderungsvorbehalt<br>Der Anbieter hat das Recht, Bestimmungen dieser Nutzungsbedingungen zu ändern, wenn ein wichtiger Grund hierfür vorliegt. <br>";
					privacyAgreement = "Wir freuen uns über Ihr Interesse an unserer App. Als Anbieter dieser App ist uns der sichere Umgang mit Ihren Daten besonders wichtig. Die Erhebung, Verarbeitung und Nutzung Ihrer personenbezogenen Daten geschieht ausschließlich unter Beachtung der geltenden datenschutzrechtlichen Bestimmungen und des Telemediengesetzes (TMG). Ohne Ihre Zustimmung werden wir Ihre personenbezogenen Daten nicht auf anderem Wege nutzen, als er sich aus oder im Zusammenhang mit dieser Datenschutzerklärung ergibt. Wir möchten Ihnen nachfolgend erläutern, welche Daten wir wann und zu welchem Zweck erheben, verarbeiten und nutzen.<br><br>1. Erhebung, Verarbeitung und Nutzung personenbezogener Daten<br>Personenbezogene Daten sind Einzelangaben über persönliche und sachliche Verhältnisse einer bestimmten oder bestimmbaren Person also Daten die Rückschlüsse über eine Person zulassen. <br>Mit der Installation von Tap-Erlebnis erheben wir folgende Daten:<br>a. Registrierung und Nutzung <br>Die Nutzung dieser App ist ohne Registrierung möglich. <br>b. Standortdaten<br>Wir benötigen den Zugriff auf den Standort Ihres Gerätes. Ihre GPS-Daten werden erhoben, gespeichert und an den Anbieter übermittelt. Die GPS-Daten sind notwendig, damit der Nutzer über Inhalte in seiner näheren Umgebung informiert wird. Daten zu Ihrem Standort werden nur für diese Bearbeitung Ihrer Anfrage genutzt. Ihre Standortdaten werden nach Beendigung Ihrer Anfrage nicht gespeichert und nicht weitergegeben.<br>c. Aktives Kontaktieren<br>Der Nutzer gestattet, dass der Betreiber die App aktiv kontaktiert, sofern diese online genutzt wird, um über Aktualisierungen der Datenschutzerklärung zu informieren.<br><br>II. Datensicherheit<br>Wir sichern unsere Systeme durch technische und organisatorische Maßnahmen gegen Verlust, Zerstörung, Zugriff, Veränderung oder Verbreitung Ihrer Daten durch unbefugte Personen. Trotz regelmäßiger Kontrollen ist ein vollständiger Schutz gegen alle Gefahren jedoch nicht möglich.<br>GeoQuest verwendet an manchen Stellen zur Verschlüsselung den Industriestandard SSL (Secure Sockets Layer). Dadurch wird die Vertraulichkeit Ihrer persönlichen Angaben über das Internet gewährleistet.<br><br>III. Bekanntmachung von Veränderungen<br>Gesetzesänderungen oder Änderungen unserer unternehmensinternen Prozesse können eine Anpassung dieser Datenschutzerklärung erforderlich machen. Für den Fall einer solchen Änderung wird die App, sofern Sie online gehen, Sie informieren.<br>";
					break;
				case ProductIDs.Intern:
				default:
					downloadTimeOutSeconds = 60;
					colorProfile = "default";
					break;
			}

			return;
		}

		public bool metaCategoryIsSearchable (string category) {

			if ( metaCategoryUsage != null && metaCategoryUsage.Count > 0 ) {

				foreach ( QuestMetaCategory qmc in metaCategoryUsage ) {

					if ( qmc.name.ToUpper().Equals(category.ToUpper()) && qmc.considerInSearch ) {

						return true;

					}

				}

			}

			return false;
		}



		public QuestMetaCategory getMetaCategory (string category) {

			if ( metaCategoryUsage != null && metaCategoryUsage.Count > 0 ) {

				foreach ( QuestMetaCategory qmc in metaCategoryUsage ) {

					if ( qmc.name.ToUpper().Equals(category.ToUpper()) && qmc.considerInSearch ) {

						return qmc;

					}

				}

			}

			return null;
		}

	}






	[System.Serializable]
	public class Language {
	
		public string bezeichnung;
		public string anzeigename_de;
		public Sprite sprite;
		public bool available = true;

	}




	[System.Serializable]
	public class QuestMetaCategory {
		public string name;

		public bool considerInSearch;
		public bool filterButton;

		public List<string> possibleValues;

		public List<string> chosenValues;


		public void addPossibleValues (string values) {

			Debug.Log(values);


			if ( values.Contains(",") ) {


				List<string> split = new List<string>();
				split.AddRange(values.Split(','));


				foreach ( string s1 in split ) {

					if ( split.Any(s => s.ToUpper().Equals(s1.ToUpper())) ) {

						// is in already

					}
					else {

						if ( !s1.Equals("") ) {

							possibleValues.Add(s1);
						}

					}


				}

			}
			else {

				if ( !values.Equals("") ) {

					possibleValues.Add(values);

				}


			}

		}


		public bool isChosen (string s1) {
			if ( chosenValues.Any(s => s.Equals(s1, StringComparison.OrdinalIgnoreCase)) ) {

				return true;

			}

			return false;
		}



		public void chooseValue (string s1) {


			if ( chosenValues.Any(s => s.Equals(s1, StringComparison.OrdinalIgnoreCase)) ) {

				// is in already

			}
			else {


				chosenValues.Add(s1);


			}

		}


		public void unchooseValue (string s1) {

			foreach ( string s in chosenValues.GetRange(0,chosenValues.Count) ) {


				if ( s.ToUpper().Equals(s1.ToUpper()) ) {


					chosenValues.Remove(s);

				}

			}




		}

	}

}