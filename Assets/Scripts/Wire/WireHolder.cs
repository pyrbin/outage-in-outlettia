using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(DistanceJoint2D), typeof(MovementController))]
public class WireHolder : MonoBehaviour
{
    [Required]
    public Wire Wire;
    public float PlaceDistance = 1f;
    public float Height = 1f;
    public bool ShouldPlace = true;

    private MovementController Controller;
    private DistanceJoint2D DistanceJoint;

    private bool recordTravel = true;
    private bool tryingToHold = false;
    private float distanceX = 0;
    private float3 previousPosition = float3.zero;

    public bool AttachedToWire => DistanceJoint.enabled;

    public void ToggleRecordingDistance() => recordTravel = !recordTravel;

    private void Start()
    {
        TryGetComponent(out DistanceJoint);
        TryGetComponent(out Controller);

        DistanceJoint.enabled = false;
    }

    public void ToggleHold()
    {
        if (!AttachedToWire && !tryingToHold)
        {
            Wire.Tighten();
            tryingToHold = true;
            return;
        }

        if (AttachedToWire)
        {
            DistanceJoint.enabled = false;
            Wire.LastPointUpdated -= UpdateAttachedPoint;
        }

        if (tryingToHold)
        {
            tryingToHold = false;
        }
    }

    void SyncDistanceJointWithWirePoint()
    {
        var distance = math.distance(Wire.LastPlaced.Value, ((float3)transform.position).xy);
        DistanceJoint.distance = distance;
        DistanceJoint.maxDistanceOnly = true;
        DistanceJoint.connectedAnchor = Wire.LastPlaced.Value;
    }

    void FixedUpdate()
    {
        if (!ShouldPlace) return;

        if (tryingToHold)
        {
            if (Wire.LastPlacedIsHangable(transform))
            {
                Wire.LastPointUpdated += UpdateAttachedPoint;
                SyncDistanceJointWithWirePoint();
                DistanceJoint.enabled = true;
                tryingToHold = false;
            }
        }

        // State Normal
        if (!AttachedToWire)
        {
            var isOnEdge = false;
            if (Controller.IsGrounded)
            {
                var hit = Physics2D.Raycast(transform.position, -Vector2.up, Mathf.Infinity, Wire.GroundMask);
                if (hit)
                {
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
    }

    void UpdateAttachedPoint(Wire.Point point)
    {
        SyncDistanceJointWithWirePoint();
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

    void Drop()
    {
        Wire.Place();
    }

    void RecordDistance()
    {
        distanceX += math.distance(transform.position.x, previousPosition.x);
        previousPosition = (float3)transform.position;
    }

}
