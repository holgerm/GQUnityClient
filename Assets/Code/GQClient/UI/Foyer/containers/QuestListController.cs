using UnityEngine;
using System.Collections;
using GQ.Client.Model;
using System.Collections.Generic;
using System;
using GQ.Client.UI.Dialogs;
using GQ.Client.Util;
using GQ.Client.Conf;
using UnityEngine.UI;
using GQ.Client.Err;
using QM.Util;

namespace GQ.Client.UI.Foyer
{

    /// <summary>
    /// Shows all Quest Info objects, e.g. in a scrollable list within the foyer. Drives a dialog while refreshing its content.
    /// </summary>
    public class QuestListController : QuestContainerController
    {
        public Transform InfoList;
        public GameObject HiddenQuests;


        #region React on Events

        public override void OnQuestInfoChanged(object sender, QuestInfoChangedEvent e)
        {
            Debug.Log("QuestListController.OnQuestInfoChanged e.type: " + e.ChangeType.ToString());
            QuestInfoUIC qiCtrl;
            switch (e.ChangeType)
            {
                case ChangeType.AddedInfo:
                    qiCtrl =
                        QuestInfoUICListElement.Create(
                        root: InfoList.gameObject,
                        qInfo: e.NewQuestInfo,
                        containerController: this
                    ).GetComponent<QuestInfoUICListElement>();
                    QuestInfoControllers.Add(e.NewQuestInfo.Id, qiCtrl);
                    qiCtrl.Show();
                    updateListSorting();
                    break;
                case ChangeType.ChangedInfo:
                    //				if (e.OldQuestInfo == null || !QuestInfoControllers.TryGetValue (e.OldQuestInfo.Id, out qiCtrl)) {
                    //					Log.SignalErrorToDeveloper (
                    //						"Quest Info Controller for quest id {0} not found when a Change event occurred.",
                    //						e.OldQuestInfo.Id
                    //					);
                    //					break;
                    //				}
                    if (e.NewQuestInfo == null || !QuestInfoControllers.TryGetValue(e.NewQuestInfo.Id, out qiCtrl))
                    {
                        Log.SignalErrorToDeveloper(
                            "Quest Info Controller for quest id {0} not found when a Change event occurred.",
                            e.NewQuestInfo.Id
                        );
                        break;
                    }
                    //				if (e.OldQuestInfo.Id != e.NewQuestInfo.Id) {
                    //					Log.SignalErrorToDeveloper (
                    //						"Quest Info Controller for quest id {0} got an update that changed the id to {1} which is not allowed and will be ignored.",
                    //						e.NewQuestInfo.Id, e.NewQuestInfo.Id
                    //					);
                    //					break;
                    //				}
                    qiCtrl.UpdateData(e.NewQuestInfo);
                    qiCtrl.Show();
                    updateListSorting();
                    break;
                case ChangeType.RemovedInfo:
                    if (!QuestInfoControllers.TryGetValue(e.OldQuestInfo.Id, out qiCtrl))
                    {
                        Log.SignalErrorToDeveloper(
                            "Quest Info Controller for quest id {0} not found when a Remove event occurred.",
                            e.OldQuestInfo.Id
                        );
                        break;
                    }
                    qiCtrl.Hide();
                    QuestInfoControllers.Remove(e.OldQuestInfo.Id);
                    updateElementOrderLayout();
                    break;
                case ChangeType.ListChanged:
                    RegenerateAll();
                    break;
                case ChangeType.FilterChanged:
                    RegenerateAllAfterFilterChanged();
                    break;
                case ChangeType.SorterChanged:
                    updateListSorting();
                    break;
            }
        }

        /// <summary>
        /// Sorts the list. Takes the current sorter into account to move the gameobjects in the right order.
        /// </summary>
        private void updateListSorting()
        {
            Base.Instance.StartCoroutine(sortViewAsCoroutine());
        }

        private IEnumerator sortViewAsCoroutine()
        {

            List<QuestInfoUIC> qcList = new List<QuestInfoUIC>(QuestInfoControllers.Values);
            qcList.Sort();

            for (int i = 0; i < qcList.Count; i++)
            {
                qcList[i].transform.SetSiblingIndex(i);

                if (i % 5 == 0)
                    yield return null;
            }

            updateElementOrderLayout();
        }

        /// <summary>
        /// Updates the view.
        /// </summary>
        public override void RegenerateAll()
        {
            Base.Instance.StartCoroutine(regenerateAllAsCoroutine());
        }

        /// <summary>
        /// Updates the view in coroutine mode. 
        /// 
        /// What happens is: 
        /// First all quest infos are deleted and the internal list is cleared. 
        /// Collect all filtered quest infos from the QuestInfoManager and create new controls for each.
        /// Sort the list according to the current sorting settings.
        /// 
        /// </summary>
        /// <returns>The view as coroutine.</returns>
		private IEnumerator regenerateAllAsCoroutine()
        {
            if (this == null)
            {
                yield break;
            }
            if (InfoList == null)
            {
                yield break;
            }

            // hide and delete all list elements:
            foreach (KeyValuePair<int, QuestInfoUIC> kvp in QuestInfoControllers)
            {
                kvp.Value.Hide();
                kvp.Value.Destroy();
            }

            QuestInfoControllers.Clear();

            //int steps = 0;
            foreach (QuestInfo info in QuestInfoManager.Instance.GetFilteredQuestInfos())
            {
                // create new list elements
                QuestInfoUICListElement qiCtrl =
                    QuestInfoUICListElement.Create(
                        root: InfoList.gameObject,
                        qInfo: info,
                        containerController: this
                    ).GetComponent<QuestInfoUICListElement>();
                QuestInfoControllers[info.Id] = qiCtrl;
                qiCtrl.Show();

                //if (steps % 3 == 0)
                //{
                //    yield return null;
                //    steps = 0;
                //}
            }

            updateListSorting();
        }

        public void RegenerateAllAfterFilterChanged()
        {
            if (this == null)
            {
                return;
            }
            if (InfoList == null)
            {
                return;
            }

            // we make a separate list of ids of all old quest infos:
            List<int> rememberedOldIDs = new List<int>(QuestInfoControllers.Keys);

            // we create new qi elements and keep those we can reuse. We remove those from our helper list.
            foreach (QuestInfo info in QuestInfoManager.Instance.GetFilteredQuestInfos())
            {
                QuestInfoUIC qiCtrl;
                if (QuestInfoControllers.TryGetValue(info.Id, out qiCtrl))
                {
                    qiCtrl.Show(); // why do we need to show them here again? Aren't they still shown? Why?
                                   // this new element was already there, hence we keep it:
                    rememberedOldIDs.Remove(info.Id);
                }
                else
                {
                    // Create not yet created qi controller, 
                    // e.g. after starting with this qi filtered out and changed the filter
                    // so we see it now for the first time.
                    QuestInfoUICListElement missingQICtrl =
                        QuestInfoUICListElement.Create(
                            root: InfoList.gameObject,
                            qInfo: info,
                            containerController: this
                        ).GetComponent<QuestInfoUICListElement>();
                    QuestInfoControllers[info.Id] = missingQICtrl;
                    QuestInfoControllers[info.Id].Show();
                }
            }

            // now in the helper list only the old unused elements are left. Hence we delete them:
            foreach (int oldID in rememberedOldIDs)
            {
                QuestInfoControllers[oldID].Hide();
            }

            updateElementOrderLayout();
            updateListSorting();
        }


        private void updateElementOrderLayout()
        {
            if (ConfigurationManager.Current.listEntryDividingMode != ListEntryDividingMode.AlternatingColors)
                return;
                
            for (int i = 0; i < InfoList.childCount; i++)
            {
                QuestInfoUIC qic = InfoList.GetChild(i).GetComponent<QuestInfoUIC>();
                Color bgCol = i % 2 == 0 ?
                    ConfigurationManager.Current.listEntryBgColor :
                    ConfigurationManager.Current.listEntrySecondBgColor;
                Color fgCol = i % 2 == 0 ?
                    ConfigurationManager.Current.listEntryFgColor :
                    ConfigurationManager.Current.listEntrySecondFgColor;


                qic.gameObject.GetComponent<Image>().color = bgCol;
                FoyerListLayoutConfig.SetQuestInfoEntryLayout(qic.gameObject, "InfoButton", sizeScaleFactor: 0.65f, fgColor: fgCol);
                qic.transform.Find("InfoButton/Image").GetComponent<Image>().color = fgCol;
                FoyerListLayoutConfig.SetQuestInfoEntryLayout(qic.gameObject, "Name", fgColor: fgCol);
                FoyerListLayoutConfig.SetQuestInfoEntryLayout(qic.gameObject, "DownloadButton", fgColor: fgCol);
                FoyerListLayoutConfig.SetQuestInfoEntryLayout(qic.gameObject, "StartButton", fgColor: fgCol);
                FoyerListLayoutConfig.SetQuestInfoEntryLayout(qic.gameObject, "DeleteButton", fgColor: fgCol);
                FoyerListLayoutConfig.SetQuestInfoEntryLayout(qic.gameObject, "UpdateButton", fgColor: fgCol);
            }
        }

        /// <summary>
        /// Assumes no element is new and no element has been removed, but their state or the context for showing them has changed.
        /// </summary>
        public override void UpdateElementViews()
        {
            foreach (KeyValuePair<int, QuestInfoUIC> kvp in QuestInfoControllers)
            {
                kvp.Value.UpdateView();
            }
        }



        #endregion

    }
}