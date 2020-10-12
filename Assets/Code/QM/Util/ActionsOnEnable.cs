using System;
using UnityEngine;

public class ActionsOnEnable : MonoBehaviour
{
    private readonly ActionsOnDisable _actionsOnDisable = new ActionsOnDisable();
    public event Action OnEnabled;

    public ActionsOnDisable ActionsOnDisable
    {
        get { return _actionsOnDisable; }
    }

    void OnEnable()
    {
        OnEnabled?.Invoke();
    }

}
