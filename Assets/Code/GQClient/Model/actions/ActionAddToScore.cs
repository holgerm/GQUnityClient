using System.Xml;

namespace GQ.Client.Model
{

    public class ActionAddToScore : Action
     {

        #region Structure
        public ActionAddToScore(XmlReader reader) : base(reader) { }

        protected int scoreToAdd;

		protected override void ReadAttributes (XmlReader reader)
		{
			scoreToAdd = GQML.GetIntAttribute (GQML.ACTION_SETVARIABLE_VALUE, reader);
		}
		#endregion

		#region Runtime
		public override void Execute ()
		{
			int oldScore = Variables.GetValue (GQML.VAR_SCORE).AsInt ();

			Variables.SetVariableValue (GQML.VAR_SCORE, new Value (oldScore + scoreToAdd));
		}
		#endregion
		}
}
