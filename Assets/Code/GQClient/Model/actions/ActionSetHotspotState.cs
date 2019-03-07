using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Xml.Serialization;
using GQ.Client.Err;
using System.Xml;

namespace GQ.Client.Model
{

	public class ActionSetHotspotState : ActionAbstract
	{
		#region State

		public int HotspotId { get; set; }

		public bool ApplyToAll { get; set; }

        public string Activity { get; set; }

        public string Visbility { get; set; }

        #endregion


        #region Structure

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
                hotspot.Active = (Activity == GQML.ACTIVE);
                hotspot.Visible = (Visbility == GQML.VISIBLE);

                Debug.Log(("Action SetHotspotState hotspot: " + HotspotId + ", activity: " + Activity + ", visibility: " + Visbility).Yellow());
            }

        }

		#endregion
	}
}
