using Code.GQClient.Conf;
using GQClient.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.UI.Foyer.containers
{
    internal class QuestTopicTreeController : QuestContainerController
    {
        public Button upwardButton;
        public TMP_Text topicName;
        public Button forwardButton;
        public GameObject topicArea;
        public GameObject topicContentRoot;
        public GameObject questInfoArea;

        private new void Start()
        {
            base.Start();
            Topic.OnCursorChanged += SetDirty;
            Topic.CursorHome();
        }

        private static bool _dirtyTopicTree;

        private static void SetDirty()
        {
            _dirtyTopicTree = true;
        }

        private void LateUpdate()
        {
            if (_dirtyTopicTree)
            {
                UpdateView();
            }
        }


        /// <summary>
        /// Update the Topic Tree View to reflect a change in the topic tree model.
        /// </summary>
        private void UpdateView()
        {
            upwardButton.targetGraphic.enabled = 
                Topic.Cursor.Parent != Topic.Null;
            upwardButton.interactable =
                Topic.Cursor.Parent != Topic.Null;
            
            forwardButton.targetGraphic.enabled = false;
            forwardButton.interactable = false;
            
            topicName.text = Topic.Cursor.Name;

            if (Topic.Cursor.Children.Count > 0)
                ShowTopicArea();
            else
            {
                topicArea.SetActive(false);
            }

            questInfoArea.SetActive(Topic.Cursor.NumberOfOwnQuestInfos > 0);

            _dirtyTopicTree = false;
        }

        protected override void SorterChanged()
        {
            // TODO maybe we could sort for alphabet, numbers, date, grades etc.
        }

        protected override void FilterChanged()
        {
            UpdateView();
        }

        protected override void ListChanged()
        {
            Topic.ClearAll();

            foreach (var info in Qim.GetFilteredQuestInfos())
            {
                foreach (var topic in info.Topics)
                {
                    Topic.Create(topic).AddQuestInfo(info);
                }
            }

            SetDirty();
        }

        private void ShowTopicArea()
        {
            // clean topic area:
            var rootT = topicContentRoot.transform;
            for (var i = 0; i < rootT.childCount; i++)
            {
                var childGo = rootT.GetChild(i).gameObject;
                childGo.SetActive(false);
                Destroy(childGo);
            }

            Config.ResetColorPalette();
            // create topic buttons:
            foreach (var topic in Topic.Cursor.Children)
            {
                TopicButtonCtrl.Create(topicContentRoot, topic);
            }

            // show topic area:
            topicArea.SetActive(true);
        }

        protected override void RemovedInfo(QuestInfoChangedEvent e)
        {
            Topic.RemoveQuestInfo(e.OldQuestInfo, true);
        }

        protected override void ChangedInfo(QuestInfoChangedEvent e)
        {
            Topic.RemoveQuestInfo(e.OldQuestInfo, true);
            Topic.InsertQuestInfo(e.NewQuestInfo);
        }

        protected override void AddedInfo(QuestInfoChangedEvent e)
        {
            Topic.InsertQuestInfo(e.NewQuestInfo);
        }

        /// <summary>
        /// Unity event function e.g. for Back Button in TopicTree UI Area.
        /// </summary>
        public void OnUpwardSelected()
        {
            Topic.CursorMoveUp();
        }

        public void OnEnable()
        {
            TopicFilter.Instance.IsActive = true;
        }

        public void OnDisable()
        {
            TopicFilter.Instance.IsActive = false;
        }
    }
}