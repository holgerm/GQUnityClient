using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using GQ.Client.Err;

namespace GQ.Client.Model
{
	public abstract class ActionAbstract : IAction
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

		public I_GQML Parent { get; set; }

		public Quest Quest {
			get {
				return Parent.Quest;
			}
		}

		/// <summary>
		/// Reader is at action element when we call this method. 
		/// The complete action node incl. the end element is consumed when we leave.
		/// </summary>
		/// <param name="reader">Reader.</param>
		public virtual void ReadXml (XmlReader reader)
		{
			GQML.AssertReaderAtStart (reader, GQML.ACTION);

			ReadAttributes (reader);

			if (reader.IsEmptyElement) {
				// consume the empty action element and terminate:
				reader.Read ();
				return;
			}

			// consume the Begin Action Element:
			reader.Read (); 

			XmlRootAttribute xmlRootAttr = new XmlRootAttribute ();
			xmlRootAttr.IsNullable = true;

			while (!GQML.IsReaderAtEnd (reader, GQML.ACTION)) {

				// if we find another element within this action we read that:
				if (reader.NodeType == XmlNodeType.Element) {
					ReadContent (reader, xmlRootAttr);
				} else {
					if (!reader.Read ()) {
						return;
					}
				}
			}
		
			// consume the closing action tag (if not empty action element)
			if (reader.NodeType == XmlNodeType.EndElement)
				reader.Read ();
		}

		//		protected void tryThis (XmlReader reader)
		//		{
		//			if (!reader.IsEmptyElement) {
		//				// consume the starting action element, when there is content
		//				reader.Read ();
		//			}
		//		}
		//
		protected virtual void ReadAttributes (XmlReader reader)
		{
		}

		protected virtual void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		{
			switch (reader.LocalName) {
			// UNKOWN CASE:
			default:
				Log.WarnDeveloper ("Action has additional unknown {0} element. (Ignored)", reader.LocalName);
				reader.Skip ();
				break;
			}
		}

		#endregion


		#region Functions

		public abstract void Execute ();

		#endregion
	}
}
