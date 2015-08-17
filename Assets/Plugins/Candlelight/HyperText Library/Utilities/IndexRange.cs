// 
// IndexRange.cs
// 
// Copyright (c) 2014, Candlelight Interactive, LLC
// All rights reserved.
// 
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://download.unity3d.com/assetstore/customer-eula.pdf
// 
// This file contains a class for describing a range of indices.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Candlelight
{
	/// <summary>
	/// A class for describing a range of indices.
	/// </summary>
	public class IndexRange : System.ICloneable, IEnumerable<int>
	{
		/// <summary>
		/// Gets the number of elements encompassed by this instance.
		/// </summary>
		/// <value>The number of elements encompassed by this instance.</value>
		public int Count { get { return UnityEngine.Mathf.Abs(EndIndex - StartIndex) + 1; } }
		/// <summary>
		/// The direction of the range, positive or negative.
		/// </summary>
		private int Direction { get { return EndIndex >= StartIndex ? 1 : -1; } }
		/// <summary>
		/// Gets or sets the end index.
		/// </summary>
		/// <value>The end index.</value>
		public int EndIndex { get; set; }
		/// <summary>
		/// Gets or sets the start index.
		/// </summary>
		/// <value>The start index.</value>
		public int StartIndex { get; set; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Candlelight.IndexRange"/> class.
		/// </summary>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		public IndexRange(int start, int end)
		{
			StartIndex = start;
			EndIndex = end;
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public object Clone()
		{
			return new IndexRange(this.StartIndex, this.EndIndex);
		}

		/// <summary>
		/// Determines whether or not this instance contains the specified index.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance contains the specified index; otherwise <c>false</c>.
		/// </returns>
		/// <param name="index">Index.</param>
		public bool Contains(int index)
		{
			return Direction > 0 ?
				index >= this.StartIndex && index <= this.EndIndex :
				index <= this.StartIndex && index >= this.EndIndex;
		}

		/// <summary>
		/// Determines whether or not this instance contains the specified other <see cref="Candlelight.IndexRange"/>.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance contains the specified other <see cref="Candlelight.IndexRange"/>; otherwise
		/// <c>false</c>.
		/// </returns>
		/// <param name="other">Other.</param>
		public bool Contains(IndexRange other)
		{
			return Contains(other.StartIndex) && Contains(other.EndIndex);
		}
		
		/// <summary>
		/// Gets an enumerator.
		/// </summary>
		/// <returns>An enumerator.</returns>
		public IEnumerator<int> GetEnumerator()
		{
			return (from i in Enumerable.Range(0, Count) select StartIndex + i * Direction).GetEnumerator();
		}
		
		/// <summary>
		/// Gets an enumerator.
		/// </summary>
		/// <returns>An enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Offset this <see cref="Candlelight.IndexRange"/> using the specified deltaValues.
		/// </summary>
		/// <param name="deltaValues">A collection delta values for each interval in the old range.</param>
		public void Offset(Dictionary<IndexRange, int> deltaValues)
		{
			int direction = Direction;
			if (direction < 0)
			{
				Reverse();
			}
			foreach (KeyValuePair<IndexRange, int> delta in deltaValues)
			{
				int deltaEnd = Mathf.Max(delta.Key.StartIndex, delta.Key.EndIndex);
				int deltaStart = Mathf.Min(delta.Key.StartIndex, delta.Key.EndIndex);
				if (deltaEnd <= StartIndex)				// ...  |-------|
				{
					StartIndex += delta.Value;
					EndIndex += delta.Value;
				}
				else if (Contains(deltaStart))			// |--.----|.....
				{
					if (deltaStart == StartIndex)		// .-------|.....
					{
						StartIndex += delta.Value;
					}
					EndIndex += delta.Value;
				}
				else if (Contains(deltaEnd))			// .....|--.----|
				{
					StartIndex += delta.Value;
					EndIndex += delta.Value;
				}
				else if (								// ...|-------|..
					delta.Key.Contains(StartIndex) && delta.Key.Contains(EndIndex)
				)
				{
					StartIndex += delta.Value;
					EndIndex += delta.Value;
				}
			}
			if (direction < 0)
			{
				Reverse();
			}
		}

		/// <summary>
		/// Reverse this instance.
		/// </summary>
		public void Reverse()
		{
			int start = StartIndex;
			StartIndex = EndIndex;
			EndIndex = start;
		}
		
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Candlelight.IndexRange"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents the current <see cref="Candlelight.IndexRange"/>.
		/// </returns>
		public override string ToString()
		{
			return string.Format("[{0}, {1}]", StartIndex, EndIndex);
		}
	}
}