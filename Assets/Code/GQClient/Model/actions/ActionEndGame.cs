using System.Xml;

namespace Code.GQClient.Model.actions
{

    public class ActionEndGame : Action
   
	{
        public ActionEndGame(XmlReader reader) : base(reader) { }

        public override void Execute ()
		{
			Quest.End ();
		}
	}
}
