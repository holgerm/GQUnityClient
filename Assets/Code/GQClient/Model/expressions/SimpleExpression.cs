using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using GQ.Client.Err;

namespace GQ.Client.Model
{

	/// <summary>
	/// Simple expression. Covers num, bool, var and string expressions.
	/// </summary>
	public abstract class SimpleExpression : IExpression
	{

		#region Structure

		public System.Xml.Schema.XmlSchema GetSchema ()
		{
			return null;
		}

		public void WriteXml (System.Xml.XmlWriter writer)
		{
			Debug.LogWarning ("WriteXML not implemented for " + GetType ().Name);
		}

		protected Value value;

		/// <summary>
		/// Reader should be at the bool, num, string or var element when called. It completely consumes that node incl the end element.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public void ReadXml (System.Xml.XmlReader reader)
		{
			if (reader.NodeType != XmlNodeType.Element || !GQML.IsExpressionType (reader.LocalName)) {
				Log.SignalErrorToDeveloper (
					"Instead of an xml element of an expression we got an {0} with name {1}", 
					reader.NodeType.ToString (),
					reader.LocalName
				);
			}

			string expressionName = reader.LocalName;

			reader.MoveToContent ();

			while (
				reader.NodeType != XmlNodeType.Text && 
				reader.NodeType != XmlNodeType.EndElement && 
				reader.NodeType != XmlNodeType.None &&
				!reader.IsEmptyElement
			) {
				reader.Read ();
			}

			setValue (reader.Value);

			while (!reader.IsEmptyElement && reader.NodeType != XmlNodeType.EndElement) {
				reader.Read ();
			}

			GQML.AssertReaderAtEnd (reader, expressionName);
			reader.Read ();
		}

		protected abstract void setValue (string valueAsString);

		#endregion


		#region Function

		public virtual Value Evaluate ()
		{
			return value;
		}

		#endregion
	
	}
}
