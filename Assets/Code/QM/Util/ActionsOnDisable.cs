using System;
using UnityEngine;

public class ActionsOnDisable : MonoBehaviour
{
    public event Action OnDisabled;

    private void OnDisable()
    {
        OnDisabled?.Invoke();
    }
}