// #define DEBUG_LOG

using System.Collections;
using System.Collections.Generic;
using GQClient.Model;
using Code.GQClient.UI.author;
using Code.GQClient.UI.Foyer.questinfos;
using Code.GQClient.Util;
using UnityEngine;

namespace Code.GQClient.UI.Foyer.containers
{
    /// <summary>
    /// Abstract super class of all viewer controllers that show quest sets.
    /// E.g. the quest list, topic hierarchy and map in the foyer.
    /// </summary>
    public abstract class QuestContainerController : MonoBehaviour
    {
        protected QuestInfoManager qim;

        protected Dictionary<int, QuestInfoUIC> QuestInfoControllers;

        protected void Start()
        {
            qim = QuestInfoManager.Instance;
            QuestInfoControllers = new Dictionary<int, QuestInfoUIC>();
            qim.OnDataChange += OnQuestInfoChanged;
            qim.OnFilterChange += OnQuestInfoChanged;
            ShowDeleteOption.DeleteOptionVisibilityChanged += UpdateElementViews;
        }

        public virtual void OnQuestInfoChanged(object sender, QuestInfoChangedEvent e)
        {
#if DEBUG_LOG
            Debug.Log("QuestListController.OnQuestInfoChanged e.type: " + e.ChangeType.ToString());
#endif
            switch (e.ChangeType)
            {
                case ChangeType.AddedInfo:
                    AddedInfo(e);
                    break;
                case ChangeType.ChangedInfo:
                    ChangedInfo(e);
                    break;
                case ChangeType.RemovedInfo:
                    RemovedInfo(e);
                    break;
                case ChangeType.ListChanged:
                    ListChanged();
                    break;
                case ChangeType.FilterChanged:
                    FilterChanged();
                    break;
                case ChangeType.SorterChanged:
                    SorterChanged();
                    break;
            }
        }

        protected abstract void SorterChanged();

        protected abstract void FilterChanged();

        protected abstract void ListChanged();

        protected abstract void RemovedInfo(QuestInfoChangedEvent e);

        protected abstract void ChangedInfo(QuestInfoChangedEvent e);

        protected abstract void AddedInfo(QuestInfoChangedEvent e);

        void OnDestroy()
        {
            if (qim != null)
            {
                qim.OnDataChange -= OnQuestInfoChanged;
                qim.OnFilterChange -= OnQuestInfoChanged;
                ShowDeleteOption.DeleteOptionVisibilityChanged -= UpdateElementViews;
            }
        }

        /// <summary>
        /// Assumes no element is new and no element has been removed,
        /// but their state or the context for showing them has changed.
        /// </summary>
        private void UpdateElementViews()
        {
            CoroutineStarter.Run(UpdateElementViewsAsCoroutine());
        }

        private IEnumerator UpdateElementViewsAsCoroutine()
        {
            foreach (KeyValuePair<int, QuestInfoUIC> kvp in QuestInfoControllers)
            {
                kvp.Value.UpdateView();
                yield return null;
            }
        }
    }
}