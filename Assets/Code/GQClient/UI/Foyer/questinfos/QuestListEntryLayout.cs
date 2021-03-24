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
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(gameObject, fgColor: Config.Current.listEntryBgColor);
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(gameObject, "InfoButton", sizeScaleFactor: 0.65f, fgColor: Config.Current.listEntryFgColor);
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(gameObject, "Name", fgColor: Config.Current.listEntryFgColor);
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(gameObject, "DownloadButton", fgColor: Config.Current.listEntryFgColor);
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(gameObject, "StartButton", fgColor: Config.Current.listEntryFgColor);
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(gameObject, "DeleteButton", fgColor: Config.Current.listEntryFgColor);
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(gameObject, "UpdateButton", fgColor: Config.Current.listEntryFgColor);
        }

    }
}
