using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // Triggers when interactable is being interacted
    public event Action<Interactor> Interacted;

    public void Interact(Interactor user)
    {
        OnInteract(user);
        Interacted?.Invoke(user);
    }

    protected abstract void OnInteract(Interactor user);
}
