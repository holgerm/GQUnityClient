using System;
using System.Globalization;
using System.Xml;
using Code.GQClient.Err;
using Code.GQClient.Model.expressions;
using Code.GQClient.Model.gqml;
using GQClient.Model;
using Code.GQClient.Model.mgmt.quests;

namespace Code.GQClient.Model.actions
{

    public class ActionStartQuest : Action

    {

        #region Structure
        public ActionStartQuest(XmlReader reader) : base(reader) { }

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
                qi.Play();
            }
        }
        #endregion
    }
}
