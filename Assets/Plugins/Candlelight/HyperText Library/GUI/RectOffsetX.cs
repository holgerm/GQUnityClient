// 
// RectOffsetX.cs
// 
// Copyright (c) 2014-2015, Candlelight Interactive, LLC
// All rights reserved.
// 
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://download.unity3d.com/assetstore/customer-eula.pdf

using UnityEngine;

namespace Candlelight
{
	/// <summary>
	/// An extension class for <see cref="UnityEngine.RectOffset"/>.
	/// </summary>
	public static class RectOffsetX
	{
		/// <summary>
		/// Gets the horizontal average.
		/// </summary>
		/// <returns>The horizontal average.</returns>
		/// <param name="rectOffset">A <see cref="UnityEngine.RectOffset"/>.</param>
		public static float GetHorizontalAverage(this RectOffset rectOffset)
		{
			return 0.5f * rectOffset.horizontal;
		}

		/// <summary>
		/// Gets the vertical average.
		/// </summary>
		/// <returns>The vertical average.</returns>
		/// <param name="rectOffset">A <see cref="UnityEngine.RectOffset"/>.</param>
		public static float GetVerticalAverage(this RectOffset rectOffset)
		{
			return 0.5f * rectOffset.vertical;
		}
	}
}