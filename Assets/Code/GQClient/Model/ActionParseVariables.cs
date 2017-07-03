using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Xml.Serialization;
using GQ.Client.Err;
using System.Xml;
using System;

namespace GQ.Client.Model
{

	public class ActionParseVariables : ActionAbstract
	{
		#region Structure

		protected string fromVarName = null;

		public string FromVarName {
			get {
				return fromVarName;
			}
			protected set {
				fromVarName = value;
			}
		}

		protected override void ReadAttributes (XmlReader reader)
		{
			base.ReadAttributes (reader);

			FromVarName = GQML.GetStringAttribute (GQML.ACTION_ATTRIBUTE_FROMVARNAME, reader);
		}

		#endregion


		#region Functions

		public override void Execute ()
		{
			if (fromVarName == null) {
				Log.SignalErrorToDeveloper ("ParseVariableAction Action without FromVar name can not be executed. (Ignored)");
				return;
			}

			string originalText = Variables.GetValue (fromVarName).AsString ();

			const char DELIMITER = ',';

			char[] receivedChars = originalText.ToCharArray ();

			int curIndex = 0;
			char key = '\0';
			System.Text.StringBuilder valueBuilder;

			while (curIndex <= receivedChars.Length - 2) {
				// index must leave a rest of at least two chars: the key and the ':'

				// parse key:
				key = receivedChars [curIndex];
				curIndex += 2; // skip the key char and the ':'

				// parse value:
				valueBuilder = new System.Text.StringBuilder ();

				while (curIndex <= receivedChars.Length - 1) {

					if (receivedChars [curIndex] != DELIMITER) {
						// ordinary content:
						valueBuilder.Append (receivedChars [curIndex]);
						curIndex++;
						continue; // do NOT store key-value but proceed to gather the value
					}

					if (curIndex == receivedChars.Length - 1) {
						// the current is a ',' and the last in the array we interpret it as an empty value
						// e.g. [i:,] => id = ""
						// we proceed one char and do not have to do anything since the while loop will terminate
						curIndex++;
					} else {
						// now we look at the char after the first ',':
						curIndex++;

						if (receivedChars [curIndex] == DELIMITER) {
							// we found a double ',,' which is just a ',' within the content
							// hence we add just one ',' ignore the second and go on parsing the value further
							// e.g. [p:me,, you and him] -> payload = "me, you and him"
							valueBuilder.Append (receivedChars [curIndex]);
							curIndex++; // remember this is the second increase!
							continue; // ready to step one char further
						} else {
							// we found a single ',' which signifies the end of the value
							// we keep the index pointing at the next key and finish with this value
							// e.g. [i:123,p:hello] -> id = "123"; payload = "hello"
							break; // leaving value gathering and go on with next key-value pair
						}
					}
				}
				// end of KV pair reached: store it
				// if value can be parsed as number we store it as number typed var:
				Variables.SetVariableValue (
					key.ToString (), 
					Value.CreateValueFromRawString (valueBuilder.ToString ())
				);
			}
		}

		#endregion
	}
}
