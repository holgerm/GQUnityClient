using System;
using System.Collections;
using System.Collections.Generic;
using GQ.Client.Err;
using UnityEngine;

namespace GQ.Client.Model
{
    /// <summary>
    /// Quest History records all relevant events within the according quest, e.g. page starts.
    /// 
    /// Quest history will server three purposes:
    /// 
    /// 1. Back function
    /// 2. Resume function (for this reason it has to be persisted)
    /// 3. Analysis data (that can be sent to a server) 
    /// 
    /// </summary>
    public class QuestHistory
    {
        protected Quest quest;

        public QuestHistory(Quest quest)
        {
            this.quest = quest;
            entryList = new List<HistoryEntry>();
            pageBackStack = new Stack<int>();
        }

        internal readonly List<HistoryEntry> entryList;
        internal readonly Stack<int> pageBackStack;

        #region Public API
        internal void Record(HistoryEntry entry)
        {
            entryList.Add(entry);
            entry.getRecorded(this);
        }

        public bool CanGoBackToPreviousPage
        {
            get
            {
                return pageBackStack.Count > 1;
            }
        }

        /// <summary>
        /// Goes back to the previous page if possible, i.e. if a page is on back stack.
        /// </summary>
        /// <returns>The back to previous page.</returns>
        public int GoBackToPreviousPage()
        {
            int id = -1;
            int sizeBeforeException = -1;
            try
            {
                sizeBeforeException = pageBackStack.Count;
                // we leave the current page, hence we pop it from back stack:
                pageBackStack.Pop();

                // we go back to the previous page, wich is the on top of the back stack now:
                id = pageBackStack.Peek();
                Page pageToStart = quest.GetPageWithID(id);



                // if we still have a back page on stack, we can allow to go back further:
                pageToStart.Start(CanGoBackToPreviousPage);
            }
            catch (InvalidOperationException)
            {
                Log.SignalErrorToDeveloper(
                    "Tried to go back one page when no pages were on history stack (on page {0} in quest {1}). Stack size before pop() was: {2}.", 
                    quest.CurrentPage.Id, quest.Id, sizeBeforeException.ToString());
            }
            return id;
        }
        #endregion

    }

    internal abstract class HistoryEntry
    {
        readonly DateTime time;

        protected HistoryEntry()
        {
            time = DateTime.Now;
        }

        public abstract void getRecorded(QuestHistory history);
    }

    internal class PageStarted : HistoryEntry
    {
        readonly int pageId;
        protected bool allowReturn;

        public PageStarted(Page page, bool allowReturn) : base()
        {
            this.pageId = page.Id;
            this.allowReturn = allowReturn;
        }

        public override void getRecorded(QuestHistory history)
        {
            if (!allowReturn)
            {
                history.pageBackStack.Clear();
            }

            if (history.pageBackStack.Count == 0 || history.pageBackStack.Peek() != pageId)
            {
                // if this page is already on top of the back stack, we ignore its start:
                history.pageBackStack.Push(pageId);
            }
        }

        public override string ToString()
        {
            return string.Format("PageStarted ({0})", pageId);
        }
    }

}
