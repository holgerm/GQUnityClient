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
public class QuestConditionGrouper {

	[XmlElement("and")]
	public QuestConditionGrouper
		and;
	[XmlElement("or")]
	public QuestConditionGrouper
		or;
	[XmlElement("not")]
	public QuestConditionGrouper
		not;
	[XmlElement("lt")]
	public List<QuestConditionComparer>
		lt;
	[XmlElement("gt")]
	public List<QuestConditionComparer>
		gt;
	[XmlElement("leq")]
	public List<QuestConditionComparer>
		leq;
	[XmlElement("geq")]
	public List<QuestConditionComparer>
		geq;
	[XmlElement("eq")]
	public List<QuestConditionComparer>
		eq;

	public bool isfullfilled () {

		return isfullfilled("and");

	}

	public bool isfullfilled (string type) {

		List<bool> allbools = new List<bool>();

		if ( and != null ) {
			allbools.Add(and.isfullfilled("and"));
		}

		if ( or != null ) {
			allbools.Add(or.isfullfilled("or"));
		}

		if ( not != null ) {
			allbools.Add(not.isfullfilled("not"));
		}

		if ( eq != null ) {

			foreach ( QuestConditionComparer qcc in eq ) {

				allbools.Add(qcc.isFullfilled("eq"));

			}

		}

		if ( lt != null ) {
				
			foreach ( QuestConditionComparer qcc in lt ) {
					
				allbools.Add(qcc.isFullfilled("lt"));
					
			}

		}

		if ( gt != null ) {
				
			foreach ( QuestConditionComparer qcc in gt ) {
					
				allbools.Add(qcc.isFullfilled("gt"));
					
			}
				
		}

		if ( leq != null ) {
					
			foreach ( QuestConditionComparer qcc in leq ) {
						
				allbools.Add(qcc.isFullfilled("leq"));
						
			}
					
		}
				
		if ( geq != null ) {
					
			foreach ( QuestConditionComparer qcc in geq ) {
						
				allbools.Add(qcc.isFullfilled("geq"));
						
			}
					
		}
			
		if ( allbools.Count > 0 ) {

			if ( type == "and" ) {

				bool ands = true;

				foreach ( bool b in allbools ) {

					if ( !b ) {
						ands = false;
					}

				}

				return ands;

			}
			else
			if ( type == "or" ) {

				bool ors = false;
						
				foreach ( bool b in allbools ) {
							
					if ( b ) {
						ors = true;
					}
							
				}
						
				return ors;

			}
			else
			if ( type == "not" ) {
						
				bool nots = true;
						
				foreach ( bool b in allbools ) {
							
					if ( !b ) {
						nots = false;
					}
							
				}
						
				return nots;
						
			}

		} 

		return true;

	}
}
