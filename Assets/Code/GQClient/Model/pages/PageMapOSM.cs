using System.Xml;
using Code.GQClient.Model.gqml;

namespace Code.GQClient.Model.pages
{
    public class PageMapOSM : PageNavigation
	{
        #region XML Serialization
        public PageMapOSM(XmlReader reader) : base(reader) { }

        protected override void ReadAttributes (XmlReader reader)
		{
            base.ReadAttributes(reader);

            // we actually do not read from xml but already know what the values should be:
            mapOption = true;
			listOption = false; 

			qrOption = false;
			qrText = "";
			qrNotFoundText = "";

			nfcOption = false;
			nfcText = "";
			nfcNotFoundText = "";

			numberOption = false;
			numberText = "";
			numberNotFoundText = "";

			iBeaconOption = false;
			iBeaconText = "";
			iBeaconNotFoundText = "";
		}
		#endregion

		#region Runtime API
		/// <summary>
		/// Maps the scene to this model for a page (mission).
		/// </summary>
		/// <value>The name of the page scene.</value>
		public override string PageSceneName {
			get {
				return GQML.PAGE_TYPE_NAVIGATION;
			}
		}
		#endregion
	}
}
