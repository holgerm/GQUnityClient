using System.Xml;

namespace GQ.Client.Model
{
    public class ActionLoadVar : ActionAbstractWithVar
    {
        public ActionLoadVar(XmlReader reader) : base(reader) { }

        public override void Execute()
        {
            Variables.LoadVariableFromStore(VarName);
        }
    }
}
