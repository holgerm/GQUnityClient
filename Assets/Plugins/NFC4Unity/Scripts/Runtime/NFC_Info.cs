using UnityEngine;
using System.Collections;
using System;
using System.Text;


namespace QM.NFC
{

	/// <summary>
	/// NFC_Info represents the data stored on an NFC chip.
	/// 
	/// In order to be transmitted between Java and C# the data is mashalled using this format:
	/// 
	/// key:value,key:value,key:value etc.
	/// 
	/// Where key is a char that can be i for id, p for payload, t for techlist.
	/// And value is the original content from the NFC chip, except for occurrences of the DELIMITER character which are doubled. 
	/// I.e. every occurrence of ',' in the content has been replaced by ',,'. 
	/// Therefore the unmarshall method searches for double DELIMITERs and replaces them by one single DELIMITER again. 
	/// Occurrences of single DELIMITER in the marshalled content will in contrast be interpreted as the end of the 
	/// currently parsed content and ignored but trigger the switch to searching for the next key.
	/// 
	/// NFC_Info is an immutable object.
	/// </summary>
	public class NFC_Info
	{
		private const char DELIMITER = ',';
		private const char KEY_VALUE_DELIMITER = ':';
		private const char KEY_ID = 'i';
		private const char KEY_PAYLOAD = 'p';
		private const char KEY_TECHS = 't';

		private string _id;

		/// <summary>
		/// The Identifier of the NFC chip. In order to support usage in Dictionaries, this is an immutable field.
		/// </summary>
		/// <value>The identifier.</value>
		public string Id {
			get {
				return _id;
			}
			private set {
				if (_id == null)
					_id = value;
			}
		}

		private string _payload;

		/// <summary>
		/// The payload (aka content) of the NFC chip. In order to support usage in Dictionaries, this is an immutable field.
		/// </summary>
		/// <value>The payload.</value>
		public string Payload {
			get {
				return _payload;
			}
			private set {
				if (_payload == null)
					_payload = (value == null) ? "" : value;
			}
		}

		private string[] _techs;

		/// <summary>
		/// The tech descriptions of the NFC chip. In order to support usage in Dictionaries, this is an immutable field.
		/// </summary>
		/// <value>The tech descriptions.</value>
		public string[] Techs {
			get {
				return _techs;
			}
			private set {
				if (_techs == null)
					_techs = value;
			}
		}

		private bool _valid;

		public bool Valid {
			get {
				_valid &= (Id != null && Id.Length > 0);
				_valid &= Payload != null;
				return _valid;
			}
			private set {
				_valid = value;
			}
		}

		public static bool operator == (NFC_Info info1, NFC_Info info2)
		{
			return ReferenceEquals (info1, info2);
		}

		public static bool operator != (NFC_Info info1, NFC_Info info2)
		{
			return !ReferenceEquals (info1, info2);
		}

		public override bool Equals (System.Object other)
		{
			bool equal = true;

			equal &= other != null;

			NFC_Info otherInfo = other as NFC_Info;

			equal &= otherInfo != null;
			equal &= this.Id == otherInfo.Id;
			equal &= this.Payload == otherInfo.Payload;
			if (this.Techs == null)
				equal &= otherInfo.Techs == null;
			else
				for (int i = 0; equal && i < Techs.Length; i++)
					equal &= this.Techs [i].Equals (otherInfo.Techs [i]);

			return equal;
		}

		public override int GetHashCode ()
		{
			unchecked { // Overflow is fine, just wrap
				int hash = 17;
				if (Id != null)
					hash = hash * 23 + Id.GetHashCode ();
				if (Id != null)
					hash = hash * 23 + Payload.GetHashCode ();
				if (Id != null)
					hash = hash * 23 + Techs.GetHashCode ();
				return hash;
			}
		}

		public NFC_Info (string receivedString)
		{
			Id = null;
			Payload = null;
			Techs = null;
			Valid = true;

			unmarshall (receivedString);
		}

		public NFC_Info (string id, string payload, string[] techs = null)
		{
			Id = id;
			Payload = payload;
			Techs = techs;
			// TODO Valid ?
		}

		public string marshall ()
		{
			StringBuilder details = new StringBuilder ();

			// id:
			if (Id != null && !Id.Equals ("")) {
				details.Append ("" + KEY_ID + KEY_VALUE_DELIMITER + maskCommas (Id));
			}
			// payload:
			if (Payload != null && !Payload.Equals ("")) {
				if (details.ToString ().Length > 0)
					details.Append (DELIMITER);
				details.Append ("" + KEY_PAYLOAD + KEY_VALUE_DELIMITER
				+ maskCommas (Payload));
			}
			// tech:
			if (Techs != null && !Techs.Equals ("")) {
				details.Append (DELIMITER);
				string tech = string.Join (",", Techs);
				details.Append ("" + KEY_TECHS + KEY_VALUE_DELIMITER + maskCommas (tech));
			}

			return details.ToString ();
		}

		private string maskCommas (string original)
		{
			if (original == null || original.Equals (""))
				return "";
			return original.Replace (",", ",,");
		}

		private void unmarshall (string receivedString)
		{
			char[] receivedChars = receivedString.ToCharArray ();

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
				storeKV (key, valueBuilder.ToString ());
			}
		}

		private void storeKV (char key, string value)
		{
			switch (key) {
			case KEY_ID:
				Id = value;
				break;
			case KEY_PAYLOAD:
				Payload = value;
				break;
			case KEY_TECHS:
				Techs = value.Split (new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
				break;
			default:
			// unknown key parsed: invalid NFC_Info
				Valid = false;
				break;
			}
		}
	}

}
