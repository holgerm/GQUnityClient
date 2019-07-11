using System.Xml;

namespace GQ.Client.Model
{

    public class ActionStartMission : Action
   
	{
		#region State
		public int Id { get; set; }

		public bool AllowReturn { get; set; }
        #endregion


        #region Structure
        public ActionStartMission(XmlReader reader) : base(reader) { }

        protected override void ReadAttributes (XmlReader reader)
		{
			Id = GQML.GetIntAttribute (GQML.ID, reader);
			AllowReturn = GQML.GetOptionalBoolAttribute (GQML.ACTION_STARTMISSION_ALLOWRETURN, reader, false);
		}
		#endregion


		#region Functions
		public override void Execute ()
		{
			Page pageToStart = Quest.GetPageWithID (Id);
            if (pageToStart.PageType != "MetaData")
            {
                pageToStart.Start(AllowReturn);
            }
			else {
				Quest.End ();
			}
		}
		#endregion
	}
}
