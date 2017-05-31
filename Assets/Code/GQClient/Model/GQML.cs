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

		// PAGES (aka Missions):
		public const string PAGE = "mission";
		public const string PAGE_TYPE = "type";
		public const string PAGE_TYPE_NPCTALK = "NPCTalk";
		public const string PAGE_TYPE_MULTIPLE_CHOICE_QUESTION = "MultipleChoiceQuestion";
		public const string PAGE_TYPE_TEXT_QUESTION = "TextQuestion";

		// PAGE RESULTS:
		public const string RESULT_NEW = "new";
		public const string RESULT_SUCCEEDED = "succeeded";
		public const string RESULT_FAILED = "failed";
		public const string RESULT_RUNNING = "running";

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

	}
}
