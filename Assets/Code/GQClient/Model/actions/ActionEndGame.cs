using System.Xml;

namespace GQ.Client.Model
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
