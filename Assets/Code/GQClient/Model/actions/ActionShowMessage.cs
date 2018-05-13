using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Xml.Serialization;
using GQ.Client.Err;
using System.Xml;
using GQ.Client.UI.Dialogs;

namespace GQ.Client.Model
{

	public class ActionShowMessage : ActionAbstract
	{
		#region State

		public string Message { get; set; }

		#endregion


		#region Structure

		protected override void ReadAttributes (XmlReader reader)
		{
			Message = GQML.GetStringAttribute (GQML.ACTION_SHOWMESSAGE_MESSAGE, reader, "");
		}

		#endregion


		#region Functions

		public override void Execute ()
		{
			MessageDialog dialog = new MessageDialog (Message);
			dialog.Start ();
		}

		#endregion
	}
}
