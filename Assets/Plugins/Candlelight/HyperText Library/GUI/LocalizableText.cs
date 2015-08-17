// 
// LocalizableText.cs
// 
// Copyright (c) 2015, Candlelight Interactive, LLC
// All rights reserved.
// 
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://download.unity3d.com/assetstore/customer-eula.pdf
// 
// This file contains a class for storing text values for different locales.

using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Candlelight.UI
{
	/// <summary>
	/// Localizable text.
	/// </summary>
	public class LocalizableText : ScriptableObject, ITextSource
	{
		/// <summary>
		/// A basic struct for storing a locale and its associated text.
		/// </summary>
		[System.Serializable]
		public struct LocalizedText : IPropertyBackingFieldCompatible
		{
			#region Backing Fields
			[SerializeField]
			private string m_Locale;
			[SerializeField, TextArea(3, 10)]
			private string m_Text;
			#endregion

			/// <summary>
			/// Gets the locale.
			/// </summary>
			/// <value>The locale.</value>
			public string Locale { get { return m_Locale; } }
			/// <summary>
			/// Gets the text.
			/// </summary>
			/// <value>The text.</value>
			public string Text { get { return m_Text; } }

			/// <summary>
			/// Initializes a new instance of the <see cref="Candlelight.UI.LocalizableText+LocalizedText"/> struct.
			/// </summary>
			/// <param name="locale">Locale.</param>
			/// <param name="text">Text.</param>
			public LocalizedText(string locale, string text)
			{
				m_Locale = locale;
				m_Text = text;
			}

			/// <summary>
			/// Clone this instance.
			/// </summary>
			public object Clone()
			{
				return this;
			}

			/// <summary>
			/// Gets a hash value that is based on the values of the serialized properties of this instance.
			/// </summary>
			/// <remarks>
			/// Note that any reference type fields should implement and test with this interface; IList fields should
			/// generate a value-based hash.
			/// </remarks>
			/// <returns>A hash value based on the values of the serialized properties on this instance.</returns>
			public int GetSerializedPropertiesHash()
			{
				return GetHashCode();
			}
		}

		/// <summary>
		/// Default locale string.
		/// </summary>
		private static readonly string s_DefaultLocale = "[DEFAULT]";
		#region Backing Fields
		private static readonly ReadOnlyCollection<string> s_TenMostCommonLanguages = new ReadOnlyCollection<string>(
			new string[] { "en", "ja", "ko", "zh", "de", "fr", "pt", "es", "it", "ms" }
		);
		#endregion

		/// <summary>
		/// Gets locale strings for the ten most common languages for mobile apps according to
		/// http://todaysweb.net/top-mobile-apps-games-localization-languages-maximum-revenue/
		/// </summary>
		/// <value>The ten most common languages for mobile apps.</value>
		public static ReadOnlyCollection<string> TenMostCommonLanguages { get { return s_TenMostCommonLanguages; } }

		#region Backing Fields
		[SerializeField, PropertyBackingField(typeof(PopupAttribute), "GetCurrentLocalePopupContents")]
		private string m_CurrentLocale = s_DefaultLocale;
		[SerializeField, PropertyBackingField(typeof(TextAreaAttribute), 3, 10)]
		private string m_DefaultText = "";
		[SerializeField, PropertyBackingField(typeof(FlushChildrenAttribute))]
		private List<LocalizedText> m_LocalizedText = new List<LocalizedText>();
		private UnityEngine.Events.UnityEvent m_OnBecameDirty = new UnityEngine.Events.UnityEvent();
		#endregion

		/// <summary>
		/// Gets or sets the current locale.
		/// </summary>
		/// <value>The current locale.</value>
		public string CurrentLocale
		{
			get { return m_CurrentLocale; }
			set
			{
				value = value ?? "";
				if (m_CurrentLocale != value)
				{
					m_CurrentLocale = value;
					m_OnBecameDirty.Invoke();
				}
			}
		}
		/// <summary>
		/// Gets or sets the default text.
		/// </summary>
		/// <value>The default text.</value>
		public string DefaultText
		{
			get { return m_DefaultText; }
			set
			{
				value = value ?? "";
				if (m_DefaultText != value)
				{
					m_DefaultText = value;
					m_OnBecameDirty.Invoke();
				}
			}
		}
		/// <summary>
		/// Gets a callback for whenever the text on this instance has changed.
		/// </summary>
		/// <value>A callback for whenever the text on this instance has changed.</value>
		public UnityEngine.Events.UnityEvent OnBecameDirty { get { return m_OnBecameDirty; } }
		/// <summary>
		/// Gets the output text.
		/// </summary>
		/// <value>The output text.</value>
		public string OutputText
		{
			get
			{
				int index = m_LocalizedText.FindIndex(k => k.Locale == m_CurrentLocale);
				return index < 0 ? m_DefaultText : m_LocalizedText[index].Text;
			}
		}

		/// <summary>
		/// Gets the current locale popup contents. Included for inspector.
		/// </summary>
		/// <returns>The current locale popup contents.</returns>
		/// <param name="labels">Labels.</param>
		/// <param name="values">Values.</param>
		private int GetCurrentLocalePopupContents(List<GUIContent> labels, List<object> values)
		{
			labels.Clear();
			values.Clear();
			int currentIndex = -1;
			for (int i = m_LocalizedText.Count - 1; i >= 0; --i)
			{
				labels.Add(new GUIContent(m_LocalizedText[i].Locale));
				values.Add(m_LocalizedText[i].Locale);
				currentIndex = m_LocalizedText[i].Locale == m_CurrentLocale ? i : currentIndex;
			}
			labels.Add(new GUIContent(s_DefaultLocale));
			values.Add(s_DefaultLocale);
			++currentIndex;
			labels.Reverse();
			values.Reverse();
			return currentIndex;
		}

		/// <summary>
		/// Gets the localized text.
		/// </summary>
		/// <returns>The localized text.</returns>
		public LocalizedText[] GetLocalizedText()
		{
			return m_LocalizedText.ToArray();
		}

		/// <summary>
		/// Gets the localized text.
		/// </summary>
		/// <param name="localizedText">Localized text list to populate.</param>
		public void GetLocalizedText(ref List<LocalizedText> localizedText)
		{
			localizedText = localizedText ?? new List<LocalizedText>(m_LocalizedText.Count);
			localizedText.Clear();
			localizedText.AddRange(m_LocalizedText);
		}

		/// <summary>
		/// Sets the localized text. Included for inspector.
		/// </summary>
		/// <param name="value">Value.</param>
		private void SetLocalizedText(LocalizedText[] value)
		{
			SetLocalizedText(value as IEnumerable<LocalizedText>);
		}

		/// <summary>
		/// Sets the localized text.
		/// </summary>
		/// <param name="value">Value.</param>
		public void SetLocalizedText(IEnumerable<LocalizedText> value)
		{
			value = value ?? new LocalizedText[0] as IEnumerable<LocalizedText>;
			if (value.Count() != m_LocalizedText.Count || !value.SequenceEqual(m_LocalizedText))
			{
				m_LocalizedText.Clear();
				m_LocalizedText.AddRange(value);
				m_OnBecameDirty.Invoke();
			}
		}
	}
}