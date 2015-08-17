// 
// KeywordCollection.cs
// 
// Copyright (c) 2014-2015, Candlelight Interactive, LLC
// All rights reserved.
// 
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://download.unity3d.com/assetstore/customer-eula.pdf
// 
// This file contains a base class for keyword collections. These objects can
// be used with HyperText to automatically identify keywords and create links
// for them.

using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Candlelight
{
	/// <summary>
	/// An enum to specify whether or not the terms are case sensitive.
	/// </summary>
	public enum CaseMatchMode { CaseSensitive, IgnoreCase }
	/// <summary>
	/// Word prioritization for sorting keywords. LongestNGram will prioritize "some phrase" before either "some" or
	/// "phrase."
	/// </summary>
	public enum WordPrioritization { Default, LongestNGram }

	/// <summary>
	/// A base class for a collection of keywords for use with the HyperText component.
	/// </summary>
	public abstract class KeywordCollection : ScriptableObject
	{
		/// <summary>
		/// The regex to split n-grams.
		/// </summary>
		private static readonly Regex s_SplitNGramRegex = new Regex(@"\W+");

		#region Shared Allocations
		private List<string> m_KeywordList = new List<string>();
		#endregion

		#region Backing Fields
		[SerializeField, PropertyBackingField]
		private CaseMatchMode m_CaseMatchMode = CaseMatchMode.IgnoreCase;
		private ReadOnlyCollection<string> m_Keywords = null;
		private UnityEngine.Events.UnityEvent m_OnRebuildKeywords = new UnityEngine.Events.UnityEvent();
		[SerializeField, PropertyBackingField]
		private WordPrioritization m_WordPrioritization = WordPrioritization.LongestNGram;
		#endregion
		/// <summary>
		/// Gets or sets the case match mode.
		/// </summary>
		/// <value>The case match mode.</value>
		public CaseMatchMode CaseMatchMode
		{
			get { return m_CaseMatchMode; }
			set
			{
				if (m_CaseMatchMode != value)
				{
					m_CaseMatchMode = value;
					RebuildKeywords();
				}
			}
		}
		/// <summary>
		/// Gets or sets the keyword cache. All of the keywords are assumed to be in order of priority. For example, the
		/// unigram "dog" would be given priority over the bigram "the dog" if it appears at a lower index.
		/// </summary>
		/// <value>The keyword cache.</value>
		public ReadOnlyCollection<string> Keywords
		{
			get
			{
				if (m_Keywords == null)
				{
					RebuildKeywords();
				}
				return m_Keywords;
			}
		}
		/// <summary>
		/// Gets a callback for whenever keywords are rebuilt on this instance.
		/// </summary>
		/// <value>A callback for whenever keywords are rebuilt on this instance.</value>
		public UnityEngine.Events.UnityEvent OnRebuildKeywords
		{
			get
			{
				if (m_OnRebuildKeywords == null)
				{
					m_OnRebuildKeywords = new UnityEngine.Events.UnityEvent();
				}
				return m_OnRebuildKeywords;
			}
		}
		/// <summary>
		/// Gets or sets the prioritization mode for ordering keywords.
		/// </summary>
		/// <value>The prioritization mode.</value>
		public WordPrioritization WordPrioritization
		{
			get { return m_WordPrioritization; }
			set
			{
				if (m_WordPrioritization != value)
				{
					m_WordPrioritization = value;
					RebuildKeywords();
				}
			}
		}

		/// <summary>
		/// Raises the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			RebuildKeywords();
		}

		/// <summary>
		/// Populates the supplied keyword list.
		/// </summary>
		/// <param name="keywordList">An empty keyword list.</param>
		protected abstract void PopulateKeywordList(List<string> keywordList);

		/// <summary>
		/// Rebuilds the Keywords property.
		/// </summary>
		protected void RebuildKeywords()
		{
			m_KeywordList.Clear();
			PopulateKeywordList(m_KeywordList);
			SortAndFilterKeywordList();
			m_Keywords = new ReadOnlyCollection<string>(m_KeywordList.ToArray());
			OnRebuildKeywords.Invoke();
		}

		/// <summary>
		/// Apply the current sorting and filtering properties to the keyword list.
		/// </summary>
		protected void SortAndFilterKeywordList()
		{
			string[] keywordsCache = m_KeywordList.ToArray();
			m_KeywordList.Clear();
			bool ignoreCase = this.CaseMatchMode == CaseMatchMode.IgnoreCase;
			foreach (string keyword in keywordsCache)
			{
				if (string.IsNullOrEmpty(keyword))
				{
					continue;
				}
				if (ignoreCase && string.IsNullOrEmpty(m_KeywordList.Find(kw => kw == keyword.ToLower())))
				{
					m_KeywordList.Add(keyword.ToLower());
				}
				else if (!ignoreCase && !m_KeywordList.Contains(keyword))
				{
					m_KeywordList.Add(keyword);
				}
			}
			if (this.WordPrioritization == WordPrioritization.LongestNGram)
			{
				m_KeywordList.Sort(
					(kw1, kw2) =>
						-1 * s_SplitNGramRegex.Split(kw1).Length.CompareTo(s_SplitNGramRegex.Split(kw2).Length)
				);
			}
		}
	}
}