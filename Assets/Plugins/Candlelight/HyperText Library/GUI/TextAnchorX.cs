// 
// TextAnchorX.cs
// 
// Copyright (c) 2014, Candlelight Interactive, LLC
// All rights reserved.
// 
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://download.unity3d.com/assetstore/customer-eula.pdf
// 
// This file contains as class with extension methods for TextAnchor, as well as
// some associate enums. It mirrors some found in UnityEditor.UI.FontDataDrawer.

using UnityEngine;

namespace Candlelight
{
	/// <summary>
	/// Horizontal text aligment.
	/// </summary>
	public enum HorizontalTextAligment { Left, Center, Right }
	/// <summary>
	/// Vertical text aligment.
	/// </summary>
	public enum VerticalTextAligment { Top, Middle, Bottom }

	/// <summary>
	/// Text anchor extensions. Many methods copied from UnityEditor.UI.FontDataDrawer.
	/// </summary>
	public static class TextAnchorX
	{
		/// <summary>
		/// Creates a TextAnchor from vertical and horizontal components.
		/// </summary>
		/// <returns>The TextAnchor.</returns>
		/// <param name="vertical">Vertical.</param>
		/// <param name="horizontal">Horizontal.</param>
		public static TextAnchor GetAnchor(VerticalTextAligment vertical, HorizontalTextAligment horizontal)
		{
			TextAnchor result;
			if (horizontal != HorizontalTextAligment.Left)
			{
				if (horizontal != HorizontalTextAligment.Center)
				{
					if (vertical != VerticalTextAligment.Middle)
					{
						if (vertical != VerticalTextAligment.Bottom)
						{
							result = TextAnchor.UpperRight;
						}
						else
						{
							result = TextAnchor.LowerRight;
						}
					}
					else
					{
						result = TextAnchor.MiddleRight;
					}
				}
				else
				{
					if (vertical != VerticalTextAligment.Middle)
					{
						if (vertical != VerticalTextAligment.Bottom)
						{
							result = TextAnchor.UpperCenter;
						}
						else
						{
							result = TextAnchor.LowerCenter;
						}
					}
					else
					{
						result = TextAnchor.MiddleCenter;
					}
				}
			}
			else
			{
				if (vertical != VerticalTextAligment.Middle)
				{
					if (vertical != VerticalTextAligment.Bottom)
					{
						result = TextAnchor.UpperLeft;
					}
					else
					{
						result = TextAnchor.LowerLeft;
					}
				}
				else
				{
					result = TextAnchor.MiddleLeft;
				}
			}
			return result;
		}

		/// <summary>
		/// Gets the horizontal alignment component of the supplied anchor.
		/// </summary>
		/// <returns>The horizontal alignment component of the supplied anchor.</returns>
		/// <param name="anchor">Anchor.</param>
		public static HorizontalTextAligment GetHorizontalAlignment(this TextAnchor anchor)
		{
			switch (anchor)
			{
			case TextAnchor.UpperLeft:
			case TextAnchor.MiddleLeft:
			case TextAnchor.LowerLeft:
				return HorizontalTextAligment.Left;
			case TextAnchor.UpperCenter:
			case TextAnchor.MiddleCenter:
			case TextAnchor.LowerCenter:
				return HorizontalTextAligment.Center;
			case TextAnchor.UpperRight:
			case TextAnchor.MiddleRight:
			case TextAnchor.LowerRight:
				return HorizontalTextAligment.Right;
			default:
				return HorizontalTextAligment.Left;
			}
		}

		/// <summary>
		/// Gets the vertical alignment component of the supplied anchor.
		/// </summary>
		/// <returns>The vertical alignment component of the supplied anchor.</returns>
		/// <param name="anchor">Anchor.</param>
		public static VerticalTextAligment GetVerticalAlignment(this TextAnchor anchor)
		{
			switch (anchor)
			{
			case TextAnchor.UpperLeft:
			case TextAnchor.UpperCenter:
			case TextAnchor.UpperRight:
				return VerticalTextAligment.Top;
			case TextAnchor.MiddleLeft:
			case TextAnchor.MiddleCenter:
			case TextAnchor.MiddleRight:
				return VerticalTextAligment.Middle;
			case TextAnchor.LowerLeft:
			case TextAnchor.LowerCenter:
			case TextAnchor.LowerRight:
				return VerticalTextAligment.Bottom;
			default:
				return VerticalTextAligment.Top;
			}
		}
	}
}