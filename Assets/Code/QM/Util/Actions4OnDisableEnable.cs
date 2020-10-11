using System;
using UnityEngine;

public class Actions4OnDisableEnable : MonoBehaviour
{
    public event Action OnDisabled;
    public event Action OnEnabled;
   
    void OnDisable()
    {
        OnDisabled?.Invoke();
        Debug.Log("Actions4OnDisableEnable:OnDisable() ---");
    }
    
    void OnEnable()
    {
        OnEnabled?.Invoke();
        Debug.Log("Actions4OnDisableEnable:OnEnable() +++");
    }

}
