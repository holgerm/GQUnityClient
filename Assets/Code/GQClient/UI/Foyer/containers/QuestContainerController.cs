﻿using System.Collections;
using System.Collections.Generic;
using Code.GQClient.Err;
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
        protected QuestInfoManager Qim => QuestInfoManager.Instance;

        private Dictionary<int, QuestInfoUIC> _questInfoControllers;
        protected bool StartUpdateViewAlreadyDone;

        protected Dictionary<int, QuestInfoUIC> QuestInfoControllers
        {
            get
            {
                if (_questInfoControllers == null)
                {
                    _questInfoControllers = new Dictionary<int, QuestInfoUIC>();
                }

                return _questInfoControllers;
            }
        }

        protected void Awake()
        {
            Qim.DataChange.AddListener(OnQuestInfoChanged);
            Qim.FilterChange.AddListener(FilterChanged);

            ShowDeleteOption.DeleteOptionVisibilityChanged += UpdateElementViews;
            StartUpdateViewAlreadyDone = true;
        }

        protected void Start()
        {
            // After registering we need to update our views now:
            RecreateTopicTree();
            //FilterChanged(); 
        }

        public virtual void OnQuestInfoChanged(QuestInfoChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.AddedInfo:
                    if (Qim.Filter.Accept(e.NewQuestInfo))
                        AddedInfo(e);
                    break;
                case ChangeType.RemovedInfo:
                    if (Qim.Filter.Accept(e.OldQuestInfo))
                        RemovedInfo(e);
                    break;
                case ChangeType.ListChanged:
                    RecreateTopicTree();
                    break;
                case ChangeType.FilterChanged:
                    FilterChanged();
                    break;
                case ChangeType.SorterChanged:
                    SorterChanged();
                    break;
                case ChangeType.ChangedInfo:
                    if (Qim.Filter.Accept(e.OldQuestInfo))
                        ChangedInfo(e);
                    break;
                default:
                    ChangedInfo(e);
                    break;
            }
        }

        protected abstract void SorterChanged();

        public abstract void FilterChanged();

        protected abstract void RecreateTopicTree();

        protected abstract void RemovedInfo(QuestInfoChangedEvent e);

        protected abstract void ChangedInfo(QuestInfoChangedEvent e);

        protected abstract void AddedInfo(QuestInfoChangedEvent e);

        // protected void OnEnable()
        // {
        //     Qim.OnDataChange += OnQuestInfoChanged;
        //     Qim.OnFilterChange += OnQuestInfoChanged;
        //     ShowDeleteOption.DeleteOptionVisibilityChanged += UpdateElementViews;
        // }
        //
        // protected void OnDisable()
        // {
        //     if (Qim != null)
        //     {
        //         Qim.OnDataChange -= OnQuestInfoChanged;
        //         Qim.OnFilterChange -= OnQuestInfoChanged;
        //         ShowDeleteOption.DeleteOptionVisibilityChanged -= UpdateElementViews;
        //     }
        // }

        void OnDestroy()
        {
            QuestInfoManager.DoDestroy(this);
            ShowDeleteOption.DeleteOptionVisibilityChanged -= UpdateElementViews;
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
                kvp.Value.UpdateView(kvp.Value.data);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}