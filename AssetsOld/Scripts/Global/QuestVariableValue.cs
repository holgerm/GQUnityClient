using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System;
using System.Linq;
using System.Text;
using GQ.Geo;
using GQ.Client.Util;
using UnitySlippyMap;

[System.Serializable]
public class QuestVariableValue
{

	[XmlElement ("string")]
	public List<string>
		string_value;
	[XmlElement ("num")]
	public List<double>
		num_value;
	[XmlElement ("bool")]
	public List<bool>
		bool_value;
	[XmlElement ("var")]
	public List<string>
		var_value;
	public List<double>
		double_value;
	
}
