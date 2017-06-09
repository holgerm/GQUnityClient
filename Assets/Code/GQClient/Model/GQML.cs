using UnityEngine;
using System.Collections;

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

		// PAGES:
		public const string PAGE = "mission";
		public const string PAGE_TYPE = "type";
		public const string PAGE_TYPE_NPCTALK = "NPCTalk";
		public const string PAGE_TYPE_MULTIPLE_CHOICE_QUESTION = "MultipleChoiceQuestion";
		public const string PAGE_TYPE_TEXT_QUESTION = "TextQuestion";

		// RULES & ACTIONS:
		public const string RULE = "rule";
		public const string ACTION = "action";
		public const string ACTION_TYPE = "type";
		public const string ACTION_ATTRIBUTE_VARNAME = "var";
		public const string ACTION_ATTRIBUTE_FROMVARNAME = "FromVar";
		public const string ACTION_SETVARIABLE_VALUE = "value";

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

		#endregion


		#region Predefined Values

		// PAGE STATES:
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

	}
}
