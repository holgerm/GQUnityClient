using System.Xml;
using Code.GQClient.Model.gqml;
using Code.GQClient.Util;
using QM.NFC;

namespace Code.GQClient.Model.actions
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
