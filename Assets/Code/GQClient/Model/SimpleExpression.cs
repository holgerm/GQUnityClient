using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;

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
		/// Reader should be at the bool, num, string or var element when called.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public void ReadXml (System.Xml.XmlReader reader)
		{
			XmlSerializer serializer;

			string expressionName = reader.LocalName;

			reader.MoveToContent ();

			XmlRootAttribute xmlRootAttr = new XmlRootAttribute ();
			xmlRootAttr.IsNullable = true;

			while (reader.Read ()) {

				// if we reach the end of this condition element we are ready to leave this method.
				if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals (expressionName)) {
					break;
				}

				if (reader.NodeType == XmlNodeType.Text) {
					setValue (reader.Value);
				}
			}
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
