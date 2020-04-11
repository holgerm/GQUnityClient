using System;
using System.Collections.Generic;
using System.Text;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using Code.GQClient.FileIO;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.UI;
using Code.GQClient.UI.author;
using Code.GQClient.UI.Dialogs;
using Code.GQClient.Util;
using Code.GQClient.Util.http;
using Code.GQClient.Util.tasks;
using Code.QM.Util;
using Newtonsoft.Json;
using UnityEngine;

namespace GQClient.Model
{
    /// <summary>
    /// Stores meta data about a quest, i.e. name, id, and some limited details about its content as well as usage data.
    /// 
    /// A questInfo object has the following live cycle / states:
    /// 
    /// - The Quest exists only on Server and has not been downloaded yet or has just been deleted. 
    ///   (Initially if not predeployed)
    /// 	- Can be downloaded
    /// 	- Can NOT be started
    /// 	- Can NOT be updated
    /// 	- Can NOT be deleted
    /// - The Quest has been downloaded and exists locally as well as on server with same version. (After download)
    /// 	- Can NOT be downloaded
    /// 	- Can be started
    /// 	- Can NOT be updated
    /// 	- Can be deleted
    /// - The quest exists locally but has been updated on Server:
    /// 	- Can NOT be downloaded
    /// 	- Can be started
    /// 	- Can be updated
    /// 	- Can be deleted
    /// - The quest exists locally but has been removed from Server:
    /// 	- Can NOT be downloaded
    /// 	- Can be started
    /// 	- Can NOT be updated
    /// 	- Can be deleted but a warning should be shown
    /// The life cycle for quest loaded from server can be seen here: @ref QuestsFromServerLifeCycle
    /// 
    /// With predeployed quest:
    /// - The quest has been predeployed locally and there is no newer version on server:
    /// 	- Can NOT be downloaded
    /// 	- Can be started
    /// 	- Can NOT be updated
    /// 	- Can NOT be deleted
    /// - The quest has been predeployed locally but has been updated on Server:
    /// 	- Can NOT be downloaded
    /// 	- Can be started
    /// 	- Can be updated
    /// 	- Can NOT be deleted
    /// - The quest has been predeployed locally but updated locally to the newest server version:
    /// 	- Can NOT be downloaded
    /// 	- Can be started
    /// 	- Can be downgraded (set back to the older predeployed version)
    /// 	- Can NOT be deleted
    /// The life cycle for predeployed quest can be seen here: @ref QuestsPredeployedLifeCycle
    /// 
    /// We represent these states by four features with two or three values each:
    /// 
    /// - Downloadable (true, false)
    /// - Startable (true, false)
    /// - Updatable (true, false)
    /// - Deletable (Yes, YesWithWarning, No, Downgrade)
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class QuestInfo : IComparable<QuestInfo>
    {
        #region Serialized Features

        [JsonProperty] private int id;

        public int Id
        {
            get { return id; }
            protected set { id = value; }
        }

        [JsonProperty] private string name;

        public string Name
        {
            get { return name; }
            private set { name = value; }
        }

        [JsonProperty] private string featuredImagePath;

        public string FeaturedImagePath
        {
            get { return featuredImagePath; }
            private set { featuredImagePath = value; }
        }

        [JsonProperty] private int? typeID;

        public int? TypeID
        {
            get { return typeID; }
            private set { typeID = value; }
        }

        [JsonProperty] private string iconPath;

        public string IconPath
        {
            get { return iconPath; }
            private set { iconPath = value; }
        }

        [JsonProperty] private HotspotInfo[] hotspots;

        public HotspotInfo[] Hotspots
        {
            get { return hotspots; }
            private set { hotspots = value; }
        }

        [JsonProperty] private MetaDataInfo[] metadata;

        public MetaDataInfo[] Metadata
        {
            get { return metadata; }
            private set
            {
                metadata = value;
                // reset categories which are lazily evaluated
                _categories = null;
            }
        }

        public string GetMetadata(string key)
        {
            foreach (var md in Metadata)
            {
                if (md.Key.Equals(key))
                    return md.Value;
            }

            return null;
        }

        [JsonProperty] private long? _lastUpdateOnDevice = null;

        public long? TimeStamp
        {
            get { return _lastUpdateOnDevice; }
            private set { _lastUpdateOnDevice = value; }
        }

        [JsonProperty] private long? lastUpdate;

        public long? ServerTimeStamp
        {
            get { return lastUpdate; }
            private set { lastUpdate = value; }
        }

        internal void DeletedFromServer()
        {
            NewVersionOnServer = null;
            InvokeOnChanged();
        }

        [JsonProperty] private long? _timestampOfPredeployedVersion = null;

        // TODO: move to a local data structure
        public long? TimestampOfPredeployedVersion
        {
            get { return _timestampOfPredeployedVersion; }
            set
            {
                // TODO how will we set this value? Do we need to invoke onChanged?
                if (_timestampOfPredeployedVersion == value)
                    return;

                _timestampOfPredeployedVersion = value;
                InvokeOnChanged();
            }
        }

        [JsonProperty] private int _playedTimes = 0;

        // TODO: move to a local data structure
        public int PlayedTimes
        {
            get { return _playedTimes; }
            set
            {
                if (_playedTimes == value)
                    return;
                _playedTimes = value;
                InvokeOnChanged();
            }
        }

        [JsonProperty]
        public QuestInfo NewVersionOnServer
        {
            get { return newVersionOnServer; }
            private set { newVersionOnServer = value; }
        }

        [JsonIgnore] QuestInfo newVersionOnServer;

        public void QuestContentHasBeenUpdated()
        {
            if (!IsUpdateValid(NewVersionOnServer))
            {
                return;
            }

            // OK. Let's go:
            Name = NewVersionOnServer.Name;
            FeaturedImagePath = NewVersionOnServer.FeaturedImagePath;
            TypeID = NewVersionOnServer.TypeID;
            IconPath = NewVersionOnServer.IconPath;
            TimeStamp = NewVersionOnServer.TimeStamp;
            Hotspots = NewVersionOnServer.Hotspots;
            Metadata = NewVersionOnServer.Metadata;
            // unchanged: TimestampOfPredeployedVersion
            // unchanged: PlayedTimes
            NewVersionOnServer = null;

            InvokeOnChanged();
        }

        public void QuestInfoHasBeenUpdatedTo(QuestInfo newQuestInfo)
        {
            if (!IsUpdateValid(newQuestInfo))
            {
                return;
            }

            // OK. Let's go:
            ServerTimeStamp = newQuestInfo.ServerTimeStamp;
            NewVersionOnServer = newQuestInfo;
            // the rest remains unchanged until content gets updated

            InvokeOnChanged();
        }

        private bool IsUpdateValid(QuestInfo newQuestInfo)
        {
            if (newQuestInfo == null)
            {
                Log.SignalErrorToDeveloper(
                    "QuestInfo Update to new server version failed: NO NEW server VERSION given. For Quest Id {0}", Id);
                return false;
            }

            if (Id != newQuestInfo.Id)
            {
                Log.SignalErrorToDeveloper(
                    "QuestInfo Update to new server version failed: Ids DIFFER for Quest Id {0} --> {1}",
                    Id, newQuestInfo.Id);
                return false;
            }

            if (TimeStamp >= newQuestInfo.ServerTimeStamp)
            {
                Log.SignalErrorToDeveloper(
                    "QuestInfo Update to new server version failedfor Quest Id {0}: server version NOT NEWER: local timestamp: {1} vs server timestamp: {2}",
                    Id, TimeStamp, ServerTimeStamp);
                return false;
            }

            return true;
        }

        #endregion

        #region Sub- and Super-Quests

        [JsonProperty] private List<int> superQuests = new List<int>();

        public void AddSuperQuest(int superQuestID)
        {
            if (superQuests == null)
            {
                superQuests = new List<int>();
            }

            superQuests.Add(superQuestID);
        }

        [JsonProperty] private List<int> subQuests = new List<int>();

        public void AddSubQuest(int subQuestID)
        {
            if (subQuests == null)
            {
                subQuests = new List<int>();
            }

            subQuests.Add(subQuestID);
        }

        #endregion


        #region Derived features

        [JsonIgnore]
        public HotspotInfo MarkerHotspot
        {
            get
            {
                double sumLong = 0f;
                double sumLat = 0f;
                foreach (HotspotInfo h in Hotspots)
                {
                    sumLong += h.Longitude;
                    sumLat += h.Latitude;
                }

                if (Hotspots.Length == 0)
                    return HotspotInfo.NULL;
                else
                    return new HotspotInfo(sumLat / Hotspots.Length, sumLong / Hotspots.Length);
            }
        }

        [JsonIgnore]
        public bool IsOnDevice
        {
            get { return (TimeStamp != null); }
        }

        public bool IsOnServer
        {
            get { return (ServerTimeStamp != null); }
        }

        [JsonIgnore]
        public bool IsPredeployed
        {
            get { return (TimestampOfPredeployedVersion != null); }
        }

        [JsonIgnore]
        public bool HasUpdate
        {
            get
            {
                return (
                    // exists on both device and server:
                    IsOnDevice && IsOnServer
                               // server update is newer (bigger number):
                               && ServerTimeStamp > TimeStamp
                );
            }
        }

        /// <summary>
        /// Determines whether this quest is new. This feature will be used in the UI in future versions.
        /// </summary>
        /// <returns><c>true</c> if this instance is new; otherwise, <c>false</c>.</returns>
        [JsonIgnore]
        public bool IsNew
        {
            get { return PlayedTimes == 0; }
        }

        public bool IsHidden()
        {
            return name.StartsWith("---", StringComparison.CurrentCulture);
        }

        public bool ShowDownloadOption
        {
            get { return IsOnServer && !IsOnDevice; }
        }

        public bool ShowStartOption
        {
            get { return IsOnDevice; }
        }

        public bool ShowUpdateOption
        {
            get { return HasUpdate; }
        }

        public bool ShowDeleteOption
        {
            get
            {
                return
                    IsOnDevice
                    // either configurated to offer delete or logged in as author:
                    && (Author.ShowDeleteOptionForLocalQuests);
            }
        }

        #endregion

        #region Categories

        [JsonIgnore] private List<string> _categories;

        [JsonIgnore]
        public List<string> Categories
        {
            get
            {
                if (_categories == null)
                {
                    Categories = CategoryReader.ReadCategoriesFromMetadata(Metadata);
                }

                return _categories;
            }
            set { _categories = value; }
        }

        public const string WITHOUT_CATEGORY_ID = "withoutcategory";

        public string CurrentCategoryId
        {
            get { return QuestInfoManager.Instance.Filter.CategoryToShow(this); }
        }

        #endregion

        #region Topics

        [JsonIgnore] private List<string> _topics;

        public List<string> Topics
        {
            get
            {
                if (_topics == null)
                {
                    Topics = TopicTreeReader.ReadTopicsFromMetadata(this);
                }

                return _topics;
            }
            set => _topics = value;
        }

        #endregion

        #region State & Events

        public event VoidToVoid OnChanged;

        protected void InvokeOnChanged()
        {
            if (OnChanged != null)
            {
                OnChanged();
            }
        }

        #endregion

        #region Sorting Comparison

        /// <summary>
        /// Returns a value greater than zero in case this object is considered greater than the given other. 
        /// A return value of 0 signals that both objects are equal and 
        /// a value less than zero means that this object is less than the given other one.
        /// </summary>
        /// <param name="otherInfo">Other info.</param>
        public int CompareTo(QuestInfo otherInfo)
        {
            if (SortAscending)
                return Compare(this, otherInfo);
            else
                return -Compare(this, otherInfo);
        }

        public delegate int CompareMethod(QuestInfo one, QuestInfo other);

        static public bool SortAscending = true;

        private static CompareMethod _compare;

        static public CompareMethod Compare
        {
            get
            {
                if (_compare == null)
                {
                    _compare = DEFAULT_COMPARE;
                }

                return _compare;
            }
            set { _compare = value; }
        }

        static public CompareMethod DEFAULT_COMPARE = ByName;

        static public CompareMethod ByName
        {
            get { return (QuestInfo one, QuestInfo other) => { return one.Name.CompareTo(other.Name); }; }
        }

        #endregion


        #region Runtime Functions

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("{0} (id: {1})\n", Name, Id);
            sb.AppendFormat("\t new version on server: {0}",
                NewVersionOnServer == null ? "null" : NewVersionOnServer.TimeStamp.ToString());
            // sb.AppendFormat("\t type id: {0}", TypeID);
            // sb.AppendFormat("\t icon path: {0}", IconPath);
            //  sb.AppendFormat("\t featured image path: {0}", FeaturedImagePath);
            //  sb.AppendFormat("\t with {0} hotspots.", Hotspots == null ? 0 : Hotspots.Length);
            //  sb.AppendFormat("\t and {0} metadata entries.", Metadata == null ? 0 : Metadata.Length);
            sb.Append($"\t HasUpdate?: {HasUpdate}");
            sb.Append($"\t IsOnDevice?: {IsOnDevice}");
            sb.Append($"\t IsOnServer?: {IsOnServer}");
            sb.Append($"\t ShowDownloadOption?: {ShowDownloadOption}");
            sb.Append($"\t ShowUpdateOption?: {ShowUpdateOption}");

            return sb.ToString();
        }

        public void Dispose()
        {
            OnChanged = null;
        }

        #region Downloading a Quest

        public event BoolToVoid ActivitiesBlockingChanged;

        protected void InvokeOnActivityBlockingChanged(bool newState)
        {
            if (ActivitiesBlockingChanged != null)
            {
                ActivitiesBlockingChanged(newState);
            }
        }

        private bool _activitiesBlocking;

        protected bool ActivitiesBlocking
        {
            get { return _activitiesBlocking; }
            set
            {
                _activitiesBlocking = value;
                InvokeOnActivityBlockingChanged(_activitiesBlocking);
            }
        }

        /// <summary>
        /// Downloads the quest represented by this info. Is called from the UI (Button e.g.).
        /// </summary>
        public void Download()
        {
            if (ActivitiesBlocking)
                return;

            var download = DownloadTask();

            // Set downloading state after download has ended:
            download.OnTaskEnded += (object sender, TaskEventArgs e) => { ActivitiesBlocking = false; };

            // chain exporting local qi json again after dowload has successfully completed:
            download.OnTaskCompleted +=
                (object sender, TaskEventArgs e) =>
                {
                    InvokeOnChanged();

                    new ExportQuestInfosToJSON().Start();
                };

            // DO IT:
            ActivitiesBlocking = true;
            download.Start();
        }

        private Task DownloadTask()
        {
            // Load quest data: game.xml
            var downloadGameXML =
                new Downloader(
                    url: QuestManager.GetQuestUri(Id),
                    timeout: ConfigurationManager.Current.timeoutMS,
                    maxIdleTime: ConfigurationManager.Current.maxIdleTimeMS,
                    targetPath: $"{QuestManager.GetLocalPath4Quest(Id)}{QuestManager.QUEST_FILE_NAME}"
                );
            var unused = Base.Instance.GetDownloadBehaviour(
                downloadGameXML,
                $"Lade {ConfigurationManager.Current.nameForQuestSg}"
            );

            // analyze game.xml, gather all media info compare to local media info and detect missing media
            var prepareMediaInfosToDownload =
                new PrepareMediaInfoList();
            var unused1 = Base.Instance.GetSimpleBehaviour(
                prepareMediaInfosToDownload,
                $"Synchronisiere {ConfigurationManager.Current.nameForQuestSg}-Daten",
                "Medien werden vorbereitet"
            );

            // download all missing media info
            var downloadMediaFiles =
                new MultiDownloader(
                    maxParallelDownloads: ConfigurationManager.Current.maxParallelDownloads,
                    timeout: ConfigurationManager.Current.timeoutMS
                );
            var unused2 = Base.Instance.GetSimpleBehaviour(
                downloadMediaFiles,
                $"Synchronisiere {ConfigurationManager.Current.nameForQuestSg}-Daten",
                "Mediendateien werden geladen"
            );
            downloadMediaFiles.OnTaskCompleted += (object sender, TaskEventArgs e) => { TimeStamp = ServerTimeStamp; };

            // store current media info locally
            var exportLocalMediaInfo =
                new ExportMediaInfoList();
            var unused3 = Base.Instance.GetSimpleBehaviour(
                exportLocalMediaInfo,
                $"Synchronisiere {ConfigurationManager.Current.nameForQuestSg}-Daten",
                "Medieninformationen werden lokal gespeichert"
            );

            var exportGlobalMediaJson =
                new ExportGlobalMediaJson();
            var unused5 = Base.Instance.GetSimpleBehaviour(
                exportGlobalMediaJson,
                $"Aktualisiere {ConfigurationManager.Current.nameForQuestsPl}",
                $"{ConfigurationManager.Current.nameForQuestSg}-Daten werden gespeichert"
            );

            var exportQuestsInfoJSON =
                new ExportQuestInfosToJSON();
            var unused4 = Base.Instance.GetSimpleBehaviour(
                exportQuestsInfoJSON,
                $"Aktualisiere {ConfigurationManager.Current.nameForQuestsPl}",
                $"{ConfigurationManager.Current.nameForQuestSg}-Daten werden gespeichert"
            );
            var t =
                new TaskSequence(
                    downloadGameXML,
                    prepareMediaInfosToDownload,
                    downloadMediaFiles,
                    exportLocalMediaInfo,
                    exportGlobalMediaJson,
                    exportQuestsInfoJSON);

            return t;
        }

        #endregion

        /// <summary>
        /// Updates the quest represented by this info, i.e. its content is replaced by the current server content. 
        /// It is assumed that this info already has a link to the new server version stored (cf. NewVersionOnServer property).
        /// Is called from the UI (Button e.g.).
        /// 
        /// Updating a local quest means three steps: 
        /// 
        /// 1. This info is replaced by the info of the new version (hence the list etc. in the foyer will be updated)
        /// 2. The represented quest game.xml is downloaded and replaces the old version.
        /// 3. All contained media is checked for update (new, updated, gone), cf. TODO... It is already implemented, but where?
        /// </summary>
        public void Update()
        {
            if (ActivitiesBlocking)
                return;

            // update the quest info:
            if (NewVersionOnServer != null)
            {
                //				QuestInfoManager.Instance.QuestDict.Add (data.Id, data.NewVersionOnServer); TODO
                var download = NewVersionOnServer.DownloadTask();
                download.OnTaskEnded += (object sender, TaskEventArgs e) => { ActivitiesBlocking = false; };

                // Update the quest info list ...
                download.OnTaskEnded +=
                    (object sender, TaskEventArgs e) =>
                    {
                        QuestContentHasBeenUpdated();
                        //QuestInfoManager.Instance.UpdateQuestInfoFromLocalQuest(NewVersionOnServer.Id);
                        new ExportQuestInfosToJSON().Start();
                    };

                ActivitiesBlocking = true;
                download.Start();
            }
        }

        /// <summary>
        /// Deletes the local quest represented by this info. Is called from the UI (Button e.g.).
        /// </summary>
        public void Delete()
        {
            if (ServerTimeStamp == null)
            {
                // this quest is not available on the server anymore ...
                if (!ConfigurationManager.Current.autoSynchQuestInfos)
                {
                    // in manual synch mode we warn the user to delete this quest, since he can not restore it again:
                    var dialog =
                        new CancelableFunctionDialog(
                            title: "Löschen?",
                            message: "Diese Quest können Sie nicht wieder herstellen, wenn Sie sie gelöscht haben.",
                            cancelableFunction: doDelete
                        );
                    dialog.Start();
                }
            }
            else
            {
                doDelete();
            }
        }

        private void doDelete()
        {
            // reduce media usage counter for each media used in this quest:
            var localMediaInfos = PrepareMediaInfoList.GetStoredLocalInfosFromJson(Id);
            Debug.Log($"DoDELETE #: {localMediaInfos.Count}");
            foreach (var mediaInfo in localMediaInfos)
            {
                QuestManager.Instance.DecreaseMediaUsage(mediaInfo.url);
            }
            
            Files.DeleteDirCompletely(QuestManager.GetLocalPath4Quest(Id));
            TimeStamp = null;

            if (ServerTimeStamp == null)
            {
                // delete this quest info completely when it is not even on the server anymore:
                QuestInfoManager.Instance.RemoveInfo(Id);
            }
            else
            {
                InvokeOnChanged();
            }
            
            var exportGlobalMediaJson =
                new ExportGlobalMediaJson();
            var unused5 = Base.Instance.GetSimpleBehaviour(
                exportGlobalMediaJson,
                $"Aktualisiere {ConfigurationManager.Current.nameForQuestsPl}",
                $"{ConfigurationManager.Current.nameForQuestSg}-Daten werden gespeichert"
            );

            var exportQuestsInfoJSON =
                new ExportQuestInfosToJSON();
            var unused = Base.Instance.GetSimpleBehaviour(
                exportQuestsInfoJSON,
                string.Format("Aktualisiere {0}", ConfigurationManager.Current.nameForQuestsPl),
                string.Format("{0}-Daten werden gespeichert", ConfigurationManager.Current.nameForQuestSg)
            );

            var t =
                new TaskSequence(
                    exportGlobalMediaJson,
                    exportQuestsInfoJSON);
            t.Start();
        }

        /// <summary>
        /// Starts the local quest represented by this info. Is called from the UI (Button e.g.).
        /// </summary>
        public void Play()
        {
            Debug.Log("QuestInfo.Play()");
            if (ActivitiesBlocking)
                return;

            // Close menu if open:
            Base.Instance.MenuCanvas.SetActive(false);

            if (!IsOnDevice && !IsOnServer)
            {
                Log.SignalErrorToAuthor("Unable to load missing quest with id {0} - not found.", Id);
                return;
            }

            Task playTask = null;

            if (!IsOnDevice && IsOnServer)
            {
                // TODO config flag for auto-loads or even auto-update??
                playTask = CreateLoadAndPlayTask();
            }

            // ------------------------------
            // from here on holds IsOnDevice:

            if (IsOnDevice && HasUpdate && ConfigurationManager.Current.autoUpdateSubquests)
            {
                playTask = CreateLoadAndPlayTask();
            }

            if (playTask == null)
                playTask = CreatePlayTask();

            playTask.OnTaskEnded += (object sender, TaskEventArgs e) => { ActivitiesBlocking = false; };

            ActivitiesBlocking = true;
            Debug.Log("QuestInfo.Play() before Start");

            playTask.Start();
            Debug.Log("QuestInfo.Play() after start");

        }

        private Task CreateLoadAndPlayTask()
        {
            // Quest has to be loaded first:
            var download = DownloadTask();
            // Update the quest info list ...
            download.OnTaskCompleted +=
                (object sender, TaskEventArgs e) =>
                {
                    InvokeOnChanged();

                    new ExportQuestInfosToJSON().Start();
                };
            var playTask = CreatePlayTask();
            Task loadAndPlay = new TaskSequence(download, playTask);
            return loadAndPlay;
        }

        /// <summary>
        /// Creates a task that just plays the locally existing quest, checks have to applied beforehand:
        /// </summary>
        /// <returns>The play.</returns>
        private Task CreatePlayTask()
        {
            // Load quest data: game.xml
            var loadGameXML =
                new LocalFileLoader(
                    filePath: QuestManager.GetLocalPath4Quest(Id) + QuestManager.QUEST_FILE_NAME
                );
            var unused = Base.Instance.GetDownloadBehaviour(
                loadGameXML,
                $"Lade {ConfigurationManager.Current.nameForQuestsPl}"
            );

            var questStarter = new QuestStarter();

            var t =
                new TaskSequence(loadGameXML, questStarter);

            return t;
        }

        #endregion
    }


    public class HotspotInfo
    {
        public HotspotInfo(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public static HotspotInfo NULL = new HotspotInfo(0f, 0f);
    }


    public class MetaDataInfo
    {
        public MetaDataInfo(string key, string val)
        {
            Key = key;
            Value = val;
        }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}