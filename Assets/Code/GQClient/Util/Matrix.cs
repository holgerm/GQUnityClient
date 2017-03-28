using System.Collections;
using UnityEngine;

namespace GQ.Client.Util
{

	public static class Matrix
	{

		public static T[] Rotate90<T> (this T[] oldElements, int newWidth, int newHeight)
		{
			Debug.Log ("Called Rotate90()");
			T[] newElements = new T[newWidth * newHeight];

			for (int i = 0; i < newHeight; i++) {
				for (int j = 0; j < newWidth; j++) {
					newElements [j + i * newWidth] = oldElements [newHeight * (newWidth - 1 - j) + i];
				}
			}

			return newElements;
		}


		public static T[] Rotate180<T> (this T[] oldElements, int newWidth, int newHeight)
		{
			Debug.Log ("Called Rotate180()");
			T[] newElements = new T[newHeight * newWidth];

			for (int i = 0; i < newWidth; i++) {
				for (int j = 0; j < newHeight; j++) {
					newElements [j + i * newHeight] = oldElements [newHeight * (newWidth - i) - (1 + j)];
				}
			}

			return newElements;
		}


		public static T[] Rotate270<T> (this T[] oldElements, int newWidth, int newHeight)
		{
			Debug.Log ("Called Rotate270()");
			T[] newElements = new T[newWidth * newHeight];

			for (int i = 0; i < newHeight; i++) {
				for (int j = 0; j < newWidth; j++) {
					newElements [j + i * newWidth] = oldElements [j * newHeight + (newHeight - (i + 1))];
				}
			}

			return newElements;
		}

	}

}
