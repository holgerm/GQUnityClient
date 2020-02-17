// #define DEBUG_LOG

using System.Xml;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.pages;
using UnityEngine;

namespace Code.GQClient.Model.actions
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
#if DEBUG_LOG
                    Debug.LogFormat("StartMission Action from page {0} ({1}) to id {2}",
                        Quest.CurrentPage.Id,
                        Quest.CurrentPage.PageType,
                        Id);
#endif
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
