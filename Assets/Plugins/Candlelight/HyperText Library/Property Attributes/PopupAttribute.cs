// 
// PopupAttribute.cs
// 
// Copyright (c) 2015, Candlelight Interactive, LLC
// All rights reserved.
// 
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://download.unity3d.com/assetstore/customer-eula.pdf
// 
// This file contains a custom property attribute to specify that a field
// should display a popup.

namespace Candlelight
{
	/// <summary>
	/// A custom attribute for specifying that a field should display a popup.
	/// </summary>
	public class PopupAttribute : UnityEngine.PropertyAttribute
	{
		/// <summary>
		/// Gets the popup contents getter.
		/// </summary>
		/// <value>The popup contents getter.</value>
		public string PopupContentsGetter { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Candlelight.PopupAttribute"/> class.
		/// </summary>
		/// <param name="popupContentsGetter">
		/// Name of the method for getting the labels, values, and current index with signature:
		/// int (List<GUIContent>, list<object>)
		/// </param>
		public PopupAttribute(string popupContentsGetter)
		{
			PopupContentsGetter = popupContentsGetter;
		}
	}
}