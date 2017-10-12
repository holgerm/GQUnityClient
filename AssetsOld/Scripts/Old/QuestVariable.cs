using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine.Networking;
using GQ.Client.Net;
using GQ.Client.Conf;
using QM.NFC;
using GQ.Client.Util;
using GQ.Client.Model;

[System.Serializable]
public class QuestVariable {

	public string key;
	public string type;
	public List<string> string_value;
	public List<double> num_value;
	public List<bool> bool_value;
	public List<double> double_value;

	public QuestVariable (string k, string s) {
		string_value = new List<string>();
		string_value.Add(s);
		type = "string";
		key = k;
	}

	public QuestVariable (string k, double n) {

		num_value = new List<double>();
		num_value.Add(n);
		type = "num";
		key = k;
	}

	public QuestVariable (string k, bool b) {
		bool_value = new List<bool>();
		bool_value.Add(b);
		type = "bool";
		key = k;
	}

	public string getStringValue () {

		if ( string_value != null && string_value.Count > 0 ) {
			return string_value[0];
		}
		else
		if ( bool_value != null && bool_value.Count > 0 ) {
			if ( bool_value[0] ) {
				return "true";
			}
			else {
				return "false";
			}
		}
		else
		if ( num_value != null && num_value.Count > 0 ) {

			return num_value[0] + "";

		}
		else {

			return "[null]";

		}

	}

	public override string ToString () {

		if ( type == "bool" ) {

			return type + Variables.VAR_TYPE_DELIMITER + bool_value[0].ToString();
		}
		else
		if ( type == "num" ) {
			
			return type + Variables.VAR_TYPE_DELIMITER + num_value[0].ToString();
		}
		else
		if ( type == "string" ) {
			
			return type + Variables.VAR_TYPE_DELIMITER + string_value[0];
		} 

		return "";

	}

	public bool isNull () {

		if ( string_value != null && string_value.Count > 0 ) {

			if ( string_value[0] == "[null]" ) {

				return true;
			}
			else {

				return false;

			}

		}
		else {

			return false;
		}

	}

	public double getNumValue () {

		if ( num_value != null && num_value.Count > 0 ) {
			
			return num_value[0];
			
		}

		return 0f;

	}
	
}
