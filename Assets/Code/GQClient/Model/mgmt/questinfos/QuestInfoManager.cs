using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using GQ.Client.Util;
using GQ.Client.Conf;
using GQ.Client.Model;
using Newtonsoft.Json;
using System.IO;
using GQ.Client.Err;
using GQ.Client.UI.Dialogs;
using System.Linq;
using QM.Util;
using GQ.Client.UI;
using GQ.Client.FileIO;


namespace GQ.Client.Model
{

    /// <summary>
    /// Manages the meta data for all quests available: locally on the device as well as remotely on the server.
    /// </summary>
    public class QuestInfoManager
    {

        #region store & access data

        public static string LocalQuestsPath
        {
            get
            {
                if (!Directory.Exists(Device.GetPersistentDatapath() + "/quests/"))
                {
                    Directory.CreateDirectory(Device.GetPersistentDatapath() + "/quests/");
                }
                return Device.GetPersistentDatapath() + "/quests/";
            }
        }

        public static string LocalQuestInfoJSONPath
        {
            get
            {
                return LocalQuestsPath + "infos.json";
            }
        }

        public Dictionary<int, QuestInfo> QuestDict
        {
            get;
            set;
        }

        public List<QuestInfo> GetListOfQuestInfos()
        {
            return QuestDict.Values.ToList<QuestInfo>();
        }

        public IEnumerable<QuestInfo> GetFilteredQuestInfos()
        {
            return QuestDict.Values.Where(x => Filter.Accept(x)).ToList();
        }

        public bool ContainsQuestInfo(int id)
        {
            return QuestDict.ContainsKey(id);
        }

        public int Count
        {
            get
            {
                return QuestDict.Count;
            }
        }

        public QuestInfo GetQuestInfo(int id)
        {
            QuestInfo questInfo;
            return (QuestDict.TryGetValue(id, out questInfo) ? questInfo : null);
        }

        #endregion


        #region Filter

        private QuestInfoFilter _filter;

        public QuestInfoFilter Filter
        {
            get
            {
                return _filter;
            }
            protected set
            {
                if (_filter != value)
                {
                    _filter = value;
                    // we register with later changes of the filter:
                    _filter.filterChange += FilterChanged;
                    // we use the new filter instantly:
                    FilterChanged();
                }
            }
        }

        public QuestInfoFilter.CategoryFilter CategoryFilter;

        public Dictionary<string, QuestInfoFilter.CategoryFilter> CategoryFilters;

        /// <summary>
        /// Adds the given andFilter in conjunction to the current filter(s).
        /// </summary>
        /// <param name="andFilter">And filter.</param>
        public void FilterAnd(QuestInfoFilter andFilter)
        {
            Filter = new QuestInfoFilter.And(Filter, andFilter);
        }

        public event ChangeCallback OnFilterChange;

        public void FilterChanged()
        {
            if (OnFilterChange != null)
            {
                OnFilterChange(
                    this,
                    new QuestInfoChangedEvent(
                        "Filter changed ...",
                        ChangeType.FilterChanged,
                        newQuestInfo: null,
                        oldQuestInfo: null
                    )
                );
            }
        }

        #endregion


        #region Quest Info Changes
        public void AddInfo(QuestInfo newInfo)
        {
            QuestDict.Add(newInfo.Id, newInfo);

            if (Filter.Accept(newInfo))
            {
                // Run through filter and raise event if involved:
                raiseDataChange(
                    new QuestInfoChangedEvent(
                        String.Format("Info for quest {0} added.", newInfo.Name),
                        ChangeType.AddedInfo,
                        newQuestInfo: newInfo
                    )
                );
            }
        }

        public void UpdateInfo(QuestInfo changedInfo)
        {
            QuestInfo oldInfo = null;
            if (QuestDict.TryGetValue(changedInfo.Id, out oldInfo))
            {
                oldInfo.QuestInfoHasBeenUpdatedTo(changedInfo);
            }
            else
            {
                Log.SignalErrorToDeveloper("Quest Inf {0} could not be updated because ot was not found locally.", changedInfo.Id);
            }
        }

        public void RemoveInfo(int oldInfoID)
        {
            Debug.Log("RemoveInfo(" + oldInfoID + ")");

            QuestInfo oldInfo = null;
            if (!QuestDict.TryGetValue(oldInfoID, out oldInfo))
            {
                Log.SignalErrorToDeveloper(
                    "Trying to remove quest info with ID {0} but it deos not exist in QuestInfoManager.",
                    oldInfoID
                );
                return;
            }

            oldInfo.Dispose();
            QuestDict.Remove(oldInfoID);

            if (Filter.Accept(oldInfo))
            {
                // Run through filter and raise event if involved

                raiseDataChange(
                    new QuestInfoChangedEvent(
                        String.Format("Info for quest {0} removed.", oldInfo.Name),
                        ChangeType.RemovedInfo,
                        oldQuestInfo: oldInfo
                    )
                );
            }
        }

        /// <summary>
        /// Updates the quest infos from the server and intergrates the gathered data into the local data. 
        /// 
        /// Should be called in cases like the list is shown again (or first time), 
        /// the server connection is gained back, the last update is long ago or the user demands an update.
        /// </summary>
        public void UpdateQuestInfos()
        {
            Debug.Log("UpdateQuestInfos()");
            ImportQuestInfos importLocal =
                new ImportLocalQuestInfos();
            new SimpleDialogBehaviour(
                importLocal,
                string.Format("Aktualisiere {0}", ConfigurationManager.Current.nameForQuestsPl),
                string.Format("Lese lokale {0}", ConfigurationManager.Current.nameForQuestSg)
            );

            Downloader downloader =
                new Downloader(
                    url: ConfigurationManager.UrlPublicQuestsJSON,
                    timeout: ConfigurationManager.Current.timeoutMS,
                    maxIdleTime: ConfigurationManager.Current.maxIdleTimeMS
                );
            new DownloadDialogBehaviour(
                downloader,
                string.Format("Aktualisiere {0}", ConfigurationManager.Current.nameForQuestsPl)
            );

            ImportQuestInfos importFromServer =
                new ImportServerQuestInfos();
            new SimpleDialogBehaviour(
                importFromServer,
                string.Format("Aktualisiere {0}", ConfigurationManager.Current.nameForQuestsPl),
                string.Format("Neue {0} werden lokal gespeichert", ConfigurationManager.Current.nameForQuestsPl)
            );

            ExportQuestInfosToJSON exporter =
                new ExportQuestInfosToJSON();
            new SimpleDialogBehaviour(
                exporter,
                string.Format("Aktualisiere {0}", ConfigurationManager.Current.nameForQuestsPl),
                string.Format("{0}-Daten werden gespeichert", ConfigurationManager.Current.nameForQuestSg)
            );

            AutoLoadAndUpdate autoLoader =
                new AutoLoadAndUpdate();

            TaskSequence t = 
                new TaskSequence(
                    importLocal, 
                    downloader, 
                    importFromServer, 
                    exporter, 
                    autoLoader);
            t.OnTaskCompleted += OnQuestInfosUpdateSucceeded;
            t.Start();
        }

        /// <summary>
        /// Updates quest infos based on the local infos only. No server connection needed.
        /// </summary>
        public void UpdateLocalQuestInfosOnly()
        {
            Debug.Log("UpdateLocalQuestInfosOnly()");
            ImportQuestInfos importLocal =
                new ImportLocalQuestInfos();

            importLocal.Start();
        }

        /// <summary>
        /// Here one can register listeners that will be called each time when the quest infos are successfully updated.
        /// </summary>
        public static event Task.TaskCallback OnQuestInfosUpdateSucceeded;

        public delegate void ChangeCallback(object sender, QuestInfoChangedEvent e);

        private event ChangeCallback onDataChange;

        public event ChangeCallback OnDataChange
        {
            add
            {
                onDataChange += value;
                value(
                    this,
                    new QuestInfoChangedEvent(
                        "Initializing listener ...",
                        ChangeType.ListChanged,
                        newQuestInfo: null,
                        oldQuestInfo: null
                    )
                );
            }
            remove
            {
                onDataChange -= value;
            }
        }

        public int HowManyListerners()
        {
            return onDataChange.GetInvocationList().Length;
        }



        public virtual void raiseDataChange(QuestInfoChangedEvent e)
        {
            if (onDataChange != null)
                onDataChange(this, e);
        }

        #endregion


        #region singleton

        private static QuestInfoManager _instance = null;

        public static QuestInfoManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new QuestInfoManager();
                    _instance.initViews();
                    _instance.initFilters();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public static void Reset()
        {
            _instance = null;
        }

        public QuestInfoManager()
        {
            // init quest info store:
            QuestDict = new Dictionary<int, QuestInfo>();
        }

        void initViews()
        {
            if (ConfigurationManager.Current.questInfoViews == null || ConfigurationManager.Current.questInfoViews.Length == 0)
            {
                Log.SignalErrorToDeveloper("No quest info views defined for this app. Fix that!");
                return;
            }

            string startView = ConfigurationManager.Current.questInfoViews[0];
            Base.Instance.ListCanvas.gameObject.SetActive(startView == QuestInfoView.List.ToString());
            Base.Instance.MapCanvas.gameObject.SetActive(startView == QuestInfoView.Map.ToString());
            Base.Instance.MapHolder.gameObject.SetActive(startView == QuestInfoView.Map.ToString());

            // check whether we have alternative views to offer:
            if (ConfigurationManager.Current.questInfoViews.Length <= 1)
                return;

            // Create the multitoggle View for the view alternatives currently not displayed, i.e. 2 to n:
            GameObject menuContent = Base.Instance.MenuTopLeftContent;
            ViewToggleController.Create(menuContent);
        }

        /// <summary>
        /// Initializes the quest info filters, e.g. called at start when the QuestInfoManager is initialized.
        /// </summary>
		void initFilters()
        {
            // init filters
            Filter = new QuestInfoFilter.All();

            // init hidden quests filter:
            FilterAnd(QuestInfoFilter.HiddenQuestsFilter.Instance);

            // init local quests filter:
            FilterAnd(QuestInfoFilter.LocalQuestInfosFilter.Instance);

            // init category filters:
            CategoryFilters = new Dictionary<string, QuestInfoFilter.CategoryFilter>();
            List<CategorySet> catSets = ConfigurationManager.Current.CategorySets;
            foreach (CategorySet catSet in catSets)
            {
                CategoryFilters[catSet.name] = new QuestInfoFilter.CategoryFilter(catSet);
                FilterAnd(CategoryFilters[catSet.name]);
            }

            // create UI for Category Filters:
            GameObject menuContent = Base.Instance.MenuTopLeftContent;
            foreach (CategorySet catSet in ConfigurationManager.Current.CategorySets)
            {
                CategoryTreeCtrl.Create(menuContent, CategoryFilters[catSet.name], catSet.categories);
            }
        }

        #endregion
    }

    public class QuestInfoChangedEvent : EventArgs
    {
        public string Message { get; protected set; }

        public ChangeType ChangeType { get; protected set; }

        public QuestInfo NewQuestInfo { get; protected set; }

        public QuestInfo OldQuestInfo { get; protected set; }

        public QuestInfoChangedEvent(
            string message = "",
            ChangeType type = ChangeType.ChangedInfo,
            QuestInfo newQuestInfo = null,
            QuestInfo oldQuestInfo = null
        )
        {
            Message = message;
            ChangeType = type;
            NewQuestInfo = newQuestInfo;
            OldQuestInfo = oldQuestInfo;
        }

    }

    public enum ChangeType
    {
        AddedInfo,
        RemovedInfo,
        ChangedInfo,
        ListChanged,
        FilterChanged,
        SorterChanged
    }


}
