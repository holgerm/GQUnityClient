using System.Text;
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
        
        public TMP_Text text;
        
        private new void Start()
        {
            base.Start();
            Topic.OnCursorChanged += UpdateView;
            Topic.CursorHome();
        }

        /// <summary>
        /// Update the Topic Tree View to reflect a change in the topic tree model.
        /// </summary>
        private void UpdateView()
        {
            upwardButton.enabled = 
                Topic.Cursor.Parent != Topic.Null;
            forwardButton.enabled = false;
            topicName.text = Topic.Cursor.Name;
            
            if(Topic.Cursor.Children.Count > 0)
                ShowTopicArea();
            else
            {
                topicArea.SetActive(false);
            }
            
            questInfoArea.SetActive(Topic.Cursor.NumberOfOwnQuestInfos > 0);
        }
        
        protected override void SorterChanged()
        {
            // TODO maybe we could sort for alphabet, numbers, date, grades etc.
        }

        protected override void FilterChanged()
        {
            // ignore filter changes
        }

        protected override void ListChanged()
        {
            Topic.ClearAll();
            
            foreach (var info in qim.GetFilteredQuestInfos())
            {
                foreach (var topic in info.Topics)
                {
                    Topic.Create(topic).AddQuestInfo(info);
                }
            }
            var t = Topic.Cursor;
            
            UpdateView();
        }

        private void ShowTopicArea()
        {
            // clean topic area:
            var rootT = topicContentRoot.transform;
            for (int i = 0; i < rootT.childCount; i++)
            {
                var childGo = rootT.GetChild(i).gameObject;
                childGo.SetActive(false);
                Destroy(childGo);
            }

            // create topic buttons:
            foreach (var topic in Topic.Cursor.Children)
            {
                var topicButtonCtrl = TopicButtonCtrl.Create(topicContentRoot, topic);
            }
            
            // show topic area:
            topicArea.SetActive(true);
        }

        protected override void RemovedInfo(QuestInfoChangedEvent e)
        {
            Topic.RemoveQuestInfo(e.OldQuestInfo);
            ShowTopicArea();
        }

        protected override void ChangedInfo(QuestInfoChangedEvent e)
        {
            Topic.RemoveQuestInfo(e.OldQuestInfo);
            Topic.InsertQuestInfo(e.NewQuestInfo);
            ShowTopicArea();
        }

        protected override void AddedInfo(QuestInfoChangedEvent e)
        {
            Topic.InsertQuestInfo(e.NewQuestInfo);
            ShowTopicArea();
        }

        /// <summary>
        /// Unity event function e.g. for Back Button in TopicTree UI Area.
        /// </summary>
        public void OnUpwardSelected()
        {
            Topic.CursorMoveUp();
        }
    }
}