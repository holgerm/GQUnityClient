namespace GQ.Client.Model
{
    public class ActionSaveVar : ActionAbstractWithVar
    {
        public override void Execute()
        {
            Variables.SaveVariableToPrefs(VarName);
        }
    }
}
