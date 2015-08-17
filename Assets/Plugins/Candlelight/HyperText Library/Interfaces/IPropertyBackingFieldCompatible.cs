// 
// IPropertyBackingFieldCompatible.cs
// 
// Copyright (c) 2014, Candlelight Interactive, LLC
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

namespace Candlelight
{
	/// <summary>
	/// An interface to specify that a serializable type is compatible with
	/// <see cref="Candlelight.PropertyBackingFieldAttribute"/>.
	/// </summary>
	public interface IPropertyBackingFieldCompatible : System.ICloneable
	{
		/// <summary>
		/// Gets a hash value that is based on the values of the serialized properties of this instance.
		/// </summary>
		/// <remarks>
		/// Note that any reference type fields should implement and test with this interface;
		/// <see cref="System.Collections.IList"/> fields should generate a value-based hash.
		/// </remarks>
		/// <returns>A hash value based on the values of the serialized properties on this instance.</returns>
		int GetSerializedPropertiesHash();
	}

	/// <summary>
	/// Backing field utility class.
	/// </summary>
	/// <typeparam name="T">An <see cref="Candlelight.IPropertyBackingFieldCompatible"/> type.</typeparam>
	public static class BackingFieldUtility<T> where T: IPropertyBackingFieldCompatible
	{
		/// <summary>
		/// An <see cref="System.Collections.Generic.IEqualityComparer{T}"/>, provided as a convenience for evaluating
		/// equality of sequences of <typeparamref name="T"/>.
		/// </summary>
		public class CollectionComparer : System.Collections.Generic.IEqualityComparer<T>
		{
			/// <summary>
			/// Determines if the two specified <typeparamref name="T"/> are equivalent in terms of their serialized
			/// properties.
			/// </summary>
			/// <returns>
			/// <see langword="true"/> if the two <typeparamref name="T"/> are equal; otherwise,
			/// <see langword="false"/>.
			/// </returns>
			/// <param name="a">The first <typeparamref name="T"/>.</param>
			/// <param name="b">The second <typeparamref name="T"/>.</param>
			public bool Equals(T a, T b)
			{
				return a.GetSerializedPropertiesHash() == b.GetSerializedPropertiesHash();
			}
			/// <summary>
			/// Gets the hash code of the specified <typeparamref name="T"/> in terms of its serialized properties.
			/// </summary>
			/// <returns>The hash code.</returns>
			/// <param name="obj">Object.</param>
			public int GetHashCode(T obj)
			{
				return obj == null ? 0 : obj.GetSerializedPropertiesHash();
			}
		}
		#region Backing Fields
		private static CollectionComparer m_Comparer = null;
		#endregion
		/// <summary>
		/// Gets the comparer for this class's type.
		/// </summary>
		/// <value>The comparer for this class's type.</value>
		public static CollectionComparer Comparer
		{
			get
			{
				if (m_Comparer == null)
				{
					m_Comparer = new CollectionComparer();
				}
				return m_Comparer;
			}
		}
	}
}