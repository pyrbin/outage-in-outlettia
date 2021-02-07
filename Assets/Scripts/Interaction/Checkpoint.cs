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
        if (user.TryGetComponent<WireHolder>(out WireHolder wireHolder) && !Taken)
        {
            Taken = true;
            holder = wireHolder;
            wireHolder.transform.position = SocketIn.position - new Vector3(0.35f, 0, 0);
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
        newWire.transform.parent = this.transform;
        holder.SetWire(newWire);
        newWire.MaxLength = MaxLength;
        newWire.Target = holder.transform;
        newWire.Origin = SocketOut;
        newWire.Init();
        holder.transform.position = SocketOut.position + new Vector3(0.35f, 0, 0);
        OnSuccess.Invoke();
        Hint.gameObject.SetActive(false);
        Invoke("DisableOldWire", 1f);
        JSAM.AudioManager.PlaySound(Sounds.Checkpoint_Used);

    }

    public void DisableOldWire()
    {
        // oldWire.Freeze();
    }
}
