// 
// FontUpdateTracker.cs
// 
// Copyright (c) 2015, Candlelight Interactive, LLC
// All rights reserved.
// 
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://download.unity3d.com/assetstore/customer-eula.pdf
// 
// This file contains some basic classes for performing a color tween. It
// mirrors those found in UnityEngine.UI.CoroutineTween.
// This file contains a class for tracking updates to font textures being used
// by Candlelight.UI.HyperText objects. It mirrors
// UnityEngine.UI.FontUpdateTracker, which cannot be used because it reads the
// font property directly from a UnityEngine.UI.Text object.

using UnityEngine;
using System.Collections.Generic;

namespace Candlelight.UI
{
	/// <summary>
	/// A class to track changes to font textures being used by <see cref="Candlelight.UI.HyperText"/> objects.
	/// </summary>
	public static class FontUpdateTracker
	{
		/// <summary>
		/// Fonts being tracked, and their respective <see cref="Candlelight.UI.HyperText"/> objects.
		/// </summary>
		private static Dictionary<Font, HashSet<HyperText>> s_Tracked = new Dictionary<Font, HashSet<HyperText>>();

		/// <summary>
		/// Tracks the supplied <see cref="Candlelight.UI.HyperText"/> object.
		/// </summary>
		/// <param name="hyperText">Hyper text.</param>
		public static void TrackHyperText(HyperText hyperText)
		{
			if (hyperText.FontToUse == null)
			{
				return;
			}
			HashSet<HyperText> exists;
			s_Tracked.TryGetValue(hyperText.FontToUse, out exists);
			if (exists == null)
			{
				exists = new HashSet<HyperText>();
				s_Tracked.Add(hyperText.FontToUse, exists);
#if UNITY_4_6
				hyperText.FontToUse.textureRebuildCallback += RebuildForFont(hyperText.FontToUse);
#else
				Font.textureRebuilt += (font) => RebuildForFont(hyperText.FontToUse);
#endif
			}
			exists.Add(hyperText);
		}

		/// <summary>
		/// Gets a texture rebuild callback for the supplied font.
		/// </summary>
		/// <returns>A texture rebuild callback.</returns>
		/// <param name="font">Font.</param>
#if UNITY_4_6
		private static Font.FontTextureRebuildCallback RebuildForFont(Font font)
		{
			return () =>
#else
		private static void RebuildForFont(Font font)
		{
#endif
			{
				HashSet<HyperText> texts;
				s_Tracked.TryGetValue(font, out texts);
				if (texts == null)
				{
					return;
				}
				foreach (HyperText t in texts)
				{
					t.FontTextureChanged();
				}
			};
		}

		/// <summary>
		/// Untracks the supplied <see cref="Candlelight.UI.HyperText"/> object.
		/// </summary>
		/// <param name="hyperText">Hyper text.</param>
		public static void UntrackHyperText(HyperText hyperText)
		{
			if (hyperText.FontToUse == null)
			{
				return;
			}
			HashSet<HyperText> texts;
			s_Tracked.TryGetValue(hyperText.FontToUse, out texts);
			if (texts == null)
			{
				return;
			}
			texts.Remove(hyperText);
		}
	}
}