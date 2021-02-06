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

    public float MaxLength;

    public bool Taken { get; private set; } = false;

    // (OldWire, WireHolder, NewWire)
    UnityAction<Wire, WireHolder, Wire> OnChangeWire = delegate { };

    void Start()
    {
        TryGetComponent<SpriteRenderer>(out spriteRenderer);
    }

    protected override void OnInteract(Interactor user)
    {
        if (user.TryGetComponent<WireHolder>(out WireHolder wireHolder) && !Taken)
        {
            wireHolder.transform.position = SocketIn.position;

            Wire oldWire = wireHolder.Wire;
            oldWire.Target = SocketIn;
            wireHolder.ShouldPlace = false;
            //wireHolder.Wire.Freeze();

            Wire newWire = Instantiate(wirePrefab);
            newWire.Target = wireHolder.transform;
            newWire.Origin = SocketOut;
            wireHolder.Wire = newWire;
            //wire.MaxLength = MaxLength;

            spriteRenderer.sprite = TakenSprite;

            Taken = true;
            OnChangeWire.Invoke(oldWire, wireHolder, newWire);
            wireHolder.NewCheckpoint(this);

        }
    }
}
