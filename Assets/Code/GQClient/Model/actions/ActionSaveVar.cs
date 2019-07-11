using System.Xml;

namespace GQ.Client.Model
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
