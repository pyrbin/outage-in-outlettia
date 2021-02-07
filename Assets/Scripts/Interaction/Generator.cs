using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Generator : Interactable
{

    private SpriteRenderer spriteRenderer;

    [SerializeField] private Transform Socket;
    [SerializeField] private Transform Hint;

    public UnityAction OnInserted = delegate { };

    private WireHolder holder;
    private Wire newWire;
    private Wire oldWire;

    public float Time = 0.9f;

    public bool Taken { get; private set; } = false;

    void Start()
    {
        Hint.gameObject.SetActive(false);
    }

    protected override void InRange(Interactor user)
    {
        Hint.gameObject.SetActive(!Taken);
    }

    protected override void LostRange(Interactor user)
    {
        Hint.gameObject.SetActive(false);
    }

    protected override void OnInteract(Interactor user)
    {
        if (user.TryGetComponent<WireHolder>(out WireHolder wireHolder))
        {
            Taken = true;
            holder = wireHolder;
            wireHolder.transform.position = Socket.position - new Vector3(0.35f, 0, 0);
            wireHolder.CheckpointUsed.Invoke(null);
            Invoke("Success", Time);
        }
    }

    public void Success()
    {
        oldWire = holder.Wire;
        oldWire.Target = Socket;
        Hint.gameObject.SetActive(false);
        foreach (var wire in FindObjectsOfType<Wire>())
        {
            wire.SetColor(new Color(255f / 255f, 203f / 255f, 1f / 255f, 1));
        }
        Invoke("DisableOldWire", 2f);
    }

    public void DisableOldWire()
    {
        OnInserted.Invoke();
    }
}
