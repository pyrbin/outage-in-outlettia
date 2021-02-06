using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;

/*
    @ if collider that grounds controller is not collider which is under handler (do raycast down)
    then dont drop cables
*/

[RequireComponent(typeof(DistanceJoint2D), typeof(MovementController))]
public class WireHolder : MonoBehaviour
{
    [Required]
    public Wire Wire;
    public float PlaceDistance = 1f;
    public float Height = 1f;

    [SerializeField]
    private InputReader InputReader;

    private MovementController Controller;
    private DistanceJoint2D DistanceJoint;

    private bool recordTravel = true;
    private float distanceX = 0;
    private float3 previousPosition = float3.zero;

    public bool AttachedToWire => DistanceJoint.enabled;

    void ToggleRecordingDistance() => recordTravel = !recordTravel;

    public void Start()
    {
        TryGetComponent(out DistanceJoint);
        TryGetComponent(out Controller);

        DistanceJoint.enabled = false;
        InputReader.HoldEvent += ToggleHold;
    }

    public void Update()
    {
    }

    void ToggleHold()
    {
        if (AttachedToWire)
        {
            DistanceJoint.enabled = false;
        }
        else if (Controller.IsFalling && !Wire.LastPlacedInAir && math.abs(Wire.LastPlaced.Value.y - transform.position.y) > 1f)
        {
            var distance = math.distance(Wire.LastPlaced.Value, ((float3)transform.position).xy);
            DistanceJoint.distance = distance;
            DistanceJoint.maxDistanceOnly = true;
            DistanceJoint.connectedAnchor = Wire.LastPlaced.Value;
            DistanceJoint.enabled = true;
        }
    }

    void FixedUpdate()
    {
        var isOnEdge = false;
        if (Controller.IsGrounded)
        {
            var hit = Physics2D.Raycast(transform.position, -Vector2.up, Mathf.Infinity, Wire.GroundMask);
            if (hit)
            {
                Debug.DrawLine(transform.position, new float3(hit.point, 0), Color.red);
                isOnEdge = math.abs(math.distance(hit.point.y, transform.position.y)) > Height;
            }
        }

        if (recordTravel && !AttachedToWire)
        {
            RecordDistance();
            if (!isOnEdge)
                DetermineIfDropWire();
        }
    }

    void DetermineIfDropWire()
    {
        if (distanceX >= PlaceDistance)
        {
            if (Controller.IsFalling && !Wire.LastPlacedInAir) return;
            distanceX = 0;
            Drop();
        }
    }

    public void Drop()
    {
        Wire.Place();
    }

    private void RecordDistance()
    {
        distanceX += math.distance(transform.position.x, previousPosition.x);
        previousPosition = (float3)transform.position;
    }

}
