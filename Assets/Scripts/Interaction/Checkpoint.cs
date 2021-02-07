using System.Collections;
using System.Collections.Generic;
using JSAM;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : Interactable
{

    private SpriteRenderer spriteRenderer;

    [SerializeField] private Transform SocketIn;
    [SerializeField] private Transform SocketOut;
    [SerializeField] private Wire wirePrefab;
    [SerializeField] private Sprite TakenSprite;
    [SerializeField] private Transform Hint;

    public float time = 0.9f;

    public float MaxLength = 50;

    public bool Taken { get; private set; } = false;

    private WireHolder holder;
    private Wire newWire;
    private Wire oldWire;

    public UnityAction OnSuccess = delegate { };

    void Start()
    {
        TryGetComponent<SpriteRenderer>(out spriteRenderer);
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
        if (user.TryGetComponent<WireHolder>(out WireHolder wireHolder) && !Taken)
        {
            Taken = true;
            holder = wireHolder;
            wireHolder.transform.position = SocketIn.position - new Vector3(0.347f, 0, 0);
            wireHolder.CheckpointUsed.Invoke(this);
            Invoke("Success", time);
            JSAM.AudioManager.PlaySound(Sounds.Connect_Wire);
        }
    }

    public void Success()
    {
        oldWire = holder.Wire;
        oldWire.Target = SocketIn;
        spriteRenderer.sprite = TakenSprite;
        newWire = Instantiate(wirePrefab);
        //newWire.Lock = true;
        newWire.transform.parent = this.transform;
        holder.distanceX = 0;
        holder.ShouldPlace = false;
        newWire.MaxLength = MaxLength;
        newWire.Target = holder.transform;
        newWire.Origin = SocketOut;
        newWire.Init();
        holder.SetWire(newWire);
        holder.transform.position = SocketOut.position + new Vector3(0.1f, 0, 0);
        OnSuccess.Invoke();
        Hint.gameObject.SetActive(false);
        holder.ShouldPlace = true;
        Invoke("DisableOldWire", .5f);
        Invoke("UnLock", .05f);
        JSAM.AudioManager.PlaySound(Sounds.Checkpoint_Used);


    }
    public void UnLock()
    {
        if (newWire.PointsLength > 1)
            newWire.RemoveLast();
    }
    public void DisableOldWire()
    {
        // oldWire.Freeze();
        this.enabled = false;
    }
}
