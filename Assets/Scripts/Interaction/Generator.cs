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

    public bool Seen { get; private set; } = false;
    public Interactor interactor;

    protected override void InRange(Interactor user)
    {
        Seen = !Taken;
        interactor = user;
    }

    protected override void LostRange(Interactor user)
    {
        Seen = false;
        Hint.gameObject.SetActive(false);
        interactor = null;
    }

    void Update()
    {
        if (Seen && !Taken && interactor && interactor.Nearest)
        {
            Hint.gameObject.SetActive(!Taken && interactor.Nearest == this as Interactable);
        }
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
            JSAM.AudioManager.PlaySound(JSAM.Sounds.Connect_Wire);
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
        this.enabled = false;
    }
}
