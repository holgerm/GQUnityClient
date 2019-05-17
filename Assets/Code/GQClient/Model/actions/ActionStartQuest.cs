using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using GQ.Client.Err;
using System;
using System.Globalization;

namespace GQ.Client.Model
{

    public class ActionStartQuest : ActionAbstract
    {

        #region Structure
        protected string questIdString;

        protected override void ReadAttributes(XmlReader reader)
        {
            questIdString = GQML.GetStringAttribute(GQML.ACTION_STARTQUEST_QUEST, reader);

            // If the quest is given explicitly with id, add it to the subquests of this quest info:
            int questId;
            if (Int32.TryParse(questIdString, System.Globalization.NumberStyles.None, CultureInfo.CurrentCulture, out questId))
            {
                QuestInfo qiForThisQuest = QuestInfoManager.Instance.GetQuestInfo(QuestManager.CurrentlyParsingQuest.Id);
                qiForThisQuest.AddSubQuest(questId);
            }
        }
        #endregion


        #region Functions

        public override void Execute()
        {
            // we evaluate the questId that we stored as raw string just in time, 
            // so if it is given as variable its value can change as long as possible:

            // try to convert questID to int directly:
            int questId;
            if (!Int32.TryParse(questIdString, System.Globalization.NumberStyles.None, CultureInfo.CurrentCulture, out questId))
            {
                // we accept also variable names here and try to evaluate:
                Value questIdValueByVar = Variables.GetValue(questIdString);
                if (questIdValueByVar == Value.Null)
                {
                    // we can neither interpret the questId as integer nor as variable. So we give up and ignore the action:
                    Log.SignalErrorToAuthor(
                        "StartQuest Action can not be executed since questID was set to {0} which we can not use.",
                        questIdString);
                }
                else
                {
                    questId = questIdValueByVar.AsInt();
                }
            }

            // prepare other quest to start:
            QuestInfo qi = QuestInfoManager.Instance.GetQuestInfo(questId);

            Quest.End(clearAlsoUpperCaseVariables: false);

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
