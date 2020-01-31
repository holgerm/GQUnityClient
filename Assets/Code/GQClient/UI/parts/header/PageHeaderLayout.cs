using System;
using GQ.Client.Conf;
using GQ.Client.Err;
using GQ.Client.Model;
using GQ.Client.UI.Dialogs;
using GQ.Client.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GQ.Client.UI
{
    public class PageHeaderLayout : HeaderLayout
    {
        public PageController pageCtrl;

        protected override void Start()
        {
            pageCtrl = GameObject.Find("/PageController").GetComponent<PageController>();
            base.Start();
        }

        public void OnEnable()
        {
            Author.SettingsChanged += Author_SettingsChanged;
        }

        public void OnDisable()
        {
            Author.SettingsChanged -= Author_SettingsChanged;
        }


        protected override void setHeader()
        {
            if (pageCtrl == null)
            {
                enableLeaveQuestButton(ConfigurationManager.Current.OfferLeaveQuests);
            }
            else
            {
                enableLeaveQuestButton(pageCtrl.OfferLeaveQuest);
            }

            base.setHeader();

            Canvas headerCanvas = Header.GetComponent<Canvas>();
            if (headerCanvas != null)
            {
                headerCanvas.overrideSorting = true;
                headerCanvas.sortingOrder = 3;
            }


        }

        protected override void setMiddleButton()
        {
            switch (ConfigurationManager.Current.headerMiddleButtonPolicy)
            {
                case HeaderMiddleButtonPolicy.TopLogo:
                    setTopLogo();
                    break;
                case HeaderMiddleButtonPolicy.QuestTitle:
                    setTitle();
                    break;
            }
        }

        protected void setTopLogo()
        {
            // set MiddleTopLogo:
            try
            {
                // hide tite text:
                Transform titleText = MiddleButton.transform.Find("TitleText");
                titleText.gameObject.SetActive(false);

                // show top logo and load image:
                Transform middleTopLogo = MiddleButton.transform.Find("TopLogo");
                middleTopLogo.gameObject.SetActive(true);

                if (middleTopLogo != null)
                {
                    Image mtlImage = middleTopLogo.GetComponent<Image>();
                    if (mtlImage != null)
                    {
                        mtlImage.sprite = Resources.Load<Sprite>(ConfigurationManager.Current.topLogo.path);
                    }
                }
            }
            catch (Exception e)
            {
                Log.SignalErrorToDeveloper("Could not set Middle Top Logo Image. Exception occurred: " + e.Message);
            }
        }

        protected void setTitle()
        {
            // hide top logo and load image:
            Transform middleTopLogo = MiddleButton.transform.Find("TopLogo");
            middleTopLogo.gameObject.SetActive(false);

            // show tite and set its text:
            Transform titleText = MiddleButton.transform.Find("TitleText");
            titleText.gameObject.SetActive(true);
            TextMeshProUGUI ttt = titleText.GetComponent<TextMeshProUGUI>();

            // ignore setting a title if we have no text element:
            if (ttt == null)
                return;

            ttt.text = (Author.LoggedIn && Author.ShowHiddenQuests) || "".Equals(QuestManager.Instance.CurrentQuestName4User)
                ? QuestManager.Instance.CurrentQuest.Name
                : QuestManager.Instance.CurrentQuestName4User;

            ttt.color = ConfigurationManager.Current.mainFgColor;
        }

        void Author_SettingsChanged(object sender, System.EventArgs e)
        {
            setTitle();
        }

        void enableLeaveQuestButton(bool enable)
        {
            // gather game objects and components:
            Transform menuButtonT = Header.transform.Find("ButtonPanel/MenuButton");
            Button menuButton = menuButtonT.GetComponent<Button>();
            Image image = menuButtonT.transform.Find("Image").GetComponent<Image>();

            // put icon:
            image.sprite = Resources.Load<Sprite>("icons/endQuest");

            // put function and activate button:
            menuButton.onClick.AddListener(leaveQuest);
            menuButton.enabled = true;

            // show:
            menuButton.enabled = enable;
            image.enabled = enable;
        }

        void leaveQuest()
        {
            Quest curQuest = QuestManager.Instance.CurrentQuest;
            if (curQuest != null)
            {
                if (ConfigurationManager.Current.warnWhenLeavingQuest)
                {
                    CancelableFunctionDialog.Show(
                        ConfigurationManager.Current.warnDialogTitleWhenLeavingQuest,
                        ConfigurationManager.Current.warnDialogMessageWhenLeavingQuest,
                        curQuest.End,
                        ConfigurationManager.Current.warnDialogOKWhenLeavingQuest,
                        ConfigurationManager.Current.warnDialogCancelWhenLeavingQuest);
                }
                 else
                {
                    curQuest.End();
                }
            }
        }

    }
}
