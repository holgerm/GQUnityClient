// 
// RichText.cs
// 
// Copyright (c) 2014, Candlelight Interactive, LLC
// All rights reserved.
// 
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://download.unity3d.com/assetstore/customer-eula.pdf
// 
// This file contains a basic struct for specifying RichText styling.

using UnityEngine;

namespace Candlelight
{
	/// <summary>
	/// Rich text style collection.
	/// </summary>
	[System.Serializable]
	public struct RichTextStyle
	{
		/// <summary>
		/// Gets the default style.
		/// </summary>
		/// <value>The default style.</value>
		public static RichTextStyle DefaultStyle { get { return new Candlelight.RichTextStyle(1f, FontStyle.Normal); } }

		#region Backing Fields
		[SerializeField, PropertyBackingField]
		private float m_SizeScalar;
		[SerializeField]
		private FontStyle m_FontStyle;
		[SerializeField]
		private bool m_ShouldReplaceColor;
		[SerializeField]
		private Color m_ReplacementColor;
		private string m_ColorString;
		#endregion

		/// <summary>
		/// Gets the color string.
		/// </summary>
		/// <value>The color string.</value>
		public string ColorString
		{
			get
			{
				if (string.IsNullOrEmpty(m_ColorString))
				{
					m_ColorString = string.Format(
						"#{0}{1}{2}{3}",
						((int)(m_ReplacementColor.r * 255f)).ToString("X2"),
						((int)(m_ReplacementColor.g * 255f)).ToString("X2"),
						((int)(m_ReplacementColor.b * 255f)).ToString("X2"),
						((int)(m_ReplacementColor.a * 255f)).ToString("X2")
					);
				}
				return m_ColorString;
			}
		}
		/// <summary>
		/// Gets the non dynamic version of this <see cref="Candlelight.RichTextStyle"/>.
		/// </summary>
		/// <value>The non dynamic version of this <see cref="Candlelight.RichTextStyle"/>.</value>
		public RichTextStyle NonDynamicVersion
		{
			get
			{
				RichTextStyle result = new RichTextStyle(this.m_ReplacementColor);
				result.m_ShouldReplaceColor = this.m_ShouldReplaceColor;
				return result;
			}
		}
		/// <summary>
		/// Gets the color of the replacement.
		/// </summary>
		/// <value>The color of the replacement.</value>
		public Color ReplacementColor { get { return m_ReplacementColor; } }
		/// <summary>
		/// Gets a value indicating whether this <see cref="Candlelight.RichTextStyle"/> should replace color.
		/// </summary>
		/// <value><c>true</c> if should replace color; otherwise, <c>false</c>.</value>
		public bool ShouldReplaceColor { get { return m_ShouldReplaceColor; } }
		/// <summary>
		/// Gets the font size scalar.
		/// </summary>
		/// <value>The font size scalar.</value>
		public float SizeScalar
		{
			get { return m_SizeScalar; }
			private set { m_SizeScalar = Mathf.Max(0, value); }
		}
		/// <summary>
		/// Gets or sets the font style.
		/// </summary>
		/// <value>The font style.</value>
		public FontStyle FontStyle { get { return m_FontStyle; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Candlelight.RichTextStyle"/> struct. Use this constructor for
		/// styles to be used for non-dynamic fonts.
		/// </summary>
		/// <param name="replacementColor">Replacement color.</param>
		public RichTextStyle(Color replacementColor) : this(1f, FontStyle.Normal, replacementColor)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Candlelight.RichTextStyle"/> struct.
		/// </summary>
		/// <param name="sizeScalar">Size scalar.</param>
		/// <param name="fontStyle">Font style.</param>
		public RichTextStyle(float sizeScalar, FontStyle fontStyle) : this(sizeScalar, fontStyle, Color.white)
		{
			this.m_ShouldReplaceColor = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Candlelight.RichTextStyle"/> struct.
		/// </summary>
		/// <param name="size">Size scalar.</param>
		/// <param name="fontStyle">Font style.</param>
		/// <param name="replacementColor">Replacement color.</param>
		public RichTextStyle(float sizeScalar, FontStyle fontStyle, Color replacementColor) : this()
		{
			this.m_ReplacementColor = replacementColor;
			this.m_ShouldReplaceColor = true;
			this.SizeScalar = sizeScalar;
			this.m_FontStyle = fontStyle;
		}

		/// <summary>
		/// Gets the size of this style based on that of the surrounding text.
		/// </summary>
		/// <returns>The size of this style based on that of the surrounding text.</returns>
		/// <param name="surroundingTextSize">Surrounding text size.</param>
		public int GetSize(int surroundingTextSize)
		{
			return (int)(m_SizeScalar * surroundingTextSize) > 0 ?
				(int)(m_SizeScalar * surroundingTextSize) : surroundingTextSize;
		}

		/// <summary>
		/// Creates a string representation of a start tag for the style.
		/// </summary>
		/// <returns>The start tag.</returns>
		/// <param name="surroundingTextSize">The base size of text to multiply by the size scalar.</param>
		public string ToStartTag(int surroundingTextSize)
		{
			return string.Format(
				"{0}{1}{2}",
				FontStyle == FontStyle.Bold ? "<b>" :
					FontStyle == FontStyle.BoldAndItalic ? "<b><i>" :
					FontStyle == FontStyle.Italic ? "<i>" : "",
				m_ShouldReplaceColor ? string.Format("<color={0}>", ColorString) : "",
				m_SizeScalar != 1f && m_SizeScalar > 0f ? string.Format("<size={0}>", GetSize(surroundingTextSize)) : ""
			);
		}

		/// <summary>
		/// Creates a string representation of an end tag for the style.
		/// </summary>
		/// <returns>The end tag.</returns>
		public string ToEndTag()
		{
			return string.Format(
				"{0}{1}{2}",
				m_SizeScalar != 1f && m_SizeScalar > 0f ? "</size>" : "",
				m_ShouldReplaceColor ? "</color>" : "",
				FontStyle == FontStyle.Bold ? "</b>" :
					FontStyle == FontStyle.BoldAndItalic ? "</i></b>" :
					FontStyle == FontStyle.Italic ? "</i>" : ""
			);
		}
		
		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="Candlelight.RichTextStyle"/>.
		/// </summary>
		/// <param name="obj">
		/// The <see cref="System.Object"/> to compare with the current <see cref="Candlelight.RichTextStyle"/>.
		/// </param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="Candlelight.RichTextStyle"/>; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			RichTextStyle style = (RichTextStyle)obj;
			if ((System.Object)style == null)
			{
				return false;
			}
			return this == style;
		}
		
		/// <summary>
		/// Determines whether the specified <see cref="Candlelight.RichTextStyle"/> is equal to the current
		/// <see cref="Candlelight.RichTextStyle"/>.
		/// </summary>
		/// <param name="style">
		/// The <see cref="Candlelight.RichTextStyle"/> to compare with the current
		/// <see cref="Candlelight.RichTextStyle"/>.</param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="Candlelight.RichTextStyle"/> is equal to the current
		/// <see cref="Candlelight.RichTextStyle"/>; otherwise, <c>false</c>.
		/// </returns>
		public bool Equals(RichTextStyle style)
		{
			if ((object)style == null)
			{
				return false;
			}
			return this == style;
		}
		
		/// <summary>
		/// Serves as a hash function for a <see cref="Candlelight.RichTextStyle"/> object.
		/// </summary>
		/// <returns>
		/// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
		/// hash table.
		/// </returns>
		public override int GetHashCode()
		{
			return ObjectX.GenerateHashCode(
				m_SizeScalar.GetHashCode(),
				m_FontStyle.GetHashCode(),
				m_ShouldReplaceColor.GetHashCode(),
				m_ReplacementColor.GetHashCode()
			);
		}
		
		/// <param name="a">The first <see cref="Candlelight.RichTextStyle"/>.</param>
		/// <param name="b">The second <see cref="Candlelight.RichTextStyle"/>.</param>
		public static bool operator ==(RichTextStyle a, RichTextStyle b)
		{
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}
			return a.GetHashCode() == b.GetHashCode();
		}
		
		/// <param name="a">The first <see cref="Candlelight.RichTextStyle"/>.</param>
		/// <param name="b">The second <see cref="Candlelight.RichTextStyle"/>.</param>
		public static bool operator !=(RichTextStyle a, RichTextStyle b)
		{
			return !(a == b);
		}

		#region Obsolete
		[System.Obsolete("For non-dynamic fonts, specify a size scalar of 0.")]
		public string ToEndTag(bool isForDynamicFont)
		{
			return ToEndTag();
		}
		[System.Obsolete("For non-dynamic fonts, specify a size scalar of 0.")]
		public string ToStartTag(int surroundingTextSize, bool isForDynamicFont)
		{
			return ToStartTag(surroundingTextSize);
		}
		#endregion
	}
}