using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Generator : Interactable
{

    private SpriteRenderer spriteRenderer;

    [SerializeField] private Transform Socket;
    [SerializeField] private Transform Hint;

    public event UnityAction OnInserted;

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
            wireHolder.transform.position = Socket.position - new Vector3(0.35f, 0, 0);
            Wire oldWire = wireHolder.Wire;
            oldWire.Target = Socket;
            oldWire.Freeze();
            Invoke("Success", Time);

            wireHolder.CheckpointUsed.Invoke(null);
        }
    }

    public void Success()
    {
        OnInserted?.Invoke();
    }
}
