using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using GQ.Client.Err;
using System.Collections.Generic;

namespace GQ.Client.Model
{

	public class GQML
	{

		#region Tag Names

		// QUEST:
		public const string QUEST = "game";
		public const string QUEST_ID = "id";
		public const string QUEST_NAME = "name";
		public const string QUEST_LASTUPDATE = "lastUpdate";
		public const string QUEST_XMLFORMAT = "xmlformat";
		public const string QUEST_INDIVIDUAL_RETURN_DEFINITIONS = "individualReturnDefinitions";

		// PAGES GENERIC:
		public const string PAGE = "mission";
		public const string PAGE_ID = "id";
		public const string PAGE_TYPE = "type";
		public const string PAGE_TYPE_MULTIPLE_CHOICE_QUESTION = "MultipleChoiceQuestion";
		public const string PAGE_TYPE_TEXT_QUESTION = "TextQuestion";

		// IMAGE_WITH_TEXT PAGE:
		public const string PAGE_TYPE_IMAGEWITHTEXT = "NPCTalk";
		public const string PAGE_IMAGEWITHTEXT_IMAGEURL = "image";
		public const string PAGE_IMAGEWITHTEXT_TEXTSIZE = "textsize";
		public const string PAGE_IMAGEWITHTEXT_TEXT = "text";
		public const string PAGE_IMAGEWITHTEXT_ENDBUTTONTEXT = "endbuttontext";

		// NPC_TALK PAGE:
		public const string PAGE_TYPE_NPCTALK = "NPCTalk";
		public const string PAGE_NPCTALK_ENDBUTTONTEXT = "endbuttontext";
		public const string PAGE_NPCTALK_IMAGEURL = "image";
		public const string PAGE_NPCTALK_DISPLAYMODE = "mode";
		public const string PAGE_NPCTALK_DISPLAYMODE_ALL_AT_ONCE = "Komplett anzeigen";
		public const string PAGE_NPCTALK_DISPLAYMODE_WORD_BY_WORD = "Wordticker";
		public const string PAGE_NPCTALK_NEXTBUTTONTEXT = "nextdialogbuttontext";
		public const string PAGE_NPCTALK_SKIPWORDTICKER = "skipwordticker";
		public const string PAGE_NPCTALK_TEXTSIZE = "textsize";
		public const string PAGE_NPCTALK_TICKERSPEED = "tickerspeed";
		// DIALOG_ITEM:
		public const string PAGE_NPCTALK_DIALOGITEM = "dialogitem";
		public const string PAGE_NPCTALK_DIALOGITEM_BLOCKING = "blocking";
		public const string PAGE_NPCTALK_DIALOGITEM_AUDIOURL = "sound";
		public const string PAGE_NPCTALK_DIALOGITEM_SPEAKER = "speaker";

		// START_AND_EXIT_SCREEN PAGE:
		public const string PAGE_TYPE_STARTANDEXITSCREEN = "StartAndExitScreen";
		public const string PAGE_STARTANDEXITSCREEN_IMAGEURL = "image";
		public const string PAGE_STARTANDEXITSCREEN_DURATION = "duration";
		public const string PAGE_STARTANDEXITSCREEN_FPS = "fps";
		public const string PAGE_STARTANDEXITSCREEN_LOOP = "loop";

		// METADATA PAGE:
		public const string PAGE_TYPE_METADATA = "MetaData";
		public const string PAGE_METADATA_STRINGMETA = "stringmeta";
		public const string PAGE_METADATA_STRINGMETA_KEY = "key";
		public const string PAGE_METADATA_STRINGMETA_VALUE = "value";

		// TRIGGER:
		public const string ON_START = "onStart";
		public const string ON_SUCCESS = "onSuccess";
		public const string ON_FAIL = "onFail";
		public const string ON_END = "onEnd";
		public const string ON_ENTER = "onEnter";
		public const string ON_LEAVE = "onLeave";
		public const string ON_TAP = "onTap";


		// RULES & ACTIONS:
		public const string RULE = "rule";
		public const string ACTION = "action";
		public const string ACTION_TYPE = "type";
		public const string ACTION_ATTRIBUTE_VARNAME = "var";
		public const string ACTION_ATTRIBUTE_FROMVARNAME = "FromVar";
		public const string ACTION_PLAYAUDIO_FILE = "file";
		public const string ACTION_PLAYAUDIO_LOOP = "loop";
		public const string ACTION_PLAYAUDIO_STOPOTHERS = "stopOthers";
		public const string ACTION_SETVARIABLE_VALUE = "value";
		public const string ACTION_STARTMISSION_ALLOWRETURN = "allowReturn";

		// IF ACTION:
		public const string IF = "if";
		public const string THEN = "then";
		public const string ELSE = "else";

		//CONDITIONS:
		public const string CONDITION = "condition";
		public const string AND = "and";
		public const string OR = "or";
		public const string NOT = "not";
		public const string GREATER_THAN = "gt";
		public const string LESS_THAN = "lt";
		public const string EQUAL = "eq";
		public const string GREATER_EQUAL = "geq";
		public const string LESS_EQUAL = "leq";

		// EXPRESSIONS:
		public const string VARIABLE = "var";
		public const string BOOL = "bool";
		public const string NUMBER = "num";
		public const string STRING = "string";

		// HOTSPOTS:
		public const string HOTSPOT = "hotspot";
		public const string HOTSPOT_ID = "id";
		public const string HOTSPOT_MARKERURL = "img";
		public const string HOTSPOT_INITIAL_ACTIVITY = "initialActivity";
		public const string HOTSPOT_INITIAL_VISIBILITY = "initialVisibility";
		public const string HOTSPOT_NUMBER = "number";
		public const string HOTSPOT_RADIUS = "radius";
		public const string HOTSPOT_LATLONG = "latlong";
		public const string HOTSPOT_NFC = "nfc";
		public const string HOTSPOT_IBEACON = "iBeacon";
		public const string HOTSPOT_QRCODE = "qrcode";


		#endregion


		#region Predefined Values

		// STATES:
		public const string STATE_NEW = "new";
		public const string STATE_SUCCEEDED = "succeeded";
		public const string STATE_FAILED = "failed";
		public const string STATE_RUNNING = "running";

		// SYSTEM VARIABLE NAME PREFIXES:
		public const string VAR_PAGE_PREFIX = "$_mission_";

		// SYSTEM VARIABLE NAME PARTS:
		public const string VAR_PAGE_STATE = ".state";
		public const string VAR_PAGE_RESULT = ".result";

		#endregion


		#region Util Functions

		public static int GetIntAttribute (string attributeName, XmlReader reader, int defaultVal = 0)
		{
			string attString = getAttr (attributeName, "Int", reader);
			int val = defaultVal;
			if (!Int32.TryParse (attString, out val)) {
				Log.SignalErrorToDeveloper (
					"Int attribute {0} for a page could not be parsed. We found: {1}.", 
					attributeName, 
					attString);
			}

			return val;
		}

		public static long GetLongAttribute (string attributeName, XmlReader reader, long defaultVal = 0L)
		{
			string attString = getAttr (attributeName, "Long", reader);
			long val = defaultVal;
			if (!Int64.TryParse (attString, out val)) {
				Log.SignalErrorToDeveloper (
					"Long attribute {0} for a page could not be parsed. We found: {1}.", 
					attributeName, 
					attString);
			}

			return val;
		}

		public static double GetDoubleAttribute (string attributeName, XmlReader reader, double defaultVal = 0d)
		{
			string attString = getAttr (attributeName, "Double", reader);
			double val = defaultVal;
			if (!Double.TryParse (attString, out val)) {
				Log.SignalErrorToDeveloper (
					"Double attribute {0} for a page could not be parsed. We found: {1}.", 
					attributeName, 
					attString);
			}

			return val;
		}

		public static bool GetOptionalBoolAttribute (string attributeName, XmlReader reader, bool defaultVal = false)
		{
			string attString = getAttr (attributeName, "Bool", reader);
			if (attString == null)
				return defaultVal;

			bool result = false;
			result |= attString.ToLower ().Equals ("true");
			result |= attString.ToLower ().Equals ("1");
			return result;
		}

		public static bool GetRequiredBoolAttribute (string attributeName, XmlReader reader)
		{
			string attString = getAttr (attributeName, "Bool", reader);
			bool val = false;
			if (!bool.TryParse (attString, out val)) {
				Log.SignalErrorToDeveloper (
					"Bool attribute {0} for a page could not be parsed. We found: {1}.", 
					attributeName, 
					attString);
			}

			return val;
		}

		public static string GetStringAttribute (string attributeName, XmlReader reader, string defaultVal = "")
		{
			string val = getAttr (attributeName, "String", reader);
			return (val == null ? defaultVal : val);
		}

		/// <summary>
		/// Returns either the attribute string value or null.
		/// </summary>
		/// <returns>The attr.</returns>
		/// <param name="attributeName">Attribute name.</param>
		/// <param name="typeName">Type name.</param>
		/// <param name="reader">Reader.</param>
		private static string getAttr (string attributeName, string typeName, XmlReader reader)
		{
			if (attributeName == null) {
				Log.SignalErrorToDeveloper ("Tried to read an {0} attribute, but the name was null.", typeName);
				return null;
			}
			if (attributeName.Equals ("")) {
				Log.SignalErrorToDeveloper ("Tried to read an {0} attribute, but the name was empty.", typeName);
				return null;
			}

			return reader.GetAttribute (attributeName);
		}

		private static List<string> expressionNodeNames = 
			new List<string> (
				new string[] { NUMBER, STRING, BOOL, VARIABLE });


		internal static bool IsExpressionType (string xmlExpressionCandidate)
		{
			return expressionNodeNames.Contains (xmlExpressionCandidate);
		}

		private static List<string> conditionNodeNames = 
			new List<string> (
				new string[] { AND, OR, NOT, GREATER_THAN, GREATER_EQUAL, EQUAL, LESS_EQUAL, LESS_THAN });


		internal static bool IsConditionType (string xmlConditionCandidate)
		{
			return conditionNodeNames.Contains (xmlConditionCandidate);
		}



		#endregion


		#region Assertions

		public static void AssertReaderAtStart (XmlReader reader, string nodeName)
		{
			if (!IsReaderAtStart (reader, nodeName)) {
				Log.SignalErrorToDeveloper (
					"Expected a {0} element but we got a {1} with name {2}", 
					nodeName,
					reader.NodeType.ToString (),
					reader.LocalName
				);
			}
		}

		public static bool IsReaderAtStart (XmlReader reader, string nodeName)
		{
			return reader.NodeType == XmlNodeType.Element && reader.LocalName.Equals (nodeName);
		}

		public static void AssertReaderAtEnd (XmlReader reader, string nodeName)
		{
			if (!IsReaderAtEnd (reader, nodeName)) {
				Log.SignalErrorToDeveloper (
					"Expected a {0} end element but we got a {1} with name {2}", 
					nodeName,
					reader.NodeType.ToString (),
					reader.LocalName
				);
			}
		}

		public static bool IsReaderAtEnd (XmlReader reader, string nodeName)
		{
			return reader.LocalName.Equals (nodeName) && (reader.NodeType == XmlNodeType.EndElement || reader.IsEmptyElement);
		}

		#endregion

	}
}
