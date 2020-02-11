using System.Xml;
using Code.GQClient.Model.expressions;

namespace Code.GQClient.Model.actions
{
    public class ActionSaveVar : ActionAbstractWithVar
    {
        public ActionSaveVar(XmlReader reader) : base(reader) { }

        public override void Execute()
        {
            Variables.SaveVariableToPrefs(VarName);
        }
    }
}
