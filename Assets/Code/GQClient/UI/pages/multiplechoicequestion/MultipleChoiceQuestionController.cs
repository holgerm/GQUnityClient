using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GQ.Client.Model;

namespace GQ.Client.UI
{

	public class MultipleChoiceQuestionController : PageController
	{

		#region Runtime API

		protected PageMultipleChoiceQuestion mcqPage;

		/// <summary>
		/// Is called during Start() of the base class, which is a MonoBehaviour.
		/// </summary>
		public override void Initialize ()
		{
			mcqPage = (PageMultipleChoiceQuestion)page;

			// show the content:
//			ShowImage ();
//			ClearText ();
//			AddCurrentText ();
//			UpdateForwardButton ();
		}

		#endregion

	}
}
