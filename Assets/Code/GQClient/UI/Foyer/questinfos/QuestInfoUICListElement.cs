using UnityEngine;
using GQ.Client.Model;
using UnityEngine.UI;
using GQ.Client.Conf;
using TMPro;

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

        public Button NameButton;

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

        public override void Hide(GameObject go = null)
        {
            transform.SetParent(listController.HiddenQuests.transform);
        }

        public override void Show(GameObject go = null)
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
                    data.Play();
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
            data.Play();
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
            GameObject go = PrefabController.Create("prefabs", PREFAB, root);
            go.name = PREFAB + " (" + qInfo.Name + ")";

            // set entry height:
            FoyerListLayoutConfig.SetQuestInfoEntryLayout(go);

            QuestInfoUICListElement ctrl = go.GetComponent<QuestInfoUICListElement>();

            // set info button as configured:
            ctrl.setCategorySymbol(qInfo);

            // set data and event management:
            ctrl.data = qInfo;
            ctrl.data.ActivitiesBlockingChanged += ctrl.OnActivitiesBlockingChanged;
            ctrl.listController = containerController;
            ctrl.data.OnChanged += ctrl.UpdateView;
            ctrl.UpdateView();
            return go;
        }

        private void OnActivitiesBlockingChanged(bool isBlocking)
        {
            DownloadButton.interactable = !isBlocking;
            DeleteButton.interactable = !isBlocking;
            StartButton.interactable = !isBlocking;
            UpdateButton.interactable = !isBlocking;
            NameButton.interactable = !isBlocking;
        }

        public override void UpdateView()
        {
            // Update Info-Icon:
            // set info button as configured:
            setCategorySymbol(data);

            // Update Name:
            Name.text = data.Name;
            // Set Name button for download or play or nothing:
            Button nameButton = Name.gameObject.GetComponent<Button>();
            nameButton.onClick.RemoveAllListeners();
            if (data.ShowDownloadOption)
            {
                nameButton.onClick.AddListener(() =>
                {
                    Download();
                });
            }
            if (data.IsOnDevice)
            {
                nameButton.onClick.AddListener(() =>
                {
                    Play();
                });
            }

            // Update Buttons:
            HideAllButtons();
            // Show DOWNLOAD button if needed:
            if (data.ShowDownloadOption)
            {
                DownloadButton.gameObject.SetActive(true);
                DownloadButton.interactable = true;
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
            }
            // Show DELETE button if needed:
            if (data.ShowDeleteOption)
            {
                DeleteButton.gameObject.SetActive(true);
                DeleteButton.interactable = true;
            }

            // TODO call the lists sorter ...
        }

        private void setCategorySymbol(QuestInfo qInfo)
        {
            // set info button as configured:
            if (ConfigurationManager.Current.mainCategorySet != null && ConfigurationManager.Current.mainCategorySet != "")
            {
                CategorySet mainCategorySet = ConfigurationManager.Current.GetMainCategorySet();
                if (mainCategorySet == null)
                    return;

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
            Name = EnsurePrefabVariableIsSet<TextMeshProUGUI>(Name, "Name", NAME_PATH);

            DownloadButton = EnsurePrefabVariableIsSet<Button>(DownloadButton, "Download Button", "DownloadButton");
            StartButton = EnsurePrefabVariableIsSet<Button>(StartButton, "Start Button", "StartButton");
            DeleteButton = EnsurePrefabVariableIsSet<Button>(DeleteButton, "Delete Button", "DeleteButton");
            UpdateButton = EnsurePrefabVariableIsSet<Button>(UpdateButton, "Update Button", "UpdateButton");
            NameButton = EnsurePrefabVariableIsSet<Button>(NameButton, "Name", "Name");
        }

        #endregion
    }

}