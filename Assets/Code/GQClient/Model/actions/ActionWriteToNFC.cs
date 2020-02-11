using System.Xml;
using GQ.Client.Util;
using QM.NFC;

namespace GQ.Client.Model
{
    public class ActionWriteToNFC : Action

    {
        #region Structure
        public ActionWriteToNFC(XmlReader reader) : base(reader) { }

        public string Content { get; protected set; } = null;

        protected override void ReadAttributes(XmlReader reader)
        {
            base.ReadAttributes(reader);

            Content = GQML.GetStringAttribute(GQML.ACTION_WRITETONFC_CONTENT, reader);
        }
        #endregion


        #region Runtime
        public override void Execute()
        {
            string payload = Content.MakeReplacements();

            if (payload.Equals("[null]"))
                return;


            if (payload.StartsWith("\"") && payload.EndsWith("\""))
            {
                payload = payload.Substring(1, payload.Length - 2);
            }

            NFC_Connector.Connector.NFCWrite(payload);
        }
        #endregion
    }
}
