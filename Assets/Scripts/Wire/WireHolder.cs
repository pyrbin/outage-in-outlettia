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
            Wire.LastPointUpdated -= UpdateAttachedPoint;
        }
        // set attached
        else if (Controller.IsFalling)
        {
            Wire.Tighten();
            if (Wire.LastPlacedIsHangable(transform))
            {
                Wire.LastPointUpdated += UpdateAttachedPoint;
                SyncDistanceJoinWithWirePoint();
                DistanceJoint.enabled = true;
            }
        }
    }

    void SyncDistanceJoinWithWirePoint()
    {
        var distance = math.distance(Wire.LastPlaced.Value, ((float3)transform.position).xy);
        DistanceJoint.distance = distance;
        DistanceJoint.maxDistanceOnly = true;
        DistanceJoint.connectedAnchor = Wire.LastPlaced.Value;
    }

    void FixedUpdate()
    {
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
        // State Attached
        else if (AttachedToWire)
        {
            var direction = math.normalize(Wire.LastPlaced.Value - ((float3)transform.position).xy);
            var distance = math.distance(Wire.LastPlaced.Value, ((float3)transform.position).xy);
            var hit = Physics2D.Raycast(transform.position, direction, distance, Wire.GroundMask);
            if (hit)
            {
                Wire.AddPointToBack(new Wire.Point { Value = hit.point });
            }
        }

    }

    void UpdateAttachedPoint(Wire.Point point)
    {
        SyncDistanceJoinWithWirePoint();
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
