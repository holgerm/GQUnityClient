using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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
		Odysseum
	}

	public ProductIDs product;
	public bool overrideProductSettingsInInspector = false;
	public int portalID = 1;
	public int autostartQuestID = 0;
	public bool autostartIsPredeployed = false;
	public string colorProfile = "default";
	public string mapboxKey = "pk.eyJ1IjoiZ3F3Y2MiLCJhIjoiTFhiakh3WSJ9.lDYp_76i3_uE5cSd1BQmuA";
	public string mapboxMapID = "gqwcc.m824ig5p";
	public int downloadTimeOutSeconds = 300;
	public string impressum = "";
	public Sprite toplogo;
	private string productName;
	public string nameForQuest = "Quest";
	public string questvisualization = "list";
	public bool showMessageForDatasendAction = true;
	public bool showPrivacyAgreement = true;
	public string privacyAgreement = "";
	public int privacyAgreementVersion = -1;
	public bool showAGBs = true;
	public string agbs = "";
	public int agbsVersion = -1;
	public bool questvisualizationchangable = false;
	public bool showcloudquestsimmediately = false;
	public bool showtextinloadinglogo = true;
	public bool showinternetconnectionmessage = true;
	public string defaultlanguage = "system";
	public bool languageChangableByUser = true;
	public List<Language> languagesAvailable;
	public List<MarkerCategorySprite> categoryMarker;
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
	public bool useMapOffline = false;

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
//		Debug.Log ("CONFIG: setting product: " + product);
		switch ( product ) {
			case ProductIDs.CarlBenz:
				productName = "carlbenz";
				portalID = 141;
				autostartQuestID = 6088;
				downloadTimeOutSeconds = 600;
				colorProfile = "carlbenz";
				mapboxKey = "pk.eyJ1IjoiZ3FjYXJsYmVueiIsImEiOiIxY21SYWZZIn0.IHbffn5Xk5xh-cwoLOfB_A";
				mapboxMapID = "gqcarlbenz.ac1f8820";
				impressum = "<b>Impressum</b>\n\nStadtmuseum im Prinz-Max­Palais\n\n<b>Leitung:</b>\nDr. Peter Pretsch\n\n<b>Adresse:</b>\nKarlstraße 10\n76133 Karlsruhe\n\nTel.: 0721-133 4230, 4231, 4234\nFax: 0721-133 4239\n\nstadtmuseum@kultur.karlsruhe.de\n\n<b>Öffnungszeiten:</b>\nDi, Fr 10-18 Uhr • Do 10-19 Uhr,\nSa 14-18 Uhr • So 11-18 Uhr\nMo, Mi geschlos­­sen\n\n<b>Straßenbahnlinien:</b>\nTram 1, 2, 3, 4, 6 und S-Bahn S1, S5, S11\nHaltestelle Europaplatz";
				break;
			case ProductIDs.WikiCultureCity:
				productName = "wcc";
				portalID = 281;
				autostartQuestID = 0;
				downloadTimeOutSeconds = 60;
				colorProfile = "wcc";
				mapboxKey = "pk.eyJ1IjoiZ3F3Y2MiLCJhIjoiTFhiakh3WSJ9.lDYp_76i3_uE5cSd1BQmuA";
				mapboxMapID = "gqwcc.m824ig5p";
				impressum = "<b>Titel</b>\n WikiCultureCity\n\n <b>Beschreibung</b>\n Anbieterkennzeichnung der WikiCultureCity GbR, Sebastianstr. 39, D-53115 Bonn\n\n <b>Einrichtung / Institution</b>\n WikiCultureCity GbR\n\n <b>Rechtsform</b>\n Gesellschaft bürgerlichen Rechts (GbR)\n\n <b>Vertreten durch</b>\n Frank Wittwer\n\n <b>Straße</b>\n Sebastianstr. 39\n\n <b>Ort</b>\n D-53115 Bonn\n\n <b>E-Mail</b>\n f.wittwer@wikiculturecity.com\n\n <b>Land</b>\n Deutschland\n\n <b>Domain</b>\n wikiculturecity.com\n\n <b>Umsatzsteuer-Identifikations-Nr.</b>\n FEHLT NOCH\n\n <b>Inhaltlich verantwortlich (v.i.S.d.P.)</b>\n Frank Wittwer\n\n <b>Technisch verantwortlich</b>\n QuestMill GmbH\n\n \n <b>Rechtliche Hinweise</b>\n \n Alle Angaben unserer mobilen App wurden sorgfältig geprüft. Wir bemühen uns, dieses Informationsangebot aktuell und inhaltlich richtig sowie vollständig anzubieten. Dennoch ist das Auftreten von Fehlern nicht völlig auszuschließen. Eine Garantie für die Vollständigkeit, Richtigkeit und letzte Aktualität kann daher nicht übernommen werden. Der Herausgeber kann diese Webseite nach eigenem Ermessen jederzeit ohne Ankündigung verändern und / oder deren Betrieb einstellen. Er ist nicht verpflichtet, Inhalte dieser Webseite zu aktualisieren. Der Zugang und die Benutzung dieses Angebots geschieht auf eigene Gefahr des Benutzers. WikiCultureCity ist nicht verantwortlich und übernimmt keinerlei Haftung für Schäden, u. a. für direkte, indirekte, zufällige, vorab konkret zu bestimmende oder Folgeschäden, die angeblich durch den oder in Verbindung mit dem Zugang und/oder der Benutzung dieses Angebots aufgetreten sind. Der Betreiber übernimmt keine Verantwortung für die Inhalte und die Verfügbarkeit von Websites Dritter, die über externe Links dieses Informationsangebotes erreicht werden. Der Herausgeber distanziert sich ausdrücklich von allen Inhalten, die möglicherweise straf- oder haftungsrechtlich relevant sind. Bei der Anforderung von Inhalten aus diesem Internetangebot werden Zugriffsdaten gespeichert. Das sind Daten über die Seite, von der aus die Datei angefordert wurde, der Name der angeforderten Datei, das Datum, Uhrzeit und Dauer der Anforderung, die übertragene Datenmenge sowie der Zugriffsstatus (z.B. Datei übertragen, Datei nicht gefunden). Diese gespeicherten Daten werden ausschließlich zu statistischen Zwecken ausgewertet und auf keinen Fall an Dritte weitergeleitet. Sofern innerhalb des Internetangebotes die Möglichkeit zur Eingabe persönlicher oder geschäftlicher Daten (E-Mailadressen, Namen, Anschriften) besteht, so erfolgt die Preisgabe dieser Daten seitens des Nutzers auf ausdrücklich freiwilliger Basis Bilder und Text die von Dritten, den Teilnehmen die Beiträge und Bilder bei WCC einstellen, haften diese für die Inhalte und Darstellung. Regelmäßig, wenn nicht etwas anders ausdrücklich zu einzelnen Bildern und Texten angegeben wird, haben diejenige, die diese Texte und Bilder einstellen, die Rechte an Bild und Text und gestatten WCC die uneingeschränkte Verwendung auf der Plattform von WCC. ";
				agbs = "Anbieter dieser App ist die WikiCultureCity GbR, Sebastianstr. 39, D-53115 Bonn. Der Anbieter dieser App betreibt ein Webportal, auf dem registrierte Mitglieder (nachfolgend „Autoren“) mithilfe des Global Position Systems (GPS) interaktive Touren zu verschiedenen Themenbereichen erstellen können (nachfolgend „GeoQuests“). Hierzu erstellt der Autor anhand von GPS-Koordinaten eine Route die sich unter Zuhilfenahme von Bildern, Texten, Videoclips und Voice-Messages mit verschiedenen Themenbereichen auseinandersetzt. Die GeoQuests werden sowohl registrierten, als auch nicht registrierten Nutzern über eine Smartphone-Applikation (nachfolgend „App“) zur Verfügung gestellt.<br><br>1.\tGeltungsbereich der Nutzungsbedingungen<br>Diese Nutzungsbedingungen gelten für alle Inhalte und Dienste, die der Anbieter im Rahmen des Webportals und der App angeboten werden. Die Nutzungsbedingungen sind jederzeit abrufbar.   <br>Entgegenstehende oder diesen Nutzungsbedingungen abweichende Bedingungen des Nutzer haben keine Geltung.  <br><br>1.\tBereitstellungsbedingungen<br>1.1.\tDurch die unentgeltliche Bereitstellung sowie durch einen entsprechenden Abruf der GeoQuests seitens der nicht registrierten Nutzer wird kein Vertragsverhältnis zwischen dem Anbieter und dem jeweiligen Nutzer begründet. <br>1.2.\tDer Anbieter ist bemüht, einen ordnungsgemäßen Betrieb des Webportals sicherzustellen, steht jedoch nicht für die ununterbrochene Nutzbarkeit bzw. Erreichbarkeit des Webportals ein. Dies gilt insbesondere für technisch bedingte Verzögerungen im Rahmen von Wartungsarbeiten oder Weiterentwicklungen, Unterbrechungen oder Ausfälle der Angebote, des Internets oder des Zugangs zum Internet. <br>1.3.\tDer Anbieter behält sich vor, Teile der Angebote, einzelne Angebote oder alle Angebote als Ganzes ohne gesonderte Vorankündigung zu verändern oder die Veröffentlichung zeitweise oder endgültig einzustellen. Entschädigungsansprüche des Nutzers entstehen hieraus nicht. <br><br>2.\tHaftung und Haftungsbeschränkung <br>2.1.\tDer Anbieter ist nicht verpflichtet und auch nicht in der Lage, die Rechtmäßigkeit der von Nutzern oder den Autoren hochgeladenen oder publizierten Inhalte umfassend zu prüfen, zu überwachen und/oder nach Umständen zu forschen, die auf eine rechtswidrige Tätigkeit hinweisen. Das gleiche gilt, wenn und soweit von den Angeboten des Anbieters auf Webseiten Dritter verlinkt oder verwiesen wird. Der Anbieter macht sich die von Nutzern hochgeladenen oder publizierten Inhalte sowie die auf den Webseiten Dritter liegenden, durch Link verknüpften Inhalte nicht zu Eigen. Der Anbieter steht nicht dafür ein, dass diese Inhalte rechtmäßig, korrekt, aktuell und/oder vollständig sind. Für Schäden, die Aufgrund der Nutzung dieser Inhalte entstehen haftet der Anbieter nicht.<br>2.3.\tWenn der Anbieter Hinweise auf Gesetzesverstöße oder Rechtsverletzungen durch fremde oder verlinkte Inhalte erhält, wird der Anbieter soweit eine Pflicht besteht die Inhalte überprüfen und erforderlichenfalls sperren und löschen. <br>2.4.\tDer Anbieter haftet für Schadensersatz oder Ersatz vergeblicher Aufwendungen unbeschränkt bei Vorsatz oder grober Fahrlässigkeit, für die Verletzung von Leben, Leib oder Gesundheit, nach den Vorschriften des Produkthaftungsgesetzes.<br>2.5.\tBei leicht fahrlässiger Verletzung einer Pflicht, die wesentlich für die Erreichung des Vertragszwecks ist (Kardinalpflicht), ist die Haftung des Anbieters der Höhe nach begrenzt auf den Schaden, der nach der Art des fraglichen Geschäfts vorhersehbar und typisch ist.<br>2.6.\tDie vorstehende Haftungsbeschränkung der Ziff. 2.4. – 2.5. gilt auch für die persönliche Haftung der Mitarbeiter, Vertreter und Organe des Anbieters. <br>2.7.\tEine darüber hinausgehende Haftung besteht nicht. <br><br>3.\tOffenlegung von Informationen <br>Der Anbieter ist berechtigt, vorhandene Informationen von und über Nutzer ohne dessen ausdrückliches Einverständnis an Dritte herauszugeben, soweit er hierzu verpflichtet oder dies nach pflichtgemäßen Ermessen notwendig und rechtlich zulässig ist, um gesetzliche Bestimmungen oder richterliche oder behördliche Anordnungen zu erfüllen. <br><br>4.\tAnwendbares Recht, Gerichtsstand<br>4.1.\tEs gilt das Recht der Bundesrepublik Deutschland unter Ausschluss der Normen des Kollisionsrechts und des UN-Kaufrechts.<br><br>4.2.\tGerichtsstand für alle Streitigkeiten ist – soweit gesetzlich zulässig – der Sitz des Anbieters. <br>5.\tSalvatorische Klausel <br>Sollten einzelne Regelungen der Nutzungsbedingungen unwirksam sein oder werden, wird dadurch die Wirksamkeit der übrigen Regelungen nicht berührt. Sofern Teile der Nutzungsbedingungen der geltenden Rechtslage nicht, nicht mehr oder nicht vollständig entsprechen sollten, bleiben die übrigen Teile in ihrem Inhalt und ihrer Gültigkeit davon unberührt und der ungültige Teil gilt als durch einen solchen ersetzt, der dem Sinn und Zweck des unwirksamen Teils bzw. dem Parteiwillen in rechtswirksamer Weise wirtschaftlich am nächsten kommt. <br>6.\tÄnderungsvorbehalt<br>Der Anbieter hat das Recht, Bestimmungen dieser Nutzungsbedingungen zu ändern, wenn ein wichtiger Grund hierfür vorliegt. <br>";
				privacyAgreement = "Wir freuen uns über Ihr Interesse an unserer App. Als Anbieter dieser App ist uns der sichere Umgang mit Ihren Daten besonders wichtig. Die Erhebung, Verarbeitung und Nutzung Ihrer personenbezogenen Daten geschieht ausschließlich unter Beachtung der geltenden datenschutzrechtlichen Bestimmungen und des Telemediengesetzes (TMG). Ohne Ihre Zustimmung werden wir Ihre personenbezogenen Daten nicht auf anderem Wege nutzen, als er sich aus oder im Zusammenhang mit dieser Datenschutzerklärung ergibt. Wir möchten Ihnen nachfolgend erläutern, welche Daten wir wann und zu welchem Zweck erheben, verarbeiten und nutzen.<br><br>1. Erhebung, Verarbeitung und Nutzung personenbezogener Daten<br>Personenbezogene Daten sind Einzelangaben über persönliche und sachliche Verhältnisse einer bestimmten oder bestimmbaren Person also Daten die Rückschlüsse über eine Person zulassen. <br>Mit der Installation von Tap-Erlebnis erheben wir folgende Daten:<br>a. Registrierung und Nutzung <br>Die Nutzung dieser App ist ohne Registrierung möglich. <br>b. Standortdaten<br>Wir benötigen den Zugriff auf den Standort Ihres Gerätes. Ihre GPS-Daten werden erhoben, gespeichert und an den Anbieter übermittelt. Die GPS-Daten sind notwendig, damit der Nutzer über Inhalte in seiner näheren Umgebung informiert wird. Daten zu Ihrem Standort werden nur für diese Bearbeitung Ihrer Anfrage genutzt. Ihre Standortdaten werden nach Beendigung Ihrer Anfrage nicht gespeichert und nicht weitergegeben.<br>c. Aktives Kontaktieren<br>Der Nutzer gestattet, dass der Betreiber die App aktiv kontaktiert, sofern diese online genutzt wird, um über Aktualisierungen der Datenschutzerklärung zu informieren.<br><br>II. Datensicherheit<br>Wir sichern unsere Systeme durch technische und organisatorische Maßnahmen gegen Verlust, Zerstörung, Zugriff, Veränderung oder Verbreitung Ihrer Daten durch unbefugte Personen. Trotz regelmäßiger Kontrollen ist ein vollständiger Schutz gegen alle Gefahren jedoch nicht möglich.<br>GeoQuest verwendet an manchen Stellen zur Verschlüsselung den Industriestandard SSL (Secure Sockets Layer). Dadurch wird die Vertraulichkeit Ihrer persönlichen Angaben über das Internet gewährleistet.<br><br>III. Bekanntmachung von Veränderungen<br>Gesetzesänderungen oder Änderungen unserer unternehmensinternen Prozesse können eine Anpassung dieser Datenschutzerklärung erforderlich machen. Für den Fall einer solchen Änderung wird die App, sofern Sie online gehen, Sie informieren.<br>";
				break;
			case ProductIDs.ErzbistumKoeln:
				productName = "ebk";
				portalID = 121;
				autostartQuestID = 0;
				downloadTimeOutSeconds = 60;
				colorProfile = "ebk";
				mapboxKey = "pk.eyJ1IjoiaG9sZ2VybXVlZ2dlIiwiYSI6Im1MLW9rN2MifQ.6KebeI6zZ3QNe18n2AQyaw";
				mapboxMapID = "mapbox.streets";
				impressum = "Anbieterkennzeichnung gemäß § 5 Telemediengesetz (TMG) und Informationspflicht im Sinne des § 55 Abs. 2 Rundfunkstaatsvertrags (RStV).\n <b>Titel</b>\n GeoQuest\n <b>Einrichtung / Institution</b>\n Erzbistum Köln\n <b>Rechtsform</b>\n Körperschaft des öffentlichen Rechts (KdöR)\n <b>Vertreten durch</b>\n Generalvikar Dr. Dominik Meiering\n <b>Straße</b>\n Marzellenstraße 32\n <b>Ort</b>\n 50668 Köln\n <b>Telefon</b>\n 0049 (0)221 1642 0\n <b>Fax</b>\n 0049 (0)221 1642 1700\n <b>E-Mail</b>\n ehe-familie@erzbistum-koeln.de\n <b>Land</b>\n Deutschland\n <b>Domain</b>\n erzbistum-koeln.de\n <b>Umsatzsteuer-Identifikations-Nr.</b>\n DE 122 777 469 \n <b>Inhaltlich verantwortlich (v.i.S.d.P.)</b>\n Efi Goebel\n <b>Technisch verantwortlich</b>\n QuestMill GmbH\n";
				break;
			case ProductIDs.LWL:
				productName = "lwl";
				portalID = 402;
				autostartQuestID = 0;
				downloadTimeOutSeconds = 60;
				colorProfile = "lwl";
				mapboxKey = "pk.eyJ1IjoiaG9sZ2VybXVlZ2dlIiwiYSI6Im1MLW9rN2MifQ.6KebeI6zZ3QNe18n2AQyaw";
				mapboxMapID = "mapbox.streets";
				impressum = "<b>Titel</b>\n LWL Quest\n <b>Beschreibung</b>\n Anbieterkennzeichnung des Landschaftsverband Westfalen-Lippe <b>Einrichtung / Institution</b>\n LWL-Industriemuseum Westfälisches Landesmuseum für Industriekultur\n <b>Rechtsform</b>\n Körperschaft des öffentlichen Rechts (KdöR)\n <b>Vertreten durch</b>\n LWL-Direktor Matthias Löb\n <b>Straße</b>\n Grubenweg 5\n <b>Ort</b>\n 44388 Dortmund\n <b>Telefon</b>\n 0049 (0)231 6961-152\n <b>Land</b>\n Deutschland\n <b>Domain</b>\n lwl.org\n <b>Inhaltlich verantwortlich (v.i.S.d.P.)</b>\n Christiane Spänhoff\n <b>Technisch verantwortlich</b>\n QuestMill GmbH\n\n1. Haftung: Der Landschaftsverband Westfalen-Lippe ist als Dienstanbieter nach § 7 Abs. 1 Teledienstegesetz für die eigenen Inhalte, die er zur Nutzung bereit hält verantwortlich. Die Haftung für Schäden materieller oder ideeller Art, die durch die Nutzung der Inhalte verursacht werden, ist ausgeschlossen, sofern nicht Vorsatz oder grobe Fahrlässigkeit vorliegt. Der Landschaftsverband Westfalen-Lippe ist für den Inhalt fremder Websites, die er innerhalb seines Angebotes durch Hyperlinks zugänglich macht, entsprechend den Regelungen des § 8 Teledienstegesetz nicht verantwortlich.\n\n2. Urheber- und Kennzeichnungsrecht: Die verantwortlichen Redakteure sind bestrebt, in allen Publikationen die Urheberrechte der verwendeten Grafiken, Tondokumente, Videosequenzen und Texte zu beachten, von ihm selbst erstellte Grafiken, Tondokumente, Videosequenzen und Texte zu verwenden oder auf lizenzfreie Grafiken, Tondokumente, Videosequenzen und Texte zurückzugreifen. Alle innerhalb des Internetangebots genannten und ggf. durch Dritte geschützte Marken- und Warenzeichen unterliegen den Bestimmungen des jeweils gültigen Kennzeichenrechts und den Besitzrechten der jeweiligen eingetragenen Eigentümer. Allein aufgrund der bloßen jeweiligen Nennung ist nicht der Schluss zu ziehen, dass Markenzeichen nicht durch Rechte Dritter geschützt sind. Die Verantwortung für die Beachtung dieser Rechte liegt bei dem jeweiligen Nutzer. Das Copyright für veröffentlichte, vom Autor selbst erstellte Objekte bleibt allein beim Autor der Seiten. Eine Vervielfältigung oder Verwendung solcher Grafiken, Tondokumente, Videosequenzen und Texte in anderen Publikationen ist ohne Zustimmung des Autors nicht gestattet.";
				break;
			case ProductIDs.Public:
				productName = "public";
				portalID = 61;
				autostartQuestID = 0;
				downloadTimeOutSeconds = 60;
				colorProfile = "default";
				mapboxKey = "pk.eyJ1IjoiaG9sZ2VybXVlZ2dlIiwiYSI6Im1MLW9rN2MifQ.6KebeI6zZ3QNe18n2AQyaw";
				mapboxMapID = "mapbox.streets";
				impressum = "<b>Titel</b>\n GeoQuest\n <b>Beschreibung</b>\n Anbieterkennzeichnung der QuestMill GmbH,\n Clostermannstr. 1, \n51065 Köln <b>Einrichtung / Institution</b>\n QuestMill GmbH\n <b>Rechtsform</b>\n Gesellschaft mit beschränkter Haftung (GmbH)\n <b>Vertreten durch</b>\n Holger Mügge\n <b>Straße</b>\n Clostermannstr. 1\n <b>Ort</b>\n 51065 Köln\n <b>Telefon</b>\n 0049 (0)221 922 4343\n <b>Land</b>\n Deutschland\n <b>Domain</b>\n quest-mill.com\n <b>Umsatzsteuer-Identifikations-Nr.</b>\n DE298593210 \n <b>Inhaltlich verantwortlich (v.i.S.d.P.)</b>\n Holger Mügge\n <b>Technisch verantwortlich</b>\n QuestMill GmbH\n";
				break;
			case ProductIDs.Demos:
				productName = "demos";
				portalID = 341;
				autostartQuestID = 0;
				downloadTimeOutSeconds = 60;
				colorProfile = "default";
				mapboxKey = "pk.eyJ1IjoiaG9sZ2VybXVlZ2dlIiwiYSI6Im1MLW9rN2MifQ.6KebeI6zZ3QNe18n2AQyaw";
				mapboxMapID = "mapbox.streets";
				impressum = "<b>Titel</b>\n GeoQuest\n <b>Beschreibung</b>\n Anbieterkennzeichnung der QuestMill GmbH,\n Clostermannstr. 1, \n51065 Köln <b>Einrichtung / Institution</b>\n QuestMill GmbH\n <b>Rechtsform</b>\n Gesellschaft mit beschränkter Haftung (GmbH)\n <b>Vertreten durch</b>\n Holger Mügge\n <b>Straße</b>\n Clostermannstr. 1\n <b>Ort</b>\n 51065 Köln\n <b>Telefon</b>\n 0049 (0)221 922 4343\n <b>Land</b>\n Deutschland\n <b>Domain</b>\n quest-mill.com\n <b>Umsatzsteuer-Identifikations-Nr.</b>\n DE298593210 \n <b>Inhaltlich verantwortlich (v.i.S.d.P.)</b>\n Holger Mügge\n <b>Technisch verantwortlich</b>\n QuestMill GmbH\n";
				break;
			case ProductIDs.Odysseum:
				productName = "odysseum";
				portalID = 381;
				autostartQuestID = 0;
				downloadTimeOutSeconds = 60;
				colorProfile = "odysseum";
				mapboxKey = "pk.eyJ1IjoiaG9sZ2VybXVlZ2dlIiwiYSI6Im1MLW9rN2MifQ.6KebeI6zZ3QNe18n2AQyaw";
				mapboxMapID = "mapbox.streets";
				impressum = "<b>Titel</b>\n GeoQuest\n <b>Beschreibung</b>\n Anbieterkennzeichnung der QuestMill GmbH,\n Clostermannstr. 1, \n51065 Köln <b>Einrichtung / Institution</b>\n QuestMill GmbH\n <b>Rechtsform</b>\n Gesellschaft mit beschränkter Haftung (GmbH)\n <b>Vertreten durch</b>\n Holger Mügge\n <b>Straße</b>\n Clostermannstr. 1\n <b>Ort</b>\n 51065 Köln\n <b>Telefon</b>\n 0049 (0)221 922 4343\n <b>Land</b>\n Deutschland\n <b>Domain</b>\n quest-mill.com\n <b>Umsatzsteuer-Identifikations-Nr.</b>\n DE298593210 \n <b>Inhaltlich verantwortlich (v.i.S.d.P.)</b>\n Holger Mügge\n <b>Technisch verantwortlich</b>\n QuestMill GmbH\n";
				break;
			case ProductIDs.Intern:
			default:
				productName = "intern";
				portalID = 1;
				autostartQuestID = 0;
				downloadTimeOutSeconds = 60;
				colorProfile = "default";
				mapboxKey = "pk.eyJ1IjoiaG9sZ2VybXVlZ2dlIiwiYSI6Im1MLW9rN2MifQ.6KebeI6zZ3QNe18n2AQyaw";
				mapboxMapID = "mapbox.streets";
				impressum = "<b>Titel</b>\n GeoQuest\n <b>Beschreibung</b>\n Anbieterkennzeichnung der QuestMill GmbH,\n Clostermannstr. 1, \n51065 Köln <b>Einrichtung / Institution</b>\n QuestMill GmbH\n <b>Rechtsform</b>\n Gesellschaft mit beschränkter Haftung (GmbH)\n <b>Vertreten durch</b>\n Holger Mügge\n <b>Straße</b>\n Clostermannstr. 1\n <b>Ort</b>\n 51065 Köln\n <b>Telefon</b>\n 0049 (0)221 922 4343\n <b>Land</b>\n Deutschland\n <b>Domain</b>\n quest-mill.com\n <b>Umsatzsteuer-Identifikations-Nr.</b>\n DE298593210 \n <b>Inhaltlich verantwortlich (v.i.S.d.P.)</b>\n Holger Mügge\n <b>Technisch verantwortlich</b>\n QuestMill GmbH\n";
				break;
		}

		return;
	}

}


[System.Serializable]
public class MarkerCategorySprite {

	public string category;
	public string anzeigename_de;
	public Sprite sprite;
	public bool showInList = true;
	/// <summary>
	/// Flag that determines whether this category's markers will be shown on the map. 
	/// This is currently also the only state used for category markers and also reflects the state of the buttons etc.
	/// </summary>
	public bool showOnMap = true;

}




[System.Serializable]
public class Language {
	
	public string bezeichnung;
	public string anzeigename_de;
	public Sprite sprite;
	public bool available = true;

}







