using System.Xml;
using Code.GQClient.Model.gqml;

namespace Code.GQClient.Model.actions
{

    public class ActionSetHotspotState : Action
   
	{
		#region State
		public int HotspotId { get; set; }

		public bool ApplyToAll { get; set; }

        public string Activity { get; set; }

        public string Visbility { get; set; }
        #endregion

        #region Structure
        public ActionSetHotspotState(XmlReader reader) : base(reader) { }

        protected override void ReadAttributes (XmlReader reader)
		{
            HotspotId = GQML.GetIntAttribute (GQML.HOTSPOT, reader);
            ApplyToAll = GQML.GetOptionalBoolAttribute (GQML.ACTION_SETHOTSPOTSTATE_APPLYTOALL, reader, false);
            Activity = GQML.GetStringAttribute(GQML.ACTION_SETHOTSPOTSTATE_ACTIVITY, reader, defaultVal:GQML.ACTIVE);
            Visbility = GQML.GetStringAttribute(GQML.ACTION_SETHOTSPOTSTATE_VISIBILITY, reader, defaultVal: GQML.VISIBLE);
        }
        #endregion


        #region Functions
        public override void Execute ()
		{
            if (ApplyToAll)
            {
                foreach (var hotspot in Quest.AllHotspots)
                {
                    hotspot.Active = (Activity == GQML.ACTIVE);
                    hotspot.Visible = (Visbility == GQML.VISIBLE);
                }
            }
            else
            {
                Hotspot hotspot = Quest.GetHotspotWithID(HotspotId);
                if (hotspot == null)
                    return;
                hotspot.SetActivityAndVisibilityAtOnce(
                    Activity == GQML.ACTIVE, 
                    Visbility == GQML.VISIBLE);
            }
        }
		#endregion
	}
}
