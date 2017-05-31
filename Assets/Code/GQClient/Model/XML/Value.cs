using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;
using GQ.Client.Util;
using System;
using GQ.Client.Err;
using System.Text.RegularExpressions;
using System.Text;

namespace GQ.Client.Model.XML
{

	/// <summary>
	/// Value represents any possible value in Geoquest, like Boolean, String, Intergers and Float Numbers. 
	/// It is used as part of conditions or right hand side of assignmnet statements. 
	/// 
	/// This class is used for representing the XML model structure at runtime 
	/// but also for storing values in the variables registry.
	/// </summary>
	public class Value
	{
		#region Structure

		protected Type type;
		protected string internalValue;

		public Value (string valueAsText, Type type)
		{
			this.type = type;
			internalValue = valueAsText;
		}

		public enum Type
		{
			Bool,
			Text,
			Integer,
			Float,
			VariableName
		}


		#endregion


		#region Access Functions for different Types

		public bool asBool ()
		{
			bool result = false;

			try {
				if (type == Type.Bool || type == Type.Text) {
					result = Convert.ToBoolean (internalValue);
				}
				if (type == Type.Integer) {
					int asInt = Convert.ToInt32 (internalValue);
					result = Convert.ToBoolean (asInt);
				}
				if (type == Type.Float) {
					double asDouble = Convert.ToDouble (internalValue);
					result = Convert.ToBoolean (asDouble);
				}
				return result;
			} catch (FormatException) {
				result = false;
				Log.WarnAuthor ("Value {0} could ne be read as Bool so {1} was used instead.", internalValue, result);
				return result;
			} 

			if (type == Type.VariableName) {
				return Variables.getVariableValue (internalValue).asBool ();
			}

			result = false;
			Log.WarnDeveloper ("Unknown Value Type found when trying to read value {0} typed {1} as Bool so {2} was used instead.", internalValue, type, result);
			return result;
		}

		protected static Regex RegExpNumber = new Regex (@"^[\s0]*(?<Sign>-?)[\s0]*(?<NumPreComma>[1-9][0-9]*)(?<Comma>[.,]?)(?<NumPostComma>[0-9]*)");

		protected string extractNumberString (string originalString)
		{
			Match match = RegExpNumber.Match (originalString);
			StringBuilder numberStringBuilder = new StringBuilder ();
			if (match.Groups ["Sign"].Success)
				numberStringBuilder.Append (match.Groups ["Sign"].Value);
			
			numberStringBuilder.Append (match.Groups ["NumPreComma"].Value);

			if (match.Groups ["Comma"].Success && match.Groups ["NumPostComma"].Success) {
				numberStringBuilder.Append ("." + match.Groups ["NumPostComma"].Value);
			}
			return numberStringBuilder.ToString ();
		}

		public double asDouble ()
		{
			double result = 0d;

			try {
				if (type == Type.Bool || type == Type.Text) {
					result = Convert.ToDouble (internalValue);
				}
				if (type == Type.Integer || type == Type.Float) {
					result = Convert.ToDouble (extractNumberString (internalValue));
				}
				return result;
			} catch (OverflowException) {
				result = (internalValue.StartsWith ("-") ? Double.MinValue : Double.MaxValue);
				Log.WarnAuthor ("Tried to read value {0} as Double but value exceeded limits so {1} was used instead.", internalValue, result);
				return result;
			} catch (FormatException) {
				result = 0d;
				Log.WarnAuthor ("Value {0} could ne be read as Double so {1} was used instead.", internalValue, result);
				return result;
			} 

			if (type == Type.VariableName) {
				return Variables.getVariableValue (internalValue).asDouble ();
			}
				
			result = 0d;
			Log.WarnDeveloper ("Unknown Value Type found when trying to read value {0} typed {1} as Double so {2} was used instead.", internalValue, type, result);
			return result;
		}

		public int asInt ()
		{
			int result = 0;

			try {
				if (type == Type.Bool || type == Type.Text) {
					result = Convert.ToInt32 (internalValue);
				}
				if (type == Type.Integer || type == Type.Float) {
					result = Convert.ToInt32 (Convert.ToDouble (extractNumberString (internalValue)));
				}
				return result;
			} catch (OverflowException) {
				result = (internalValue.StartsWith ("-") ? Int32.MinValue : Int32.MaxValue);
				Log.WarnAuthor ("Tried to read value {0} as Int but value exceeded limits so {1} was used instead.", internalValue, result);
				return result;
			} catch (FormatException) {
				result = 0;
				Log.WarnAuthor ("Value {0} could ne be read as Int so {1} was used instead.", internalValue, result);
				return result;
			} 

			if (type == Type.VariableName) {
				return Variables.getVariableValue (internalValue).asInt ();
			}

			result = 0;
			Log.WarnDeveloper ("Unknown Value Type found when trying to read value {0} typed {1} as Int so {2} was used instead.", internalValue, type, result);
			return result;
		}

		public string asString ()
		{
			return internalValue;
		}

		public string asVariableName ()
		{
			string result;

			if (Variables.IsValidVariableName (internalValue))
				result = internalValue;
			else {
				try {
					result = Variables.LongestValidVariableNameFromStart (internalValue);
				} catch (ArgumentException) {
					result = "_undefined";
					Log.WarnAuthor ("Value {0} could ne be read as Variable Name so {1} was used instead.", internalValue, result);
				}
			}

			return result;
		}

		#endregion


		#region Comparison Functions

		public bool IsEqual (Value other)
		{
			Debug.Log ("IsEqual: " + this + " : " + other);
			if (this.type == Type.Bool) {
				if (other.type != Type.Bool) {
					Log.WarnAuthor ("You cannot compare Bool with " + other.type.ToString ());
					return false;
				} else
					return this.internalValue.Equals (other.internalValue);
			}

			if (this.type == Type.Text) {
				if (other.type != Type.Text) {
					Log.WarnAuthor ("You cannot compare Text with " + other.type.ToString ());
					return false;
				} else
					return this.internalValue.Equals (other.internalValue);
			}

			if (this.type == Type.Integer) {
				if (other.type == Type.Integer)
					return this.internalValue == other.internalValue;
				else if (other.type == Type.Float)
					return Values.NearlyEqual (this.asDouble (), other.asDouble ());
				else {
					Log.WarnAuthor ("You cannot compare Number with " + other.type.ToString ());
					return false;
				} 
			}

			if (this.type == Type.Float) {
				if (other.type == Type.Float) {
					return Values.NearlyEqual (this.asDouble (), other.asDouble ());
				} else if (other.type == Type.Integer) {
					return Values.NearlyEqual (this.asDouble (), other.asDouble ());
				} else {
					Log.WarnAuthor ("You cannot compare Number with " + other.type.ToString ());
					return false;
				} 
			}

			if (this.type == Type.VariableName) {
				Value thisValue = Variables.getVariableValue (this.internalValue);
				if (other.type == Type.VariableName) {
					Value otherValue = Variables.getVariableValue (other.internalValue);
					return thisValue.IsEqual (otherValue);
				} else {
					return thisValue.IsEqual (other);
				}
			}

			Log.WarnDeveloper ("Unknown Value Type found when checking if isEqual between " + type.ToString () + " and " + other.type.ToString ());
			return false;
		}

		public bool IsGreaterThan (Value other)
		{
			if (this.type == Type.Bool) {
				if (other.type != Type.Bool) {
					Log.WarnAuthor ("You cannot compare Bool with " + other.type.ToString ());
					return false;
				} else {
					// true > false is the only way this can be greater than other:
					return (this.asBool () && !other.asBool ());
				}
			}

			if (this.type == Type.Text) {
				if (other.type != Type.Text) {
					Log.WarnAuthor ("You cannot compare Text with " + other.type.ToString ());
					return false;
				} else
					return this.internalValue.CompareTo (other.internalValue) > 0;
			}

			if (this.type == Type.Integer) {
				if (other.type == Type.Integer)
					return this.asInt () > other.asInt ();
				else if (other.type == Type.Float)
					return Values.GreaterThan (this.asDouble (), other.asDouble ());
				else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.type, other.type);
					return false;
				}
			}

			if (this.type == Type.Float) {
				if (other.type == Type.Float || other.type == Type.Integer) {
					return Values.GreaterThan (this.asDouble (), other.asDouble ());
				} else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.type, other.type);
					return false;
				}
			}

			if (this.type == Type.VariableName) {
				Value thisValue = Variables.getVariableValue (this.internalValue);
				if (other.type == Type.VariableName) {
					Value otherValue = Variables.getVariableValue (other.internalValue);
					return thisValue.IsGreaterThan (otherValue);
				} else {
					return thisValue.IsGreaterThan (other);
				}
			}

			Log.WarnDeveloper ("Unknown Value Type found when checking if IsGreaterThan between " + this.type + " and " + other.type);
			return false;
		}

		public bool IsGreaterOrEqual (Value other)
		{
			if (this.type == Type.Bool) {
				if (other.type != Type.Bool) {
					Log.WarnAuthor ("You cannot compare Bool with " + other.type.ToString ());
					return false;
				} else {
					// true > false is the only way this can be greater than other:
					return (this.asBool () || (!this.asBool () && !other.asBool ()));
				}
			}

			if (this.type == Type.Text) {
				if (other.type != Type.Text) {
					Log.WarnAuthor ("You cannot compare Text with " + other.type.ToString ());
					return false;
				} else {
					Debug.Log ("COMPARE Strings: " + internalValue + " : " + other.internalValue + " => " + this.internalValue.CompareTo (other.internalValue));
					return this.internalValue.CompareTo (other.internalValue) >= 0;
				}
			}

			if (this.type == Type.Integer) {
				if (other.type == Type.Integer)
					return this.asInt () >= other.asInt ();
				else if (other.type == Type.Float)
					return Values.GreaterThan (this.asDouble (), other.asDouble ()) || Values.NearlyEqual (this.asDouble (), other.asDouble ());
				else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.type, other.type);
					return false;
				}
			}

			if (this.type == Type.Float) {
				if (other.type == Type.Float || other.type == Type.Integer) {
					return Values.GreaterThan (this.asDouble (), other.asDouble ()) || Values.NearlyEqual (this.asDouble (), other.asDouble ());
				} else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.type, other.type);
					return false;
				}
			}

			if (this.type == Type.VariableName) {
				Value thisValue = Variables.getVariableValue (this.internalValue);
				if (other.type == Type.VariableName) {
					Value otherValue = Variables.getVariableValue (other.internalValue);
					return thisValue.IsGreaterOrEqual (otherValue);
				} else {
					return thisValue.IsGreaterOrEqual (other);
				}
			}

			Log.WarnDeveloper ("Unknown Value Type found when checking if IsGreaterOrEqual between " + this.type + " and " + other.type);
			return false;
		}

		public bool IsLessThan (Value other)
		{
			if (this.type == Type.Bool) {
				if (other.type != Type.Bool) {
					Log.WarnAuthor ("You cannot compare Bool with " + other.type.ToString ());
					return false;
				} else {
					// false < true is the only way this can be greater than other:
					return (!this.asBool () && other.asBool ());
				}
			}

			if (this.type == Type.Text) {
				if (other.type != Type.Text) {
					Log.WarnAuthor ("You cannot compare Text with " + other.type.ToString ());
					return false;
				} else
					return this.internalValue.CompareTo (other.internalValue) < 0;
			}

			if (this.type == Type.Integer) {
				if (other.type == Type.Integer)
					return this.asInt () < other.asInt ();
				else if (other.type == Type.Float)
					return Values.GreaterThan (other.asDouble (), this.asDouble ());
				else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.type, other.type);
					return false;
				}
			}

			if (this.type == Type.Float) {
				if (other.type == Type.Float || other.type == Type.Integer) {
					return Values.GreaterThan (other.asDouble (), this.asDouble ());
				} else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.type, other.type);
					return false;
				}
			}

			if (this.type == Type.VariableName) {
				Value thisValue = Variables.getVariableValue (this.internalValue);
				if (other.type == Type.VariableName) {
					Value otherValue = Variables.getVariableValue (other.internalValue);
					return thisValue.IsLessThan (otherValue);
				} else {
					return thisValue.IsLessThan (other);
				}
			}

			Log.WarnDeveloper ("Unknown Value Type found when checking if IsLessThan between " + this.type + " and " + other.type);
			return false;
		}

		public bool IsLessOrEqual (Value other)
		{
			if (this.type == Type.Bool) {
				if (other.type != Type.Bool) {
					Log.WarnAuthor ("You cannot compare Bool with " + other.type.ToString ());
					return false;
				} else {
					// false < true is the only way this can be greater than other:
					return (!this.asBool () || (this.asBool () && other.asBool ()));
				}
			}

			if (this.type == Type.Text) {
				if (other.type != Type.Text) {
					Log.WarnAuthor ("You cannot compare Text with " + other.type.ToString ());
					return false;
				} else
					return this.internalValue.CompareTo (other.internalValue) <= 0;
			}

			if (this.type == Type.Integer) {
				if (other.type == Type.Integer)
					return this.asInt () <= other.asInt ();
				else if (other.type == Type.Float)
					return Values.GreaterThan (other.asDouble (), this.asDouble ()) || Values.NearlyEqual (this.asDouble (), other.asDouble ());
				else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.type, other.type);
					return false;
				}
			}

			if (this.type == Type.Float) {
				if (other.type == Type.Float || other.type == Type.Integer) {
					return Values.GreaterThan (other.asDouble (), this.asDouble ()) || Values.NearlyEqual (this.asDouble (), other.asDouble ());
				} else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.type, other.type);
					return false;
				}
			}

			if (this.type == Type.VariableName) {
				Value thisValue = Variables.getVariableValue (this.internalValue);
				if (other.type == Type.VariableName) {
					Value otherValue = Variables.getVariableValue (other.internalValue);
					return thisValue.IsLessOrEqual (otherValue);
				} else {
					return thisValue.IsLessOrEqual (other);
				}
			}

			Log.WarnDeveloper ("Unknown Value Type found when checking if IsLessOrEqual between " + this.type + " and " + other.type);
			return false;
		}

		#endregion

		#region Util Functions

		public override string ToString ()
		{
			return String.Format ("{0} ({1})", internalValue, type);
		}

		#endregion

	}
}