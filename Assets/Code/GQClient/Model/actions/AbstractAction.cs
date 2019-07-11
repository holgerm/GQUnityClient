using System.Xml;
using GQ.Client.Err;

namespace GQ.Client.Model
{
    public abstract class Action : I_GQML, IParentedXml
    {

        #region Structure
        public I_GQML Parent { get; set; }

        public Quest Quest
        {
            get
            {
                return Parent.Quest;
            }
        }

        /// <summary>
        /// Reader is at action element when we call this method. 
        /// The complete action node incl. the end element is consumed when we leave.
        /// </summary>
        /// <param name="reader">Reader.</param>
        protected Action(XmlReader reader)
        {
            CheckStart(reader);

            ReadAttributes(reader);

            if (reader.IsEmptyElement)
            {
                // consume the empty action element and terminate:
                reader.Read();
                return;
            }

            // consume the Begin Action Element:
            reader.Read();

            // if we find another element within this action we read that:
            if (reader.NodeType == XmlNodeType.Element)
            {
                ReadContent(reader);
            }
            else
            {
                if (!reader.Read())
                {
                    return;
                }
            }

            // consume the closing action tag (if not empty action element)
            if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals(GQML.ACTION))
                reader.Read();
        }


        protected virtual void ReadAttributes(XmlReader reader) { }

        protected virtual void CheckStart(XmlReader reader)
        {
            GQML.AssertReaderAtStart(reader, GQML.ACTION);
        }

        protected virtual void ReadContent(XmlReader reader)
        {
            switch (reader.LocalName)
            {
                // UNKOWN CASE:
                default:
                    Log.WarnDeveloper("Action has additional unknown {0} element. (Ignored) line {1} position {2}",
                        reader.LocalName,
                        ((IXmlLineInfo)reader).LineNumber,
                        ((IXmlLineInfo)reader).LinePosition);
                    reader.Skip();
                    break;
            }
        }
        #endregion

        #region Functions
        public abstract void Execute();
        #endregion
    }
}
