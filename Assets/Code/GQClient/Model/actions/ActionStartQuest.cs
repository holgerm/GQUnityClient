using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using GQ.Client.Err;

namespace GQ.Client.Model
{

    public class ActionStartQuest : ActionAbstract
    {

        #region Structure
        protected int questId;

        protected override void ReadAttributes(XmlReader reader)
        {
            questId = GQML.GetIntAttribute(GQML.ACTION_STARTQUEST_QUEST, reader);
            QuestManager.CurrentlyParsingQuest.AddDependeeQuest(questId);
        }
        #endregion


        #region Functions

        public override void Execute()
        {
            // prepare other quest to start:
            QuestInfo qi = QuestInfoManager.Instance.GetQuestInfo(questId);

            Quest.End();

            if (qi == null)
            {
                Log.SignalErrorToAuthor("Unable to start quest with id {0} - not found.", questId);
            }
            else
            {
                qi.Play().Start();
            }
        }

        #endregion

    }
}
