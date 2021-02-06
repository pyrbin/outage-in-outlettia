using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : Interactable
{

    private SpriteRenderer spriteRenderer;

    [SerializeField] private Transform SocketIn;
    [SerializeField] private Transform SocketOut;
    [SerializeField] private Wire wirePrefab;
    [SerializeField] private Sprite TakenSprite;

    public float time = 0.9f;

    public float MaxLength = 50;

    public bool Taken { get; private set; } = false;

    public UnityAction OnSuccess = delegate { };

    void Start()
    {
        TryGetComponent<SpriteRenderer>(out spriteRenderer);
    }

    protected override void OnInteract(Interactor user)
    {
        if (user.TryGetComponent<WireHolder>(out WireHolder wireHolder) && !Taken)
        {
            Taken = true;

            wireHolder.transform.position = SocketIn.position - new Vector3(0.35f, 0, 0);

            Wire oldWire = wireHolder.Wire;
            oldWire.Target = SocketIn;

            Wire newWire = Instantiate(wirePrefab);
            newWire.MaxLength = MaxLength;
            newWire.Target = wireHolder.transform;
            newWire.Origin = SocketOut;
            wireHolder.SetWire(newWire);

            Invoke("Success", time);
            wireHolder.CheckpointUsed.Invoke(this);
        }
    }

    public void Success()
    {
        spriteRenderer.sprite = TakenSprite;
        OnSuccess.Invoke();
    }
}
