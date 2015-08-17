// 
// ColorGradient.cs
// 
// Copyright (c) 2011-2015, Candlelight Interactive, LLC
// All rights reserved.
// 
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://download.unity3d.com/assetstore/customer-eula.pdf

using UnityEngine;

namespace Candlelight
{
	
	/// <summary>
	/// An <c>enum</c> to specify how intermediate color values should be calculated.
	/// </summary>
	public enum ColorInterpolationSpace { RGB, HSV }

	/// <summary>
	/// A linear gradient between two <see cref="UnityEngine.Color"/>s.
	/// </summary>
	[System.Serializable]
	public struct ColorGradient
	{
		#region Backing Fields
		[SerializeField]
		private readonly Color m_MaxColor;
		[SerializeField]
		private readonly Color m_MinColor;
		[SerializeField]
		private readonly ColorInterpolationSpace m_InterpolationSpace;
		#endregion
		/// <summary>
		/// Gets the end color.
		/// </summary>
		/// <value>The end color.</value>
		public Color MaxColor { get { return m_MaxColor; } }
		/// <summary>
		/// Gets the start color.
		/// </summary>
		/// <value>The start color.</value>
		public Color MinColor { get { return m_MinColor; } }
		/// <summary>
		/// Gets the interpolation space.
		/// </summary>
		/// <value>The interpolation space.</value>
		public ColorInterpolationSpace InterpolationSpace { get { return m_InterpolationSpace; } }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Candlelight.ColorGradient"/> struct.
		/// </summary>
		/// <param name="minColor">Minimum color.</param>
		/// <param name="maxColor">Maximum color.</param>
		/// <param name="interpolationSpace">Interpolation space.</param>
		public ColorGradient(
			Color minColor, Color maxColor,
			ColorInterpolationSpace interpolationSpace = ColorInterpolationSpace.RGB
		) : this()
		{	
			this.m_MinColor = minColor;
			this.m_MaxColor = maxColor;
			this.m_InterpolationSpace = interpolationSpace;
		}
		
		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="Candlelight.ColorGradient"/>.
		/// </summary>
		/// <param name="obj">
		/// The <see cref="System.Object"/> to compare with the current <see cref="Candlelight.ColorGradient"/>.
		/// </param>
		/// <returns>
		/// <see langword="true"/> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="Candlelight.ColorGradient"/>; otherwise, <see langword="false"/>.
		/// </returns>
		public override bool Equals(object obj)
		{
	        if (obj == null)
	        {
	            return false;
	        }
			if (obj is ColorGradient)
			{
				return this == (ColorGradient)obj;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Evaluate the color at the specified parameter value.
		/// </summary>
		/// <returns>
		/// The interpolated <see cref="UnityEngine.Color"/> at the specified parameter in this instance.
		/// </returns>
		/// <param name="t">A parameter value in the range [0, 1].</param>
		public Color Evaluate(float t)
		{
			return (InterpolationSpace == ColorInterpolationSpace.RGB) ?
				Color.Lerp(this.MinColor, this.MaxColor, t) :
				ColorHSV.Lerp(this.MinColor, this.MaxColor, t);
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="Candlelight.ColorGradient"/> object.
		/// </summary>
		/// <returns>
		/// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
		/// hash table.
		/// </returns>
		public override int GetHashCode()
		{
			return ObjectX.GenerateHashCode(
				MinColor.GetHashCode(), MaxColor.GetHashCode(), InterpolationSpace.GetHashCode()
			);
		}

		/// <summary>
		/// Gets a value indicating whether or not the two <see cref="Candlelight.ColorGradient"/>s are equal to one
		/// another.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the two <see cref="Candlelight.ColorGradient"/>s are equal;
		/// otherwise, <see langword="false"/>.
		/// </returns>
		/// <param name="cg1">The first <see cref="Candlelight.ColorGradient"/>.</param>
		/// <param name="cg2">The second <see cref="Candlelight.ColorGradient"/>.</param>
		public static bool operator ==(ColorGradient cg1, ColorGradient cg2)
		{
			return cg1.GetHashCode() == cg2.GetHashCode(); 
		}

		/// <summary>
		/// Gets a value indicating whether or not the two <see cref="Candlelight.ColorGradient"/>s are unequal to one
		/// another.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the two <see cref="Candlelight.ColorGradient"/>s are unequal;
		/// otherwise, <see langword="false"/>.
		/// </returns>
		/// <param name="cg1">The first <see cref="Candlelight.ColorGradient"/>.</param>
		/// <param name="cg2">The second <see cref="Candlelight.ColorGradient"/>.</param>
		public static bool operator !=(ColorGradient cg1, ColorGradient cg2)
		{
			return !(cg1 == cg2);
		}
	}
}