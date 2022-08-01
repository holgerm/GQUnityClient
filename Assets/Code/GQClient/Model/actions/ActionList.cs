using System.Collections.Generic;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;

namespace Code.GQClient.Model.actions
{
    public class ActionList : I_GQML {

		#region Structure 

		/// <summary>
		/// The contained actions.
		/// </summary>
		public List<Action> containedActions = new List<Action> ();

		public I_GQML Parent { get; set; }

		public Quest Quest {
			get
			{
				if (Parent == null) return null;
				
				return Parent.Quest;
			}
		}

		#endregion


		#region Functions

		public void Apply ()
		{
			foreach (var action in containedActions) {
				action.Execute ();
			}
		}

		#endregion



	}
}
