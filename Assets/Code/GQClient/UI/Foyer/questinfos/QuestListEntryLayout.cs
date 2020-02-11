using Code.GQClient.Conf;
using Code.GQClient.UI.layout;

namespace Code.GQClient.UI.Foyer.questinfos
{

    public class QuestListEntryLayout : LayoutConfig
    {

        public override void layout()
        {
            //// set entry background color:
            //Image image = GetComponent<Image> ();
            //if (image != null) {
            //	image.color = ConfigurationManager.Current.listEntryBgColor;
            //}

            // set heights and colors of text and image:
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(gameObject, fgColor: ConfigurationManager.Current.listEntryBgColor);
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(gameObject, "InfoButton", sizeScaleFactor: 0.65f, fgColor: ConfigurationManager.Current.listEntryFgColor);
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(gameObject, "Name", fgColor: ConfigurationManager.Current.listEntryFgColor);
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(gameObject, "DownloadButton", fgColor: ConfigurationManager.Current.listEntryFgColor);
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(gameObject, "StartButton", fgColor: ConfigurationManager.Current.listEntryFgColor);
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(gameObject, "DeleteButton", fgColor: ConfigurationManager.Current.listEntryFgColor);
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(gameObject, "UpdateButton", fgColor: ConfigurationManager.Current.listEntryFgColor);
        }

    }
}
