// 
// KeywordsGlossary.cs
// 
// Copyright (c) 2014, Candlelight Interactive, LLC
// All rights reserved.
// 
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://download.unity3d.com/assetstore/customer-eula.pdf
// 
// This file contains an advanced KeywordCollection class for use with
// HyperText. It allows for the creation of a basic keyword database containing
// alternate forms and definitions for keywords.

using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Candlelight
{
	/// <summary>
	/// Keywords glossary.
	/// </summary>
	public class KeywordsGlossary : KeywordCollection
	{
		/// <summary>
		/// A possible part of speech for a word.
		/// </summary>
		public enum PartOfSpeech
		{
			Noun,
			Verb,
			Adjective,
			Adverb
		}

		/// <summary>
		/// An inflected form for a word.
		/// </summary>
		[System.Serializable]
		public struct InflectedForm
		{
			#region Backing Fields
			[SerializeField]
			private PartOfSpeech m_PartOfSpeech;
			[SerializeField]
			private string m_Word;
			#endregion
			/// <summary>
			/// Gets the part of speech.
			/// </summary>
			/// <value>The part of speech.</value>
			public PartOfSpeech PartOfSpeech { get { return m_PartOfSpeech; } }
			/// <summary>
			/// Gets the word.
			/// </summary>
			/// <value>The word.</value>
			public string Word { get { return m_Word; } }
		}

		/// <summary>
		/// A glossary entry consisting of a main form, definition, and other inflected forms.
		/// </summary>
		[System.Serializable]
		public class Entry
		{
			#region Backing Fields
			[SerializeField, TextArea(5, 5)]
			private string m_Definition;
			[SerializeField]
			InflectedForm m_MainForm;
			[SerializeField]
			private List<InflectedForm> m_OtherForms;
			#endregion
			/// <summary>
			/// Gets the definition.
			/// </summary>
			/// <value>The definition.</value>
			public string Definition { get { return m_Definition; } }
			/// <summary>
			/// Gets the main form.
			/// </summary>
			/// <value>The main form.</value>
			public InflectedForm MainForm { get { return m_MainForm; } }
			/// <summary>
			/// Gets the other forms.
			/// </summary>
			/// <value>The other forms.</value>
			public InflectedForm[] OtherForms { get { return m_OtherForms.ToArray(); } }
		}

		#region Backing Fields
		[SerializeField, HideInInspector]
		private List<Entry> m_Entries = new List<Entry>();
		#endregion

		/// <summary>
		/// Gets the entries.
		/// </summary>
		/// <value>The entries.</value>
		public Entry[] Entries { get { return m_Entries.ToArray(); } }

		/// <summary>
		/// Gets the entry for the specified keyword if one exists.
		/// </summary>
		/// <returns>The entry.</returns>
		/// <param name="keyword">Keyword.</param>
		public Entry GetEntry(string keyword)
		{
			if (CaseMatchMode == CaseMatchMode.IgnoreCase)
			{
				keyword = keyword.ToLower();
				return m_Entries.Where(
					entry => entry.MainForm.Word.ToLower() == keyword ||
					entry.OtherForms.Where(syn => syn.Word.ToLower() == keyword).Count() > 0
				).FirstOrDefault();
			}
			else
			{
				return m_Entries.Where(
					entry => entry.MainForm.Word == keyword ||
					entry.OtherForms.Where(syn => syn.Word == keyword).Count() > 0
				).FirstOrDefault();
			}
		}

		/// <summary>
		/// Populates the supplied keyword list.
		/// </summary>
		/// <param name="keywordList">An empty keyword list.</param>
		protected override void PopulateKeywordList (List<string> keywordList)
		{
			foreach (Entry entry in m_Entries)
			{
				keywordList.Add(entry.MainForm.Word);
				keywordList.AddRange(from form in entry.OtherForms select form.Word);
			}
		}
	}
}