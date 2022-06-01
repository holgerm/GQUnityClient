using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.UI.Foyer;
using Code.GQClient.UI.Foyer.containers;
using Code.GQClient.UI.menu.categories;
using Code.GQClient.UI.menu.viewToggle;
using Code.GQClient.Util;
using Code.GQClient.Util.http;
using Code.GQClient.Util.tasks;
using Code.QM.Util;
using UnityEngine;
using UnityEngine.Networking;

namespace GQClient.Model
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

        public static string QuestsRelativeBasePath => "quests";

        public static string LocalQuestInfoJsonPath => LocalQuestsPath + "infos.json";

        public Dictionary<int, QuestInfo> QuestDict { get; }

        public List<QuestInfo> GetListOfQuestInfos()
        {
            return QuestDict.Values.ToList<QuestInfo>();
        }

        public IEnumerable<QuestInfo> GetFilteredQuestInfos()
        {
            var filteredList = QuestDict.Values.Where(x => Filter.Accept(x)).ToList();
            return filteredList;
        }

        public bool ContainsQuestInfo(int id)
        {
            return QuestDict.ContainsKey(id);
        }

        public int Count => QuestDict.Count;

        public QuestInfo GetQuestInfo(int id)
        {
            return (QuestDict.TryGetValue(id, out QuestInfo questInfo) ? questInfo : null);
        }

        #endregion


        #region Filter

        private QuestInfoFilter _filter;

        public QuestInfoFilter Filter
        {
            get => _filter;
            private set
            {
                if (_filter != value)
                {
                    _filter = value;
                    FilterChange.Invoke();
                }
            }
        }

        public QuestInfoFilter.CategoryFilter CategoryFilter;

        private Dictionary<string, QuestInfoFilter.CategoryFilter> _categoryFilters;

        /// <summary>
        /// Adds the given andFilter in conjunction to the current filter(s).
        /// </summary>
        /// <param name="andFilter">And filter.</param>
        private void FilterAnd(QuestInfoFilter andFilter)
        {
            Filter = new QuestInfoFilter.And(Filter, andFilter);
        }

        public readonly Observable FilterChange = new Observable();

        #endregion


        #region Quest Info Changes

        private void AddInfo(QuestInfo newInfo)
        {
            string debugMsg = newInfo.Topics.Count > 0
                ? $"QIM.AddInfo(): {newInfo.Name} topic: {newInfo.Topics[0]}"
                : $"QIM.AddInfo(): {newInfo.Name} no topic.";
            Debug.Log(debugMsg);
            
            QuestDict.Add(newInfo.Id, newInfo);

            QuestInfoChangedEvent ev = new QuestInfoChangedEvent(
                $"Info for quest {newInfo.Name} added.",
                type: ChangeType.AddedInfo,
                newQuestInfo: newInfo
            );

            DataChange.Invoke(ev);
        }

        public void AddLocalInfos(IEnumerable<QuestInfo> quests)
        {
            // filter for valid new quests:
            IEnumerable<QuestInfo> newQuests =
                from q in quests
                where q.Id > 0 && !QuestDict.ContainsKey(q.Id)
                select q;

            foreach (QuestInfo qi in newQuests)
            {
                QuestDict.Add(qi.Id, qi);
            }
            
            QuestInfoChangedEvent ev = new QuestInfoChangedEvent(
                $"Info for multiple quests in list added.",
                type: ChangeType.ListChanged
            );

            DataChange.Invoke(ev);
        }

        public void AddInfosFromServer(IEnumerable<QuestInfo> quests)
        {
                        // we make a separate list of ids of all old quest infos:
            var oldIDsToBeRemoved = new List<int>(QuestDict.Keys);

            // we create new qi elements and keep those we can reuse. We remove those from our helper list.
            foreach (var newInfo in quests)
            {
                if (QuestDict.TryGetValue(newInfo.Id, out var oldInfo))
                {
                    // this new element was already there, hence we keep it (remove from the remove list) and update if newer:
                    oldIDsToBeRemoved.Remove(newInfo.Id);

                    if (oldInfo.ServerTimeStamp == null || oldInfo.ServerTimeStamp < newInfo.ServerTimeStamp)
                    {
                        UpdateInfo(newInfo);
                    }
                }
                else
                {
                    AddInfo(newInfo);
                }
            }
            
            // now in the helper list only the old elements that are not mentioned in the new list anymore are left. Hence we delete them:
            foreach (var oldId in oldIDsToBeRemoved)
            {
                if (Config.Current.autoSyncQuestInfos)
                {
                    // with autoSync we automatically remove the local quest data:
                    QuestDict[oldId].Delete();

                    // and also delete the quest infos from the list ...
                    RemoveInfo(oldId);
                }
                else
                {
                    // when manually syncing ...
                    if (QuestDict[oldId].IsOnDevice)
                    {
                        // if the quest exists local, we simply set the server-side update timestamp to null:
                        QuestDict[oldId].DeletedFromServer();
                        // this will trigger an OnChanged event and update the according view of the list element
                    }
                    else
                    {
                         // if the quest has not been loaded yet, we remove the quest info:
                        QuestDict[oldId].Delete(); // introduced newly without exact knowledge (hm)
                        RemoveInfo(oldId);
                    }
                }
            }
            
            DataChange.Invoke(
                new QuestInfoChangedEvent(
                    "QuestInfoManager updated",
                    ChangeType.ListChanged));
        }

        private void UpdateInfo(QuestInfo changedInfo)
        {
            Debug.Log($"QuestInfoManager.UpdateInfo({changedInfo.Name})");
            
            if (!QuestDict.TryGetValue(changedInfo.Id, out QuestInfo curInfo))
            {
                Log.SignalErrorToDeveloper("Quest Inf {0} could not be updated because it was not found locally.",
                    changedInfo.Id);
                return;
            }

            curInfo.QuestInfoRecognizeServerUpdate(changedInfo);

            // // React also as container to a change info event
            // if (Filter.Accept(curInfo)) // TODO should we also do it, if the new qi does not pass the filter?
            // {
            //     // Run through filter and raise event if involved
            //     DataChange.Invoke(
            //         new QuestInfoChangedEvent(
            //             message: $"Info for quest {curInfo.Name} changed.",
            //             type: ChangeType.ChangedInfo,
            //             newQuestInfo: curInfo,
            //             oldQuestInfo: curInfo
            //         )
            //     );
            // }
        }

        public void RemoveInfo(int oldInfoId)
        {
            if (!QuestDict.TryGetValue(oldInfoId, out QuestInfo oldInfo))
            {
                Log.SignalErrorToDeveloper(
                    "Trying to remove quest info with ID {0} but it does not exist in QuestInfoManager.",
                    oldInfoId
                );
                return;
            }

            oldInfo.Dispose();
            QuestDict.Remove(oldInfoId);

            DataChange.Invoke(
                new QuestInfoChangedEvent(
                    $"Info for quest {oldInfo.Name} removed.",
                    type: ChangeType.RemovedInfo,
                    oldQuestInfo: oldInfo
                )
            );
        }

        /// <summary>
        /// Updates the quest infos from the server and integrates the gathered data into the local data. 
        /// 
        /// Should be called in cases like the list is shown again (or first time), 
        /// the server connection is gained back, the last update is long ago or the user demands an update.
        /// </summary>
        public static void UpdateQuestInfos()
        {
            ImportQuestInfos importLocal =
                new ImportLocalQuestInfos();
            var unused = Base.Instance.GetSimpleBehaviour(
                importLocal,
                $"Aktualisiere {Config.Current.nameForQuestsPl}",
                $"Lese lokale {Config.Current.nameForQuestsPl}"
            );

            var downloader =
                new Downloader(
                    url: ConfigurationManager.UrlPublicQuestsJson,
                    new DownloadHandlerBuffer(),
                    timeout: Config.Current.timeoutMS,
                    maxIdleTime: Config.Current.maxIdleTimeMS
                );
            var unused2 = Base.Instance.GetDownloadBehaviour(
                downloader,
                $"Aktualisiere {Config.Current.nameForQuestsPl}"
            );

            ImportQuestInfos importFromServer =
                new ImportServerQuestInfos();
            var unused3 = Base.Instance.GetSimpleBehaviour(
                importFromServer,
                $"Aktualisiere {Config.Current.nameForQuestsPl}",
                $"Neue {Config.Current.nameForQuestsPl} werden lokal gespeichert"
            );

            var exporter =
                new ExportQuestInfosToJson();
            var unused4 = Base.Instance.GetSimpleBehaviour(
                exporter,
                $"Aktualisiere {Config.Current.nameForQuestsPl}",
                $"{Config.Current.nameForQuestSg}-Daten werden gespeichert"
            );

            var autoLoader =
                new AutoLoadAndUpdate();
            
            var t =
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
        public static void UpdateLocalQuestInfosOnly()
        {
            ImportQuestInfos importLocal =
                new ImportLocalQuestInfos();

            importLocal.Start();
        }

        /// <summary>
        /// Here one can register listeners that will be called each time when the quest infos are successfully updated.
        /// </summary>
        public static event Task.TaskCallback OnQuestInfosUpdateSucceeded;

        /// <summary>
        /// Triggers when categories are read from RTConfig.json files, be it originally app distributed, locally persisted or server-based. 
        /// </summary>
        public readonly Observable<QuestInfoChangedEvent> DataChange =
            new Observable<QuestInfoChangedEvent>();

        #endregion


        #region singleton

        private static QuestInfoManager _instance;

        public static QuestInfoManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new QuestInfoManager();
                    _instance.initViews();
                    _instance.InitFilters();
                }

                return _instance;
            }
        }

        public static void Reset()
        {
            _instance = null;
        }

        private QuestInfoManager()
        {
            // init quest info store:
            QuestDict = new Dictionary<int, QuestInfo>();
            DataChange.AddListener(data =>
            {
                if (ChangeType.ListChanged == data.ChangeType)
                    InitFilters();
            });
            
            // register for changes in topic cursor (i.e. the currently shown topic)
            // this should be done within the TopicView Controller etc. not here, or? TODO
            Topic.CursorChangedEvent += FilterChange.Invoke;
        }

        void initViews()
        {
            if (Config.Current.questInfoViews == null ||
                Config.Current.questInfoViews.Length == 0)
            {
                Log.SignalErrorToDeveloper("No quest info views defined for this app. Fix that!");
                return;
            }

            var startView = Config.Current.questInfoViews[0];
            Base.Instance.ListCanvas.gameObject.SetActive(startView == QuestInfoView.List.ToString());
            Base.Instance.TopicTreeCanvas.gameObject.SetActive(startView == QuestInfoView.TopicTree.ToString());
            Base.Instance.Map.gameObject.SetActive(startView == QuestInfoView.Map.ToString());
            Base.Instance.MapCanvas.gameObject.SetActive(startView == QuestInfoView.Map.ToString());

            // check whether we have alternative views to offer:
            if (Config.Current.questInfoViews.Length <= 1)
                return;

            // Create the multi-toggle View for the view alternatives currently not displayed, i.e. 2 to n:
            GameObject menuContent = Base.Instance.MenuTopLeftContent;
            ViewToggleController.Create(menuContent);
        }

        /// <summary>
        /// Initializes the quest info filters, e.g. called at start when the QuestInfoManager is initialized.
        /// </summary>
        public void InitFilters()
        {
            Instance.FilterChange.DisableNotification();

            // init filters
            Filter = new QuestInfoFilter.All();

            // init hidden quests filter:
            FilterAnd(QuestInfoFilter.HiddenQuestsFilter.Instance);

            // init local quests filter:
            FilterAnd(QuestInfoFilter.LocalQuestInfosFilter.Instance);

            // init TopicFilter:
            FilterAnd(TopicFilter.Instance);

            // init category filters:
            _categoryFilters = new Dictionary<string, QuestInfoFilter.CategoryFilter>();
            var catSets = Config.Current.CategorySets;
            foreach (var catSet in catSets)
            {
                _categoryFilters[catSet.name] = new QuestInfoFilter.CategoryFilter(catSet);
                FilterAnd(_categoryFilters[catSet.name]);
            }

            Instance.FilterChange.EnableNotification(false);

            // create UI for Category Filters:
            foreach (var catSet in Config.Current.CategorySets)
            {
                CategoryTreeCtrl ctrl = CategoryTreeCtrl.Create(
                    root: Base.Instance.MenuTopLeftContent,
                    catFilter: _categoryFilters[catSet.name],
                    categories: catSet.categories);
            }
        }

        #endregion

        /// <summary>
        /// Remove listeners of QuestContainerController instance if possible:
        /// </summary>
        /// <param name="qcc"></param>
        public static void DoDestroy(QuestContainerController qcc)
        {
            if (_instance != null)
            {
                Instance.DataChange.RemoveListener(qcc.OnQuestInfoChanged);
                Instance.FilterChange.RemoveListener(qcc.FilterChanged);
            }
        }
    }

    public class QuestInfoChangedEvent : EventArgs
    {
        private string Message { get; }

        public ChangeType ChangeType { get; }

        public QuestInfo NewQuestInfo { get; }

        public QuestInfo OldQuestInfo { get; }

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

        public override string ToString()
        {
            var message = $"{ChangeType}: {Message}";
            return message;
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