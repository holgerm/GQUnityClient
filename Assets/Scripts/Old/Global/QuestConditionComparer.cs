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
using GQ.Util;
using UnitySlippyMap;

[System.Serializable]
public class QuestConditionComparer {

	[XmlElement("string")]
	public List<string>
		string_value;
	[XmlElement("num")]
	public List<double>
		num_value;
	[XmlElement("boolean")]
	public List<bool>
		bool_value;
	[XmlElement("var")]
	public List<string>
		var_value;

	public bool isFullfilled (string type) {

		if ( type == "eq" ) {

			if ( stringcomponents().Count > 1 ) {
				bool equals = true;
				string last = null;
				foreach ( string current in stringcomponents() ) {

					if ( last == null ) {
						last = current;
					}
					else {

						if ( current != last ) {

							equals = false;

						}

					}
				}
				return equals;
			}
			else {
				return false;
			}
		}
		else
		if ( type == "lt" ) {

			if ( intcomponents().Count > 1 ) {

				bool lessthan = true;
				int last = int.MinValue;

				foreach ( int i in intcomponents() ) {

					if ( last >= i ) {
						lessthan = false;
					}

					last = i;

				}

				return lessthan;

			}
			else {

				return false;

			}

		}
		else
		if ( type == "leq" ) {
					
			if ( intcomponents().Count > 1 ) {
						
				bool lessthan = true;
				int last = int.MinValue;
						
				foreach ( int i in intcomponents() ) {
							
					if ( last > i ) {
						lessthan = false;
					}
							
					last = i;
							
				}
						
				return lessthan;
				
			}
			else {
				
				return false;
				
			}

		}
		else
		if ( type == "gt" ) {

			if ( intcomponents().Count > 1 ) {
							
				bool greaterthan = true;
				int last = int.MaxValue;
							
				foreach ( int i in intcomponents() ) {
									
					if ( last <= i ) {
						greaterthan = false;
					}
								
					last = i;
								
				}
							
				return greaterthan;
							
			}
			else {
							
				return false;
							
			}

		}
		else
		if ( type == "geq" ) {
			
			if ( intcomponents().Count > 1 ) {
				
				bool greaterthan = true;
				int last = int.MaxValue;
				
				foreach ( int i in intcomponents() ) {
					
					if ( last < i ) {
						greaterthan = false;
					}
					
					last = i;
					
				}
				
				return greaterthan;
				
			}
			else {
				
				return false;
				
			}
				
		}
		else {

			return false;

		}
	
	}

	public List<double> intcomponents () {

		List<double> comp = new List<double>();

		if ( var_value != null ) {

			foreach ( string s in var_value ) {

				if ( !s.Contains("+") && !s.Contains("-") && !s.Contains("*") && !s.Contains("/") && !s.Contains(":") ) {

					QuestVariable qv = GameObject.Find("QuestDatabase").GetComponent<actions>().getVariable(s);
					
					if ( qv != null ) {
						if ( qv.num_value != null && qv.num_value.Count > 0 ) {
							comp.Add(qv.num_value[0]);
						}
					}
					
				}
				else {
					
					double ergebnis = GameObject.Find("QuestDatabase").GetComponent<actions>().mathVariable(s);
					Debug.Log("IF MATH" + ergebnis);
					comp.Add(ergebnis);
					
				}

			}

		} 

		if ( num_value != null ) {

			comp.AddRange(num_value);
		} 

		return comp;
	}

	public List<string> stringcomponents () {
		
		List<string> comp = new List<string>();

		if ( string_value != null ) {
			comp.AddRange(string_value);
		}
		if ( num_value != null ) {
			foreach ( float n in num_value ) {
				comp.Add("" + n);
			}
		}
		if ( bool_value != null ) {

			foreach ( bool b in bool_value ) {
					
				if ( b ) {
					comp.Add("true"); 
				}
				else {
					comp.Add("false");
				}

			}
		}
		if ( var_value != null && var_value.Count > 0 ) {

//			Debug.Log ("looking for var values");

			foreach ( string k in var_value ) {

				//	Debug.Log ("looking vor var " + k);

				string kk = new string(k.ToCharArray()
				                       .Where(c => !Char.IsWhiteSpace(c))
				                       .ToArray());

				//Debug.Log("-----starting to look for '"+kk+"'");

				if ( !kk.Contains("+") && !kk.Contains("-") && !kk.Contains("*") && !kk.Contains("/") && !kk.Contains(":") ) {
					QuestVariable qv = GameObject.Find("QuestDatabase").GetComponent<actions>().getVariable(kk);

					if ( qv != null ) {

						//Debug.Log("found");
						if ( qv.getStringValue() != null ) {
							comp.Add(qv.getStringValue());
						}

					}
					else {

						Debug.Log("couldn't find var " + kk);

					}

				}
				else {

					double ergebnis = GameObject.Find("QuestDatabase").GetComponent<actions>().mathVariable(kk);
					Debug.Log("IF MATH" + ergebnis);
					comp.Add(ergebnis.ToString());
					
				}

			}

		}
		
		return comp;
		
	}

	public double mathVariable (string input) {
		
		double currentvalue = 0.0d;
		bool needsstartvalue = true;
		input = new string(input.ToCharArray()
		                    .Where(c => !Char.IsWhiteSpace(c))
		                    .ToArray());
		//	Debug.Log ("Rechnung:"+input);
		
		string arithmetics = "";
		
		foreach ( Char c in input.ToCharArray() ) {
			
			if ( c == '+' ) {
				
				arithmetics = arithmetics + "+";
			}
			if ( c == '-' ) {
				
				arithmetics = arithmetics + "-";
			}
			if ( c == '*' ) {
				
				arithmetics = arithmetics + "*";
			}
			if ( c == '/' ) {
				
				arithmetics = arithmetics + "/";
			}
			if ( c == ':' ) {
				
				arithmetics = arithmetics + ":";
			}
			
		}
		
		//Debug.Log ("Rechnung:"+arithmetics);
		
		char[] splitter = "+-/*:".ToCharArray();
		string[] splitted = input.Split(splitter);
		
		int count = 0;
		
		foreach ( string s in splitted ) {
			
			double n;
			bool isNumeric = double.TryParse(s, out n);
			if ( isNumeric ) {
				
				if ( needsstartvalue ) {
					
					currentvalue = n;
					needsstartvalue = false;
				}
				else {
					
					if ( arithmetics.Substring(count, 1) == "+" ) {
						currentvalue += n;
					}
					else
					if ( arithmetics.Substring(count, 1) == "-" ) {
						currentvalue -= n;
					}
					else
					if ( arithmetics.Substring(count, 1) == "*" ) {
						currentvalue *= n;
					}
					else
					if ( (arithmetics.Substring(count, 1) == "/") || (arithmetics.Substring(count, 1) == ":") ) {
						
						currentvalue = currentvalue / n;
					} 
					
				}
				
			}
			else {
				
				QuestVariable qv = GameObject.Find("QuestDatabase").GetComponent<actions>().getVariable(s);
				if ( !qv.isNull() ) {
					if ( qv.num_value != null && qv.num_value.Count > 0 ) {
						if ( needsstartvalue ) {
							
							currentvalue = qv.num_value[0];
							Debug.Log(s + ":" + currentvalue.ToString("F10"));
							
							needsstartvalue = false;
							
						}
						else {
							
							n = qv.num_value[0];
							
							Debug.Log(n);
							if ( arithmetics.Substring(count, 1) == "+" ) {
								currentvalue += n;
							}
							else
							if ( arithmetics.Substring(count, 1) == "-" ) {
								currentvalue -= n;
								//								Debug.Log(currentvalue);
							}
							else
							if ( arithmetics.Substring(count, 1) == "*" ) {
								currentvalue *= n;
							}
							else
							if ( (arithmetics.Substring(count, 1) == "/") || (arithmetics.Substring(count, 1) == ":") ) {
								
								currentvalue = currentvalue / n;
							}
							count += 1;
							
						}
						
					}
				}
			}
			
		}
		
		return currentvalue;
		
	}

}
