/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System;
using UnityEngine;

/// <summary>
/// Class implements the basic functionality control of the 2D map.
/// </summary>
[Serializable]
[AddComponentMenu("")]
public class OnlineMapsControlBase2D : OnlineMapsControlBase
{
    /// <summary>
    /// Indicates whether it is possible to get the screen coordinates store. True - for 2D map.
    /// </summary>
    public override bool allowMarkerScreenRect
    {
        get { return true; }
    }
}