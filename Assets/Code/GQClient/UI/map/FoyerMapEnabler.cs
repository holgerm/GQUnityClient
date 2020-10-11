using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component MUST be placed BELOW the OnlineMaps Component within the game object!
/// </summary>
[RequireComponent(typeof(OnlineMaps))]
public class FoyerMapEnabler : MonoBehaviour
{
    private OnlineMaps _map;
    public OnlineMaps map
    {
        get
        {
            if (_map == null)
            {
                _map = GetComponent<OnlineMaps>();
            }

            return _map;
        }
    }
    
    void OnEnable()
    {
        
    }
}
