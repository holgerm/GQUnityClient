using System.Xml;
using Code.GQClient.Model.expressions;

namespace Code.GQClient.Model.actions
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
