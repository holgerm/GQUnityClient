using System;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.UI.author;
using Code.GQClient.UI.Dialogs;
using Code.GQClient.UI.pages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.parts.header
{
    public class PageHeaderLayout : HeaderLayout
    {
        private PageController _pageController;

        private PageController PageController
        {
            get
            {
                if (_pageController == null)
                {
                    _pageController = GameObject.Find("/PageController").GetComponent<PageController>();
                    if (_pageController == null)
                    {
                        Log.SignalErrorToDeveloper("PageHeaderLayout: pageController not found at '/PageController'.");
                    }
                } 
                return _pageController;
            }
            set => _pageController = value;
        }

        protected override void Start()
        {
            PageController = GameObject.Find("/PageController").GetComponent<PageController>();
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
            enableLeaveQuestButton(ConfigurationManager.Current.OfferLeaveQuests);
 
            base.setHeader();

            var headerCanvas = Header.GetComponent<Canvas>();
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
            Debug.Log("Set TopLogo");
            // set MiddleTopLogo:
            try
            {
                // hide tite text:
                var titleText = MiddleButton.transform.Find("TitleText");
                titleText.gameObject.SetActive(false);

                // show top logo and load image:
                var middleTopLogo = MiddleButton.transform.Find("TopLogo");
                middleTopLogo.gameObject.SetActive(true);

                if (middleTopLogo != null)
                {
                    var mtlImage = middleTopLogo.GetComponent<Image>();
                    if (mtlImage != null)
                    {
                        mtlImage.sprite = ConfigurationManager.Current.topLogo.GetSprite();
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
            Debug.Log("Set TopTitle");
            // hide top logo and load image:
            var middleTopLogo = MiddleButton.transform.Find("TopLogo");
            middleTopLogo.gameObject.SetActive(false);

            // show tite and set its text:
            var titleText = MiddleButton.transform.Find("TitleText");
            titleText.gameObject.SetActive(true);
            var ttt = titleText.GetComponent<TextMeshProUGUI>();

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
            var menuButtonT = Header.transform.Find("ButtonPanel/MenuButton");
            var menuButton = menuButtonT.GetComponent<Button>();
            var image = menuButtonT.transform.Find("Image").GetComponent<Image>();

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
            var curQuest = QuestManager.Instance.CurrentQuest;
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
