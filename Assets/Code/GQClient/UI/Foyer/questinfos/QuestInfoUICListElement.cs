using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using UnityEngine.UI;
using System;
using GQ.Client.Util;
using GQ.Client.Err;
using GQ.Client.GQEvents;
using GQ.Client.Conf;
using GQ.Client.UI.Dialogs;
using System.IO;
using GQ.Client.FileIO;

//using UnityEngine.Events;


namespace GQ.Client.UI.Foyer
{

    /// <summary>
    /// Represents one quest info object in a list within the foyer.
    /// </summary>
    public class QuestInfoUICListElement : QuestInfoUIC
    {

        #region Content and Structure

        protected static readonly string PREFAB = "QuestInfoListElement";

        protected const string NAME_PATH = "Name";

        public Button InfoButton;

        /// <summary>
        /// The download button is available WHEN this quest is on server but not on device.
        /// (IsOnServer && !IsOnDevice)
        /// </summary>
        public Button DownloadButton;

        /// <summary>
        /// The start button is available WHEN this quest is on device.
        /// (IsOnDevice)
        /// </summary>
        public Button StartButton;

        /// <summary>
        /// The update button is available WHEN this quest is on device and a newer version is on server.
        /// (HasUpdate)
        /// </summary>
        public Button UpdateButton;

        protected QuestListController listController { get; set; }

        /// <summary>
        /// The delete button is available WHEN this quest is locally on device.
        /// (IsOnDevice)
        /// If it is not on server a warning is issued before deletion will be executed.
        /// (&& !IsOnServer)
        /// If this quest is also predeployed, an information is issued that the predeployed and older version
        /// will remain in the list. That version is always older, since only a newer version can ever 
        /// have been loaded as an update of the original predeployed version.
        /// (&& IsPredeployed)
        /// </summary>
        // TODO what happens if we take predeployed into account.
        public Button DeleteButton;

        private enum DeletionWarning
        {
            NoWarning,
            WarningNotOnServer,
            InfoPredeployedSurvivesDelete
        }

        private DeletionWarning DeletionWarnState
        {
            get
            {
                if (!data.IsOnServer)
                {
                    return DeletionWarning.WarningNotOnServer;
                }
                if (data.IsPredeployed)
                {
                    return DeletionWarning.InfoPredeployedSurvivesDelete;
                }
                return DeletionWarning.NoWarning;
            }
        }

        #endregion


        #region Internal UI Control Functions

        public override void Hide()
        {
            transform.SetParent(listController.HiddenQuests.transform);
        }

        public override void Show()
        {
            transform.SetParent(listController.InfoList.transform);
            gameObject.SetActive(true);
        }

        protected void HideAllButtons()
        {
            DownloadButton.gameObject.SetActive(false);
            StartButton.gameObject.SetActive(false);
            DeleteButton.gameObject.SetActive(false);
            UpdateButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Shows (additionally) the given buttons and add the given method to the onClick listener.
        /// </summary>
        /// <param name="button">Button.</param>
        /// <param name="actionCallback">Action callback.</param>
        protected void ShowButtons(params Button[] buttons)
        {
            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(true);
                button.interactable = true;
            }
            // in case we can start this quest, we also allow clicks on the quest name to start it:
            Button.ButtonClickedEvent namebuttonEvent = Name.GetComponent<Button>().onClick;
            if (StartButton.gameObject.activeInHierarchy)
            {
                namebuttonEvent.RemoveAllListeners();
                namebuttonEvent.AddListener(() =>
                {
                    data.Play().Start();
                });
            }
            else
            {
                namebuttonEvent.RemoveAllListeners();
            }
        }

        #endregion


        #region Event Reaction Methods for Unity

        public void Download()
        {
            data.Download();
        }

        public void Delete()
        {
            data.Delete();
        }

        public void Play()
        {
            data.Play().Start();
        }

        public void UpdateQuest()
        {
            data.Update();
        }

        #endregion


        #region Runtime API

        public static GameObject Create(GameObject root, QuestInfo qInfo, QuestListController containerController)
        {
            // Create the view object for this controller:
            GameObject go = PrefabController.Create(PREFAB, root);
            go.name = PREFAB + " (" + qInfo.Name + ")";

            // set entry height:
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(go);

            QuestInfoUICListElement ctrl = go.GetComponent<QuestInfoUICListElement>();

            // set info button as configured:
            ctrl.setCategorySymbol(qInfo);

            // set data and event management:
            ctrl.data = qInfo;
            ctrl.listController = containerController;
            ElipsifyOverflowingText eot = ctrl.Name.transform.GetComponent<ElipsifyOverflowingText>();
            if (eot != null)
            {
                eot.maxLineNumbers = ConfigurationManager.Current.listEntryUseTwoLines ? 2 : 1;
            }
            ctrl.data.OnChanged += ctrl.UpdateView;
            ctrl.UpdateView();
            return go;
        }

        public override void UpdateView()
        {
            Debug.Log("UpdateView() on " + data.Name);

            // Update Info-Icon:
            // set info button as configured:
            setCategorySymbol(data);

            // Update Name:
            Name.text = data.Name;
            // Set Name button for download or play or nothing:
            Button nameButton = Name.gameObject.GetComponent<Button>();
            Button.ButtonClickedEvent namebuttonEvent = nameButton.onClick;
            namebuttonEvent.RemoveAllListeners();
            if (data.IsOnServer && !data.IsOnDevice)
            {
                namebuttonEvent.AddListener(() =>
                {
                    data.Download();
                });
            }
            if (data.IsOnDevice)
            {
                namebuttonEvent.AddListener(() =>
                {
                    data.Play().Start();
                });
            }

            if (data.Name == "Neue Quest 2") {
                int a = 0;
                a++;
            }

            // Update Buttons:
            HideAllButtons();
            // Show DOWNLOAD button if needed:
            if (data.ShowDownloadOption)
            {
                DownloadButton.gameObject.SetActive(true);
                DownloadButton.interactable = true;
                Debug.Log("Down");
            }
            // Show START button if needed:
            //if (ShowStartOption (data)) {
            //	StartButton.gameObject.SetActive (true);
            //	StartButton.interactable = true;
            //}
            // Show UPDATE button if needed:
            if (data.ShowUpdateOption)
            {
                UpdateButton.gameObject.SetActive(true);
                UpdateButton.interactable = true;
                Debug.Log("Up");
            }
            // Show DELETE button if needed:
            if (data.ShowDeleteOption)
            {
                DeleteButton.gameObject.SetActive(true);
                DeleteButton.interactable = true;
                Debug.Log("Del");
            }

            ElipsifyOverflowingText elipsify = Name.GetComponent<ElipsifyOverflowingText>();
            if (elipsify != null)
            {
                elipsify.ElipsifyText();
            }
            // TODO make elipsify automatic when content of name text changes....???!!!

            // TODO call the lists sorter ...
        }

        private void setCategorySymbol(QuestInfo qInfo)
        {
            // set info button as configured:
            if (ConfigurationManager.Current.mainCategorySet != null && ConfigurationManager.Current.mainCategorySet != "")
            {
                CategorySet mainCategorySet = ConfigurationManager.Current.GetMainCategorySet();
                Category determiningCategory = null;
                foreach (string myCatId in qInfo.Categories)
                {
                    determiningCategory = mainCategorySet.categories.Find(mainCat => mainCat.id == myCatId);
                    if (determiningCategory != null)
                        break;
                }

                Image infoImage = InfoButton.transform.Find("Image").GetComponent<Image>();
                infoImage.enabled = true;
                infoImage.color = ConfigurationManager.Current.listEntryFgColor;
                InfoButton.enabled = false;
                InfoButton.gameObject.SetActive(true); // show info icon

                if (determiningCategory != null)
                {
                    // set symbol for this category:
                    infoImage.sprite = determiningCategory.symbol != null ?
                        Resources.Load<Sprite>(determiningCategory.symbol.path) :
                        null;
                    if (infoImage.sprite != null)
                    {
                        infoImage.enabled = true;
                        InfoButton.enabled = true;
                        InfoButton.gameObject.SetActive(true);
                    }
                }
            }

        }

        #endregion


        #region Initialization in Editor

        public virtual void Reset()
        {
            Name = EnsurePrefabVariableIsSet<Text>(Name, "Name", NAME_PATH);

            DownloadButton = EnsurePrefabVariableIsSet<Button>(DownloadButton, "Download Button", "DownloadButton");
            StartButton = EnsurePrefabVariableIsSet<Button>(StartButton, "Start Button", "StartButton");
            DeleteButton = EnsurePrefabVariableIsSet<Button>(DeleteButton, "Delete Button", "DeleteButton");
            UpdateButton = EnsurePrefabVariableIsSet<Button>(UpdateButton, "Update Button", "UpdateButton");
        }

        #endregion
    }

}