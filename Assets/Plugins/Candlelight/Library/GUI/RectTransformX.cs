// 
// RectTransformX.cs
// 
// Copyright (c) 2016-2017, Candlelight Interactive, LLC
// All rights reserved.
// 
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://download.unity3d.com/assetstore/customer-eula.pdf

using UnityEngine;

namespace Candlelight
{
	/// <summary>
	/// Extension methods for RectTransform.
	/// </summary>
	public static class RectTransformX
	{
		/// <summary>
		/// Allocation for getting world corners;
		/// </summary>
		private static Vector3[] s_WorldCorners = new Vector3[4];

		/// <summary>
		/// Gets a value indicating whether all <see cref="UnityEngine.CanvasGroup"/> components on ancestors of
		/// <paramref name="transform"/> permit interaction.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if all <see cref="UnityEngine.CanvasGroup"/> components on ancestors of
		/// <paramref name="transform"/> permit interaction; otherwise, <see langword="false"/>.
		/// </returns>
		/// <param name="transform">Transform to test.</param>
		public static bool DoGroupsAllowInteraction(this RectTransform transform)
		{
			// figure out if parent groups allow interaction
			bool doGroupsAllowInteraction = true;
			using (var canvasGroups = new ListPool<CanvasGroup>.Scope())
			{
				while (transform != null)
				{
					transform.GetComponents(canvasGroups.List);
					bool shouldBreak = false;
					for (int i = 0; i < canvasGroups.List.Count; ++i)
					{
						if (!canvasGroups.List[i].interactable)
						{
							doGroupsAllowInteraction = false;
							shouldBreak = true;
						}
						if (canvasGroups.List[i].ignoreParentGroups)
						{
							shouldBreak = true;
						}
					}
					if (shouldBreak)
					{
						break;
					}
					transform = transform.parent as RectTransform;
				}
			}
			return doGroupsAllowInteraction;
		}

		/// <summary>
		/// Gets the pixel adjusted rect.
		/// </summary>
		/// <returns>The pixel adjusted rect.</returns>
		/// <param name="rectTransform">Rect transform.</param>
		/// <param name="canvas">Canvas.</param>
		public static Rect GetPixelAdjustedRect(this RectTransform rectTransform, Canvas canvas)
		{
			return (canvas == null || !canvas.pixelPerfect) ?
				rectTransform.rect : RectTransformUtility.PixelAdjustRect(rectTransform, canvas);
		}


		/// <summary>
		/// Gets the world center.
		/// </summary>
		/// <returns>The world center.</returns>
		/// <param name="rectTransform">Rect transform.</param>
		public static Vector3 GetWorldCenter(this RectTransform rectTransform)
		{
			rectTransform.GetWorldCorners(s_WorldCorners);
			return 0.25f * (s_WorldCorners[0] + s_WorldCorners[1] + s_WorldCorners[2] + s_WorldCorners[3]);
		}

		/// <summary>
		/// Gets the size of the RectTransform's rect in world space.
		/// </summary>
		/// <returns>The size of the RectTransform's rect in world space.</returns>
		/// <param name="rectTransform">Rect transform.</param>
		public static Vector2 GetWorldSize(this RectTransform rectTransform)
		{
			rectTransform.GetWorldCorners(s_WorldCorners);
			Quaternion q = Quaternion.Inverse(rectTransform.rotation);
			for (int i=0; i<s_WorldCorners.Length; ++i)
			{
				s_WorldCorners[i] = q * s_WorldCorners[i];
			}
			return new Vector2(s_WorldCorners[2].x - s_WorldCorners[0].x, s_WorldCorners[2].y - s_WorldCorners[0].y);
		}
	}
}