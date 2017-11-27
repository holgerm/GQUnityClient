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

	public abstract class QuestInfoFilter
	{

		abstract public bool Accept (QuestInfo qi);

		abstract public List<string> AcceptedCategories(QuestInfo qi);

		public string CategoryToShow (QuestInfo qi) {
			return AcceptedCategories(qi).Count > 0 ? AcceptedCategories(qi)[0] : QuestInfo.WITHOUT_CATEGORY_ID;
		}


		public class All : QuestInfoFilter
		{

			public override bool Accept (QuestInfo qi)
			{
				return true;
			}

			public override string ToString ()
			{
				return "All";
			}

			public override List<string> AcceptedCategories (QuestInfo qi) {
				return qi.Categories;
			}
		}

		public class Category : QuestInfoFilter {

			private List<string> acceptedCategories = new List<string>();

			public Category(params string[] categories) {
				acceptedCategories.AddRange(categories);
			}

			/// <summary>
			/// Accepts the given quest info when at least one of the quests categories is mentioned as accepted category of this filter.
			/// </summary>
			/// <param name="qi">Quest Info.</param>
			public override bool Accept (QuestInfo qi)
			{
				foreach (string cat in acceptedCategories) {
					if (qi.Categories.Contains (cat))
						return true;
				}
				return false;
			}

			public override string ToString ()
			{
				StringBuilder sb = new StringBuilder ("Category is in {");
				for (int i = 0; i < acceptedCategories.Count; i++) {
					sb.Append (acceptedCategories[i]);
					if (i+1 < acceptedCategories.Count) {
						sb.Append (", ");
					}
				}
				sb.Append ("}");
				return sb.ToString ();
			}

			public override List<string> AcceptedCategories (QuestInfo qi) {
				List<string> accCats = new List<string> ();
				foreach (string cat in acceptedCategories) {
					if (qi.Categories.Contains (cat))
						accCats.Add (cat);
				}
				return accCats;
			}
		}

		public abstract class Multi : QuestInfoFilter {
			protected List<QuestInfoFilter> subfilters = new List<QuestInfoFilter>();
		}


		public class And : Multi
		{

			public And (params QuestInfoFilter[] filters)
			{
				subfilters.AddRange (filters);
			}

			public override bool Accept (QuestInfo qi)
			{
				bool accepted = true;

				foreach (QuestInfoFilter filter in subfilters) {
					accepted &= filter.Accept (qi);
				}

				return accepted;
			}

			public override string ToString ()
			{
				StringBuilder sb = new StringBuilder ("And(");

				foreach (QuestInfoFilter sel in subfilters) {
					sb.Append (sel.ToString ());
				}

				sb.Append (")");

				return sb.ToString ();
			}

			public override List<string> AcceptedCategories (QuestInfo qi) {
				// if we have no filters we return all categories:
				if (subfilters == null || subfilters.Count == 0)
					return qi.Categories;

				List<string> acceptedCategories = new List<string>();

				if (!Accept (qi))
					return acceptedCategories;

				// the qi is accepted, so we also accept any categories to represent it:
				for (int j = 0; j < subfilters.Count; j++) {
					for (int i = 0; i < subfilters [j].AcceptedCategories (qi).Count; i++) {
						if (!acceptedCategories.Contains(subfilters [j].AcceptedCategories (qi)[i])) {
							acceptedCategories.Add (subfilters [j].AcceptedCategories (qi) [i]);
						}
					}
				}
				return acceptedCategories;
			}
		}

		public class Or : Multi
		{

			public Or (params QuestInfoFilter[] filters)
			{
				subfilters.AddRange (filters);
			}

			public override bool Accept (QuestInfo qi)
			{
				bool accepted = false;

				foreach (QuestInfoFilter filter in subfilters) {
					accepted |= filter.Accept (qi);
				}

				return accepted;
			}

			public override string ToString ()
			{
				StringBuilder sb = new StringBuilder ("Or(");

				foreach (QuestInfoFilter sel in subfilters) {
					sb.Append (sel.ToString ());
				}

				sb.Append (")");

				return sb.ToString ();
			}

			public override List<string> AcceptedCategories (QuestInfo qi) {
				// if we have no filters we return all categories:
				if (subfilters == null || subfilters.Count == 0)
					return qi.Categories;

				// all categories which are accepted by any of the filters shoud be contained:
				List<string> acceptedCategories = new List<string>();
				for (int j = 0; j < subfilters.Count; j++) {
					for (int i = 0; i < subfilters [j].AcceptedCategories (qi).Count; i++) {
						if (!acceptedCategories.Contains(subfilters [j].AcceptedCategories (qi)[i])) {
							acceptedCategories.Add (subfilters [j].AcceptedCategories (qi) [i]);
						}
					}
				}
				return acceptedCategories;
			}
		}
	}
}
