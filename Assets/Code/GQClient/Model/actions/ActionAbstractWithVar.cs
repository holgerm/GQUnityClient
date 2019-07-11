using System.Xml;

namespace GQ.Client.Model
{

    public abstract class ActionAbstractWithVar : Action

    {
        public ActionAbstractWithVar(XmlReader reader) : base(reader) { }

        private string varName = null;

        public string VarName
        {
            get
            {
                return varName;
            }
            protected set
            {
                varName = value;
            }
        }

        protected override void ReadAttributes(XmlReader reader)
        {
            base.ReadAttributes(reader);

            VarName = GQML.GetStringAttribute(GQML.VARIABLE, reader);
        }
    }
}
