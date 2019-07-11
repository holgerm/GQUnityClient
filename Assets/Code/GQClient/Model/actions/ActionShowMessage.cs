using System.Xml;
using GQ.Client.UI.Dialogs;
using GQ.Client.Util;

namespace GQ.Client.Model
{

    public class ActionShowMessage : Action
   
	{
		#region State
		public string Message { get; set; }

		public string Buttontext { get; set; }
        #endregion


        #region Structure
        public ActionShowMessage(XmlReader reader) : base(reader) { }

        protected override void ReadAttributes (XmlReader reader)
		{
			Message = GQML.GetStringAttribute (GQML.ACTION_SHOWMESSAGE_MESSAGE, reader, "");
			Buttontext = GQML.GetStringAttribute (GQML.ACTION_SHOWMESSAGE_BUTTONTEXT, reader, "Ok");
		}
		#endregion


		#region Functions
		public override void Execute ()
		{
			MessageDialog dialog = new MessageDialog (Message.MakeReplacements(), Buttontext);
			dialog.Start ();
		}
		#endregion
	}
}
