using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : Interactable
{

    [SerializeField] private Transform SocketIn;
    [SerializeField] private Transform SocketOut;
    [SerializeField] private Wire wirePrefab;

    public float MaxLength;

    // (OldWire, WireHolder, NewWire)
    UnityAction<Wire, WireHolder, Wire> OnChangeWire = delegate { };

    protected override void OnInteract(Interactor user)
    {
        if (user.TryGetComponent<WireHolder>(out WireHolder wireHolder))
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

            OnChangeWire.Invoke(oldWire, wireHolder, newWire);
            wireHolder.NewCheckpoint(this);
        }
    }
}
