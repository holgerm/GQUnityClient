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

namespace GQ.Client.Model
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

		public Type ValType {
			get;
			protected set;
		}

		protected string internalValue;

		public Value (string valueAsText, Type type)
		{
			this.ValType = type;
			internalValue = valueAsText;
		}

		/// <summary>
		/// Determines whether the specified <see cref="GQ.Client.Model.Value"/> is equal to the current <see cref="GQ.Client.Model.Value"/>.
		/// </summary>
		/// <param name="other">The <see cref="GQ.Client.Model.Value"/> to compare with the current <see cref="GQ.Client.Model.Value"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="GQ.Client.Model.Value"/> is equal to the current
		/// <see cref="GQ.Client.Model.Value"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals (System.Object other)
		{
			var otherValue = other as Value;

			if (otherValue == null)
				return false;
			
			return (
			    this.ValType == otherValue.ValType
			    && this.internalValue.Equals (otherValue.internalValue)
			);
		}

		public override int GetHashCode ()
		{            
			return base.GetHashCode ();        
		}

		public enum Type
		{
			Bool,
			Text,
			Integer,
			Float,
			VariableName,
			NULL
		}

		public static readonly Value Null = new NullValue ();

		private class NullValue : Value
		{

			public NullValue ()
				: base ("", Type.NULL)
			{

			}

			public override string ToString ()
			{
				return "NULL Value";
			}
		}

		#endregion


		#region Convenience Constructors

		/// <summary>
		/// Creates a Bool typed value object.
		/// </summary>
		public Value (bool b) : this (Convert.ToString (b), Type.Bool)
		{
		}

		/// <summary>
		/// Creates an Integer typed value object.
		/// </summary>
		public Value (int i) : this (Convert.ToString (i), Type.Integer)
		{
		}

		/// <summary>
		/// Creates a Float (double) typed value object.
		/// </summary>
		public Value (double i) : this (Convert.ToString (i), Type.Float)
		{
		}

		/// <summary>
		/// Creates a Text typed value object.
		/// </summary>
		/// <param name="s">S.</param>
		public Value (string s) : this (s, Type.Text)
		{
		}

		/// <summary>
		/// Creates the value from raw string by interpreting it as int, float, bool or just text.
		/// </summary>
		/// <returns>The value from raw string.</returns>
		/// <param name="rawString">Raw string.</param>
		public static Value CreateValueFromRawString (string rawString)
		{
			int intValue;
			if (Int32.TryParse (rawString, out intValue)) {
				return new Value (intValue);
			}
			double doubleValue;
			string extractedNumberString = extractNumberString (rawString);
			if (extractedNumberString.Length == rawString.Length && Double.TryParse (extractedNumberString, out doubleValue)) {
				return new Value (doubleValue);
			}
			bool boolValue;
			if (Boolean.TryParse (rawString, out boolValue)) {
				return new Value (boolValue);
			} 
			return new Value (rawString, Value.Type.Text);
		}

		#endregion


		#region Access Functions for different Types

		public bool AsBool ()
		{
			bool result = false;

			try {
				if (ValType == Type.Bool || ValType == Type.Text) {
					result = Convert.ToBoolean (internalValue);
				} else if (ValType == Type.Integer) {
					int asInt = Convert.ToInt32 (internalValue);
					result = Convert.ToBoolean (asInt);
				} else if (ValType == Type.Float) {
					double asDouble = Convert.ToDouble (internalValue);
					result = Convert.ToBoolean (asDouble);
				} else if (ValType == Type.VariableName) {
					result = Variables.GetValue (internalValue).AsBool ();
				} else {
					result = false;
					Log.WarnDeveloper ("Unknown Value Type found when trying to read value {0} typed {1} as Bool so {2} was used instead.", internalValue, ValType, result);
				}
				return result;
			} catch (FormatException) {
				result = false;
				Log.WarnAuthor ("Value {0} could not be read as Bool so {1} was used instead.", internalValue, result);
				return result;
			} 

		}

		protected static Regex RegExpNumber = new Regex (@"^(?<Sign>-?)[\s]*(?<NumPreComma>[0-9]*)(?<Comma>[.,]?)(?<NumPostComma>[0-9]*)");

		protected static string extractNumberString (string originalString)
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

		public double AsDouble ()
		{
			double result;

			try {
				if (ValType == Type.Bool || ValType == Type.Text) {
					return Convert.ToDouble (internalValue);
				}
				if (ValType == Type.Integer || ValType == Type.Float) {
					return Convert.ToDouble (extractNumberString (internalValue));
				}
			} catch (OverflowException) {
				result = (internalValue.StartsWith ("-") ? Double.MinValue : Double.MaxValue);
				Log.WarnAuthor ("Tried to read value {0} as Double but value exceeded limits so {1} was used instead.", internalValue, result);
				return result;
			} catch (FormatException) {
				result = 0d;
				Log.WarnAuthor ("Value {0} could ne be read as Double so {1} was used instead.", internalValue, result);
				return result;
			} 

			if (ValType == Type.VariableName) {
				return Variables.GetValue (internalValue).AsDouble ();
			}
				
			result = 0d;
			Log.WarnDeveloper ("Unknown Value Type found when trying to read value {0} typed {1} as Double so {2} was used instead.", internalValue, ValType, result);
			return result;
		}

		public int AsInt ()
		{
			int result = 0;

			try {
				if (ValType == Type.Bool || ValType == Type.Text) {
					return Convert.ToInt32 (internalValue);
				} 
				if (ValType == Type.Integer || ValType == Type.Float) {
					return Convert.ToInt32 (Convert.ToDouble (extractNumberString (internalValue)));
				} 
				if (ValType == Type.VariableName) {
					return Variables.GetValue (internalValue).AsInt ();
				} 

				// else:
				Log.WarnDeveloper ("Unknown Value Type found when trying to read value {0} typed {1} as Int so {2} was used instead.", internalValue, ValType, result);
				return 0;
			} catch (OverflowException) {
				result = (internalValue.StartsWith ("-") ? Int32.MinValue : Int32.MaxValue);
				Log.WarnAuthor ("Tried to read value {0} as Int but value exceeded limits so {1} was used instead.", internalValue, result);
				return result;
			} catch (FormatException) {
				result = 0;
				Log.WarnAuthor ("Value {0} could ne be read as Int so {1} was used instead.", internalValue, result);
				return result;
			} 
		}

		public string AsString ()
		{
			return internalValue;
		}

		public string AsVariableName ()
		{
			string result;

			if (Variables.IsValidUserDefinedVariableName (internalValue))
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
			if (this.ValType == Type.Bool) {
				if (other.ValType != Type.Bool) {
					Log.WarnAuthor ("You cannot compare Bool with " + other.ValType.ToString ());
					return false;
				} else
					return this.internalValue.Equals (other.internalValue);
			}

			if (this.ValType == Type.Text) {
				if (other.ValType != Type.Text) {
					Log.WarnAuthor ("You cannot compare Text with " + other.ValType.ToString ());
					return false;
				} else
					return this.internalValue.Equals (other.internalValue);
			}

			if (this.ValType == Type.Integer) {
				if (other.ValType == Type.Integer)
					return this.internalValue == other.internalValue;
				else if (other.ValType == Type.Float)
					return Values.NearlyEqual (this.AsDouble (), other.AsDouble ());
				else {
					Log.WarnAuthor ("You cannot compare Number with " + other.ValType.ToString ());
					return false;
				} 
			}

			if (this.ValType == Type.Float) {
				if (other.ValType == Type.Float) {
					return Values.NearlyEqual (this.AsDouble (), other.AsDouble ());
				} else if (other.ValType == Type.Integer) {
					return Values.NearlyEqual (this.AsDouble (), other.AsDouble ());
				} else {
					Log.WarnAuthor ("You cannot compare Number with " + other.ValType.ToString ());
					return false;
				} 
			}

			if (this.ValType == Type.VariableName) {
				Value thisValue = Variables.GetValue (this.internalValue);
				if (other.ValType == Type.VariableName) {
					Value otherValue = Variables.GetValue (other.internalValue);
					return thisValue.IsEqual (otherValue);
				} else {
					return thisValue.IsEqual (other);
				}
			}

			Log.WarnDeveloper ("Unknown Value Type found when checking if isEqual between " + ValType.ToString () + " and " + other.ValType.ToString ());
			return false;
		}

		public bool IsGreaterThan (Value other)
		{
			if (this.ValType == Type.Bool) {
				if (other.ValType != Type.Bool) {
					Log.WarnAuthor ("You cannot compare Bool with " + other.ValType.ToString ());
					return false;
				} else {
					// true > false is the only way this can be greater than other:
					return (this.AsBool () && !other.AsBool ());
				}
			}

			if (this.ValType == Type.Text) {
				if (other.ValType != Type.Text) {
					Log.WarnAuthor ("You cannot compare Text with " + other.ValType.ToString ());
					return false;
				} else
					return this.internalValue.CompareTo (other.internalValue) > 0;
			}

			if (this.ValType == Type.Integer) {
				if (other.ValType == Type.Integer)
					return this.AsInt () > other.AsInt ();
				else if (other.ValType == Type.Float)
					return Values.GreaterThan (this.AsDouble (), other.AsDouble ());
				else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.ValType, other.ValType);
					return false;
				}
			}

			if (this.ValType == Type.Float) {
				if (other.ValType == Type.Float || other.ValType == Type.Integer) {
					return Values.GreaterThan (this.AsDouble (), other.AsDouble ());
				} else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.ValType, other.ValType);
					return false;
				}
			}

			if (this.ValType == Type.VariableName) {
				Value thisValue = Variables.GetValue (this.internalValue);
				if (other.ValType == Type.VariableName) {
					Value otherValue = Variables.GetValue (other.internalValue);
					return thisValue.IsGreaterThan (otherValue);
				} else {
					return thisValue.IsGreaterThan (other);
				}
			}

			Log.WarnDeveloper ("Unknown Value Type found when checking if IsGreaterThan between " + this.ValType + " and " + other.ValType);
			return false;
		}

		public bool IsGreaterOrEqual (Value other)
		{
			if (this.ValType == Type.Bool) {
				if (other.ValType != Type.Bool) {
					Log.WarnAuthor ("You cannot compare Bool with " + other.ValType.ToString ());
					return false;
				} else {
					// true > false is the only way this can be greater than other:
					return (this.AsBool () || (!this.AsBool () && !other.AsBool ()));
				}
			}

			if (this.ValType == Type.Text) {
				if (other.ValType != Type.Text) {
					Log.WarnAuthor ("You cannot compare Text with " + other.ValType.ToString ());
					return false;
				} else {
					return this.internalValue.CompareTo (other.internalValue) >= 0;
				}
			}

			if (this.ValType == Type.Integer) {
				if (other.ValType == Type.Integer)
					return this.AsInt () >= other.AsInt ();
				else if (other.ValType == Type.Float)
					return Values.GreaterThan (this.AsDouble (), other.AsDouble ()) || Values.NearlyEqual (this.AsDouble (), other.AsDouble ());
				else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.ValType, other.ValType);
					return false;
				}
			}

			if (this.ValType == Type.Float) {
				if (other.ValType == Type.Float || other.ValType == Type.Integer) {
					return Values.GreaterThan (this.AsDouble (), other.AsDouble ()) || Values.NearlyEqual (this.AsDouble (), other.AsDouble ());
				} else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.ValType, other.ValType);
					return false;
				}
			}

			if (this.ValType == Type.VariableName) {
				Value thisValue = Variables.GetValue (this.internalValue);
				if (other.ValType == Type.VariableName) {
					Value otherValue = Variables.GetValue (other.internalValue);
					return thisValue.IsGreaterOrEqual (otherValue);
				} else {
					return thisValue.IsGreaterOrEqual (other);
				}
			}

			Log.WarnDeveloper ("Unknown Value Type found when checking if IsGreaterOrEqual between " + this.ValType + " and " + other.ValType);
			return false;
		}

		public bool IsLessThan (Value other)
		{
			if (this.ValType == Type.Bool) {
				if (other.ValType != Type.Bool) {
					Log.WarnAuthor ("You cannot compare Bool with " + other.ValType.ToString ());
					return false;
				} else {
					// false < true is the only way this can be greater than other:
					return (!this.AsBool () && other.AsBool ());
				}
			}

			if (this.ValType == Type.Text) {
				if (other.ValType != Type.Text) {
					Log.WarnAuthor ("You cannot compare Text with " + other.ValType.ToString ());
					return false;
				} else
					return this.internalValue.CompareTo (other.internalValue) < 0;
			}

			if (this.ValType == Type.Integer) {
				if (other.ValType == Type.Integer)
					return this.AsInt () < other.AsInt ();
				else if (other.ValType == Type.Float)
					return Values.GreaterThan (other.AsDouble (), this.AsDouble ());
				else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.ValType, other.ValType);
					return false;
				}
			}

			if (this.ValType == Type.Float) {
				if (other.ValType == Type.Float || other.ValType == Type.Integer) {
					return Values.GreaterThan (other.AsDouble (), this.AsDouble ());
				} else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.ValType, other.ValType);
					return false;
				}
			}

			if (this.ValType == Type.VariableName) {
				Value thisValue = Variables.GetValue (this.internalValue);
				if (other.ValType == Type.VariableName) {
					Value otherValue = Variables.GetValue (other.internalValue);
					return thisValue.IsLessThan (otherValue);
				} else {
					return thisValue.IsLessThan (other);
				}
			}

			Log.WarnDeveloper ("Unknown Value Type found when checking if IsLessThan between " + this.ValType + " and " + other.ValType);
			return false;
		}

		public bool IsLessOrEqual (Value other)
		{
			if (this.ValType == Type.Bool) {
				if (other.ValType != Type.Bool) {
					Log.WarnAuthor ("You cannot compare Bool with " + other.ValType.ToString ());
					return false;
				} else {
					// false < true is the only way this can be greater than other:
					return (!this.AsBool () || (this.AsBool () && other.AsBool ()));
				}
			}

			if (this.ValType == Type.Text) {
				if (other.ValType != Type.Text) {
					Log.WarnAuthor ("You cannot compare Text with " + other.ValType.ToString ());
					return false;
				} else
					return this.internalValue.CompareTo (other.internalValue) <= 0;
			}

			if (this.ValType == Type.Integer) {
				if (other.ValType == Type.Integer)
					return this.AsInt () <= other.AsInt ();
				else if (other.ValType == Type.Float)
					return Values.GreaterThan (other.AsDouble (), this.AsDouble ()) || Values.NearlyEqual (this.AsDouble (), other.AsDouble ());
				else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.ValType, other.ValType);
					return false;
				}
			}

			if (this.ValType == Type.Float) {
				if (other.ValType == Type.Float || other.ValType == Type.Integer) {
					return Values.GreaterThan (other.AsDouble (), this.AsDouble ()) || Values.NearlyEqual (this.AsDouble (), other.AsDouble ());
				} else {
					Log.WarnAuthor ("You cannot compare values of type {0} with values of type {1}.", this.ValType, other.ValType);
					return false;
				}
			}

			if (this.ValType == Type.VariableName) {
				Value thisValue = Variables.GetValue (this.internalValue);
				if (other.ValType == Type.VariableName) {
					Value otherValue = Variables.GetValue (other.internalValue);
					return thisValue.IsLessOrEqual (otherValue);
				} else {
					return thisValue.IsLessOrEqual (other);
				}
			}

			Log.WarnDeveloper ("Unknown Value Type found when checking if IsLessOrEqual between " + this.ValType + " and " + other.ValType);
			return false;
		}

		#endregion

		#region Util Functions

		public override string ToString ()
		{
			return String.Format ("{0} ({1})", internalValue, ValType);
		}

		#endregion

	}
}