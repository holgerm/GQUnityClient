using System.Collections.Generic;
using System.Xml;
using Code.GQClient.Err;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;

namespace Code.GQClient.Model.conditions
{

    [System.Serializable]
	public class CompoundCondition : ICondition, IConditionContainer
	{
		#region Structure
		public I_GQML Parent { get; set; }

		public Quest Quest {
			get {
				return Parent.Quest;
			}
		}

		/// <summary>
		/// The contained conditions. In case of the outer most "condition" tag, it will contain only one subcondition. 
		/// But in general (within And or or conditions) it might contain many subconditions.
		/// </summary>
		protected List<ICondition> containedConditions = new List<ICondition> ();

		/// <summary>
		/// Reader is at the condition element when we call this method. 
		/// Reads the xml within a given condition element until it has consumed all elements witihn the condition including the ending element.
		/// </summary>
		public CompoundCondition(XmlReader reader)
		{
			if (reader.IsEmptyElement) {
				reader.Read ();
				return;
			}

			string conditionName = reader.LocalName;

			// consume the begin of this compound condition
			reader.Read ();

			// and start reading the contained conditions:
			while (!GQML.IsReaderAtEnd (reader, conditionName)) {
				// if we reach the end of this condition element we are ready to leave this method.
				if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals (conditionName)) {
					break;
				}
					
				if (reader.NodeType != XmlNodeType.Element)
					continue;

				switch (reader.LocalName) {
				// COMPOUND CONDITIONS:
				case GQML.AND:
					containedConditions.Add (new ConditionAnd(reader));
					break;
				case GQML.OR:
					containedConditions.Add (new ConditionOr(reader));
					break;
				case GQML.NOT:
					containedConditions.Add (new ConditionNot(reader));
					break;
				// COMPARING CONDITIONS:
				case GQML.EQUAL:
					containedConditions.Add (new ConditionEq(reader));
					break;
				case GQML.GREATER_THAN:
					containedConditions.Add (new ConditionGt(reader));
					break;
				case GQML.GREATER_EQUAL:
					containedConditions.Add (new ConditionGeq(reader));
					break;
				case GQML.LESS_THAN:
					containedConditions.Add (new ConditionLt(reader));
					break;
				case GQML.LESS_EQUAL:
					containedConditions.Add (new ConditionLeq(reader));
					break;
				// UNKOWN CASE:
				default:
					Log.SignalErrorToDeveloper ("Unknown condition type found: " + reader.LocalName);
					break;
				}
			}

			// consume end element of this compound condition:
			reader.Read ();
		}

		#endregion


		#region Function

		public virtual bool IsFulfilled ()
		{
			bool allFulfilled = true;
			foreach (ICondition condition in containedConditions) {
				allFulfilled &= condition.IsFulfilled ();
				if (!allFulfilled)
					break;
			}
			return allFulfilled;
		}

		#endregion
	}
}