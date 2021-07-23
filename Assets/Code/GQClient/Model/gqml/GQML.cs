using System;
using System.Collections.Generic;
using System.Xml;
using Code.GQClient.Err;

namespace Code.GQClient.Model.gqml
{
    public class GQML
    {
        #region Tag Names

        // GENERIC:
        public const string ID = "id";

        // QUEST:
        public const string QUEST = "game";
        public const string QUEST_NAME = "name";
        public const string QUEST_LASTUPDATE = "lastUpdate";
        public const string QUEST_XMLFORMAT = "xmlformat";
        public const string QUEST_INDIVIDUAL_RETURN_DEFINITIONS = "individualReturnDefinitions";

        // PAGES GENERIC:
        public const string PAGE = "mission";
        public const string PAGE_TYPE = "type";
        public const string PAGE_TYPE_TEXT_QUESTION = "TextQuestion";

        // IMAGE_WITH_TEXT PAGE:
        public const string PAGE_TYPE_IMAGEWITHTEXT = "NPCTalk";
        public const string PAGE_IMAGEWITHTEXT_IMAGEURL = "image";
        public const string PAGE_IMAGEWITHTEXT_TEXTSIZE = "textsize";
        public const string PAGE_IMAGEWITHTEXT_TEXT = "text";
        public const string PAGE_IMAGEWITHTEXT_ENDBUTTONTEXT = "endbuttontext";

        // QUESTION PAGE COMMONS:
        public const string PAGE_QUESTION_LOOP_UNTIL_SUCCESS = "loopUntilSuccess";
        public const string PAGE_QUESTION_LOOP_BUTTON_TEXT = "loopButtonText";
        public const string PAGE_QUESTION_LOOP_BUTTON_TEXT_DEFAULT = ">>";
        public const string PAGE_QUESTION_LOOP_TEXT = "loopText";
        public const string PAGE_QUESTION_LOOP_IMAGE = "loopImage";
        public const string PAGE_QUESTION_QUESTION = "question";

        public const string PAGE_QUESTION_BACKGROUND_IMAGE = "bg";

        // ANSWER:
        public const string PAGE_QUESTION_ANSWER = "answer";

        // MULTIPLE_CHOICE_QUESTION PAGE:
        public const string PAGE_TYPE_MULTIPLECHOICEQUESTION = "MultipleChoiceQuestion";
        public const string PAGE_MULTIPLECHOICEQUESTION_SHOW_ONLY_IMAGES = "showOnlyImages";

        public const string PAGE_MULTIPLECHOICEQUESTION_SHUFFLE = "shuffle";

        // ANSWER:
        public const string PAGE_MULTIPLECHOICEQUESTION_ANSWER_CORRECT = "correct";
        public const string PAGE_MULTIPLECHOICEQUESTION_ANSWER_IMAGE = "image";

        // TEXT_QUESTION PAGE:
        public const string PAGE_TYPE_TEXTQUESTION = "TextQuestion";
        public const string PAGE_TEXTQUESTION_PROMPT = "prompt";

        // IMAGE CAPTURE PAGE:
        public const string PAGE_TYPE_IMAGECAPTURE = "ImageCapture";
        public const string PAGE_IMAGECAPTURE_BUTTONTEXT = "buttontext";
        public const string PAGE_IMAGECAPTURE_FILE = "file";
        public const string PAGE_IMAGECAPTURE_TASK = "task";
        public const string PAGE_IMAGECAPTURE_PREFER_FRONT_CAM = "preferfrontfacing";

        // AUDIO RECORD PAGE:
        public const string PAGE_TYPE_AUDIORECORD = "AudioRecord";
        public const string PAGE_AUDIORECORD_FILE = "file";
        public const string PAGE_AUDIORECORD_MAXRECTIME = "maxrectime";
        public const string PAGE_AUDIORECORD_TASK = "task";

        // NPC_TALK PAGE:
        public const string PAGE_TYPE_NPCTALK = "NPCTalk";
        public const string PAGE_NPCTALK_ENDBUTTONTEXT = "endbuttontext";
        public const string PAGE_NPCTALK_IMAGEURL = "image";
        public const string PAGE_NPCTALK_TEXT = "text";

        public const string PAGE_NPCTALK_NEXTBUTTONTEXT = "nextdialogbuttontext";

        // DIALOG_ITEM:
        public const string PAGE_NPCTALK_DIALOGITEM = "dialogitem";
        public const string PAGE_NPCTALK_DIALOGITEM_BLOCKING = "blocking";
        public const string PAGE_NPCTALK_DIALOGITEM_AUDIOURL = "sound";
        public const string PAGE_NPCTALK_DIALOGITEM_SPEAKER = "speaker";

        // NAVIGATION PAGE:
        public const string PAGE_TYPE_NAVIGATION = "Navigation";
        public const string PAGE_NAVIGATION_OPTION_MAP = "map";
        public const string PAGE_NAVIGATION_MAP_ZOOMLEVEL = "zoomlevel";
        public const string PAGE_NAVIGATION_OPTION_LIST = "list";
        public const string PAGE_NAVIGATION_OPTION_QR = "qr";
        public const string PAGE_NAVIGATION_TEXT_QR = "text_qr";
        public const string PAGE_NAVIGATION_TEXT_QR_NOTFOUND = "text_qr_notfound";
        public const string PAGE_NAVIGATION_OPTION_NFC = "nfc";
        public const string PAGE_NAVIGATION_TEXT_NFC = "text_nfc";
        public const string PAGE_NAVIGATION_TEXT_NFC_NOTFOUND = "text_nfc_notfound";
        public const string PAGE_NAVIGATION_OPTION_IBEACON = "ibeacon";
        public const string PAGE_NAVIGATION_TEXT_IBEACON = "text_ibeacon";
        public const string PAGE_NAVIGATION_TEXT_IBEACON_NOTFOUND = "text_ibeacon_notfound";
        public const string PAGE_NAVIGATION_OPTION_NUMBER = "number";
        public const string PAGE_NAVIGATION_TEXT_NUMBER = "text_number";
        public const string PAGE_NAVIGATION_TEXT_NUMBER_NOTFOUND = "text_number_notfound";

        // START_AND_EXIT_SCREEN PAGE:
        public const string PAGE_TYPE_STARTANDEXITSCREEN = "StartAndExitScreen";
        public const string PAGE_STARTANDEXITSCREEN_IMAGEURL = "image";
        public const string PAGE_STARTANDEXITSCREEN_DURATION = "duration";
        public const string PAGE_STARTANDEXITSCREEN_FPS = "fps";
        public const string PAGE_STARTANDEXITSCREEN_LOOP = "loop";

        // VIDEO PLAY PAGE:
        public const string PAGE_TYPE_VIDEOPLAY = "VideoPlay";
        public const string PAGE_VIDEOPLAY_FILE = "file";
        public const string PAGE_VIDEOPLAY_CONTROLLABLE = "controllable";
        public const string PAGE_VIDEOPLAY_VIDEOTYPE = "videotype";
        public const string PAGE_VIDEOPLAY_VIDEOTYPE_NORMAL = "Normal";
        public const string PAGE_VIDEOPLAY_VIDEOTYPE_360 = "360 Grad";
        public const string PAGE_VIDEOPLAY_VIDEOTYPE_YOUTUBE = "YouTube";
        
        // INTERACTIVE SPHERICAL IMAGE PAGE:
        public const string PAGE_TYPE_INTERACTIVESPHERICALIMAGE = "InteractiveSphericalImage";
        public const string PAGE_INTERACTIVESPHERICALIMAGE_IMAGE = "image";
        public const string PAGE_INTERACTIVESPHERICALIMAGE_INTERACTION = "interaction";
        public const string PAGE_INTERACTIVESPHERICALIMAGE_INTERACTION_ALTITUDE = "altitude";
        public const string PAGE_INTERACTIVESPHERICALIMAGE_INTERACTION_AZIMUTH = "azimuth";
        public const string PAGE_INTERACTIVESPHERICALIMAGE_INTERACTION_ICON = "icon";



        // WEBPAGE PAGE:
        public const string PAGE_TYPE_WEBPAGE = "WebPage";
        public const string PAGE_WEBPAGE_FILE = "file";
        public const string PAGE_WEBPAGE_URL = "url";
        public const string PAGE_WEBPAGE_ENDBUTTONTEXT = "endbuttontext";
        public const string PAGE_WEBPAGE_ENDBUTTONTEXT_CLOSED = "endbuttontextwhenclosed";
        public const string PAGE_WEBPAGE_ALLOW_LEAVE_ON_URL_CONTAINS = "allowleaveonurlcontains";
        public const string PAGE_WEBPAGE_ALLOW_LEAVE_ON_URL_DOESNOTCONTAIN = "allowleaveonurldoesnotcontain";
        public const string PAGE_WEBPAGE_ALLOW_LEAVE_ON_HTML_CONTAINS = "allowleaveonhtmlcontains";
        public const string PAGE_WEBPAGE_ALLOW_LEAVE_ON_HTML_DOESNOTCONTAIN = "allowleaveonhtmldoesnotcontain";
        public const string PAGE_WEBPAGE_LEAVE_ON_ALLOW = "leaveOnAllow";
        public const string PAGE_WEBPAGE_FULLSCREEN_LANDSCAPE = "fullscreensLandscape";

        // QR TAG SCANNER PAGE:
        public const string PAGE_TYPE_QRTAGSCANNER = "TagScanner";
        public const string PAGE_TYPE_QRTAGSCANNER_PROMPT = "taskdescription";

        public const string PAGE_TYPE_QRTAGSCANNER_SHOWTAGCONTENT = "showTagContent";

        // EXPECTED CODE:
        public const string PAGE_TYPE_QRTAGSCANNER_EXPECTEDCODE = "expectedCode";


        // READ_NFC PAGE:
        public const string PAGE_TYPE_READNFC = "ReadNFC";
        public const string PAGE_READNFC_IMAGEURL = "image";
        public const string PAGE_READNFC_SAVE2VAR = "saveToVar";
        public const string PAGE_READNFC_TEXT = "text";

        // METADATA PAGE:
        public const string PAGE_TYPE_METADATA = "MetaData";
        public const string PAGE_METADATA_STRINGMETA = "stringmeta";
        public const string PAGE_METADATA_STRINGMETA_KEY = "key";
        public const string PAGE_METADATA_STRINGMETA_VALUE = "value";

        // TRIGGER:
        public const string ON_START = "onStart";
        public const string ON_SUCCESS = "onSuccess";
        public const string ON_FAIL = "onFail";
        public const string ON_READ = "onRead";
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
        public const string ACTION_SETHOTSPOTSTATE_ACTIVITY = "activity";
        public const string ACTIVE = "aktiv";
        public const string INACTIVE = "inaktiv";
        public const string ACTION_SETHOTSPOTSTATE_VISIBILITY = "visibility";
        public const string VISIBLE = "sichtbar";
        public const string INVISIBLE = "unsichtbar";
        public const string ACTION_SETHOTSPOTSTATE_APPLYTOALL = "applyToAll";
        public const string ACTION_SHOWMESSAGE_BUTTONTEXT = "buttontext";
        public const string ACTION_SHOWMESSAGE_MESSAGE = "message";
        public const string ACTION_STARTMISSION_ALLOWRETURN = "allowReturn";
        public const string ACTION_STARTQUEST_QUEST = "quest";
        public const string ACTION_WRITETONFC_CONTENT = "content";
        public const string ACTION_UPLOADFILE_FILEREF = "fileref";
        public const string ACTION_UPLOADFILE_NAMEPREFIX = "nameprefix";
        public const string ACTION_UPLOADFILE_METHOD = "method";
        public const string ACTION_UPLOADFILE_URL = "url";
        public const string ACTION_UPLOADFILE_AUTH = "auth";

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

        // RUNTIME MEDIA PATH PREFIX:
        public const string PREFIX_RUNTIME_MEDIA = "@_";

        // SYSTEM VARIABLE NAME PARTS:
        public const string VAR_PAGE_STATE = ".state";
        public const string VAR_PAGE_RESULT = ".result";

        // PREDEFINED VARIABLES:
        public const string VAR_SCORE = "score";

        #endregion


        #region Util Functions

        public static int GetIntAttribute(string attributeName, XmlReader reader, int defaultVal = 0)
        {
            string attString = getAttr(attributeName, "Int", reader);
            if (null == attString)
                return defaultVal;

            int val;
            if (!Int32.TryParse(attString, out val))
            {
                return defaultVal;
            }

            return val;
        }

        public static long GetLongAttribute(string attributeName, XmlReader reader, long defaultVal = 0L)
        {
            string attString = getAttr(attributeName, "Long", reader);
            long val = defaultVal;
            Int64.TryParse(attString, out val);
            return val;
        }

        public static double GetDoubleAttribute(string attributeName, XmlReader reader, double defaultVal = 0d)
        {
            string attString = getAttr(attributeName, "Double", reader);
            double val = defaultVal;
            Double.TryParse(attString, out val);

            return val;
        }

        public static bool GetOptionalBoolAttribute(string attributeName, XmlReader reader, bool defaultVal = false)
        {
            string attString = getAttr(attributeName, "Bool", reader);
            if (attString == null)
                return defaultVal;

            bool result = false;
            result |= attString.ToLower().Equals("true");
            result |= attString.ToLower().Equals("1");
            return result;
        }

        /// accepts "1" as true and "0" as false plus the usual string representations for bools.
        public static bool GetRequiredBoolAttribute(string attributeName, XmlReader reader)
        {
            string attString = getAttr(attributeName, "Bool", reader);
            bool val = false;
            if (!bool.TryParse(attString, out val))
            {
                if (attString == "1")
                {
                    return true;
                }

                if (attString == "0")
                {
                    return false;
                }

                Log.SignalErrorToDeveloper(
                    "Bool attribute {0} for a page could not be parsed. We found: '{1}' line {2} pos {3}.",
                    attributeName,
                    attString,
                    ((IXmlLineInfo) reader).LineNumber,
                    ((IXmlLineInfo) reader).LinePosition);
            }

            return val;
        }

        public static string GetStringAttribute(string attributeName, XmlReader reader, string defaultVal = "")
        {
            string val = getAttr(attributeName, "String", reader);
            return (val == null ? defaultVal : val);
        }

        /// <summary>
        /// Returns either the attribute string value or null.
        /// </summary>
        /// <returns>The attr.</returns>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="typeName">Type name.</param>
        /// <param name="reader">Reader.</param>
        private static string getAttr(string attributeName, string typeName, XmlReader reader)
        {
            if (attributeName == null)
            {
                Log.SignalErrorToDeveloper("Tried to read an {0} attribute, but the name was null.", typeName);
                return null;
            }

            if (attributeName.Equals(""))
            {
                Log.SignalErrorToDeveloper("Tried to read an {0} attribute, but the name was empty.", typeName);
                return null;
            }

            return reader.GetAttribute(attributeName);
        }

        private static List<string> expressionNodeNames =
            new List<string>(
                new string[] {NUMBER, STRING, BOOL, VARIABLE});


        internal static bool IsExpressionType(string xmlExpressionCandidate)
        {
            return expressionNodeNames.Contains(xmlExpressionCandidate);
        }

        private static List<string> conditionNodeNames =
            new List<string>(
                new string[] {AND, OR, NOT, GREATER_THAN, GREATER_EQUAL, EQUAL, LESS_EQUAL, LESS_THAN});

        internal static bool IsConditionType(string xmlConditionCandidate)
        {
            return conditionNodeNames.Contains(xmlConditionCandidate);
        }

        #endregion


        #region Assertions

        public static void AssertReaderAtStart(XmlReader reader, string nodeName)
        {
            if (!IsReaderAtStart(reader, nodeName))
            {
                Log.SignalErrorToDeveloper(
                    "Expected a {0} element but we got a {1} with name '{2}' line {3} pos {4}",
                    nodeName,
                    reader.NodeType.ToString(),
                    reader.LocalName,
                    ((IXmlLineInfo) reader).LineNumber,
                    ((IXmlLineInfo) reader).LinePosition
                );
            }
        }

        public static bool IsReaderAtStart(XmlReader reader, string nodeName)
        {
            return reader.NodeType == XmlNodeType.Element && reader.LocalName.Equals(nodeName);
        }

        public static void AssertReaderAtEnd(XmlReader reader, string nodeName)
        {
            if (!IsReaderAtEnd(reader, nodeName))
            {
                Log.SignalErrorToDeveloper(
                    "Expected a {0} end element but we got a {1} with name '{2}' line {3} pos {4}",
                    nodeName,
                    reader.NodeType.ToString(),
                    reader.LocalName,
                    ((IXmlLineInfo) reader).LineNumber,
                    ((IXmlLineInfo) reader).LinePosition
                );
            }
        }

        public static bool IsReaderAtEnd(XmlReader reader, string nodeName)
        {
            return reader.LocalName.Equals(nodeName) &&
                   (reader.NodeType == XmlNodeType.EndElement || reader.IsEmptyElement);
        }

        #endregion
    }
}