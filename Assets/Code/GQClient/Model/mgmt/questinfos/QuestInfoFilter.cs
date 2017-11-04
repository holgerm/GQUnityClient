using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using GQ.Client.Util;
using GQ.Client.Conf;
using GQ.Client.Model;
using Newtonsoft.Json;
using System.IO;
using GQ.Client.Err;
using GQ.Client.UI.Dialogs;
using System.Text;


namespace GQ.Client.Model
{

	public interface QuestInfoFilter
	{

		bool accept (QuestInfo qi);

	}


	public class AllQuests : QuestInfoFilter
	{

		public bool accept (QuestInfo qi)
		{
			return true;
		}

		public override string ToString ()
		{
			return "All";
		}
	}


	public class AndFilter : QuestInfoFilter
	{

		List<QuestInfoFilter> selectors;

		public AndFilter (params QuestInfoFilter[] selector)
		{
			selectors.AddRange (selector);
		}

		public bool accept (QuestInfo qi)
		{
			bool accepted = true;

			foreach (QuestInfoFilter filter in selectors) {
				accepted &= filter.accept (qi);
			}

			return accepted;
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder ("And(");

			foreach (QuestInfoFilter sel in selectors) {
				sb.Append (sel.ToString ());
			}

			sb.Append (")");

			return sb.ToString ();
		}
	}

	public class OrFilter : QuestInfoFilter
	{

		List<QuestInfoFilter> selectors;

		public OrFilter (params QuestInfoFilter[] selector)
		{
			selectors.AddRange (selector);
		}

		public bool accept (QuestInfo qi)
		{
			bool accepted = false;

			foreach (QuestInfoFilter filter in selectors) {
				accepted |= filter.accept (qi);
			}

			return accepted;
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder ("Or(");

			foreach (QuestInfoFilter sel in selectors) {
				sb.Append (sel.ToString ());
			}

			sb.Append (")");

			return sb.ToString ();
		}
	}
}
