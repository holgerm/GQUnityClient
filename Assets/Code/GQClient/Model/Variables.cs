using UnityEngine;
using System.Collections;

namespace GQ.Client.Model {

	public class Variables {
	
		public static readonly string GQ_VAR_PREFIX = Definitions.GQ_PREFIX + ".var.";
		public const char VAR_TYPE_DELIMITER = ':';
		public const string VARTYPE_NUM = "num";
		public const string VARTYPE_STRING = "string";
		public const string VARTYPE_BOOL = "bool";

		public static void SaveVariableToStore (QuestVariable questVar) {

			if ( questVar == null )
				return;

			string varname = Variables.GQ_VAR_PREFIX + questVar.key;

			PlayerPrefs.SetString(varname, questVar.ToString());
		}

		public static QuestVariable LoadVariableFromStore (string varName) {
			string prefKey = Variables.GQ_VAR_PREFIX + varName;

			if ( !PlayerPrefs.HasKey(prefKey) ) {
				Debug.Log(string.Format("WARNING: Tried to load variable {0} but didn't find it in store.", varName));
				return null;
			}

			string stringValue = PlayerPrefs.GetString(prefKey);

			int i = stringValue.IndexOf(VAR_TYPE_DELIMITER);
			if ( i == -1 ) {
				Debug.LogWarning("Loaded a variable from store with invalid content: " + stringValue);
				return null;
			}

			string type = stringValue.Substring(0, i);
			string value = stringValue.Substring(i + 1);

			Debug.Log(string.Format("Loaded Variable {2} type: {0}, value: {1}", type, value, varName));

			QuestVariable resultVar = null;

			switch ( type ) {
				case VARTYPE_NUM:
					resultVar = new QuestVariable(varName, double.Parse(value));
					break;
				case VARTYPE_STRING:
					resultVar = new QuestVariable(varName, value);
					break;
				case VARTYPE_BOOL:
					resultVar = new QuestVariable(varName, bool.Parse(value));
					break;
				default:
					Debug.LogWarning("Loaded a variable from store with unknown type: " + type);
					resultVar = null;
					break;
			}

			return resultVar;
		}

	}

}
