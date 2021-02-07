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

    public void Lost(Interactor user)
    {
        LostRange(user);
    }

    public void See(Interactor user)
    {
        InRange(user);
    }

    protected abstract void OnInteract(Interactor user);
    protected abstract void InRange(Interactor user);
    protected abstract void LostRange(Interactor user);

}
