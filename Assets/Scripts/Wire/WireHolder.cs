using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(DistanceJoint2D), typeof(MovementController))]
public class WireHolder : MonoBehaviour
{
    [Header("Wire Settings")]
    [Required]
    public Wire Wire;
    public float PlaceDistance = 1f;
    public float RetractSpeed = 1f;

    [Header("Variables")]
    public bool ShouldPlace = true;
    [NaughtyAttributes.Label("Place Cooldown (after hanging)")]
    public float PlaceAfterHangingCooldown = 0.8f;

    //@todo calculate this dynamically
    public float Height = 1f;

    private MovementController Controller;
    private DistanceJoint2D DistanceJoint;

    private bool recordTravel = true;
    private bool tryingToHold = false;
    private bool retracting = false;
    private float distanceX = 0;
    private float3 previousPosition = float3.zero;
    private Wire.Point originHangPoint;

    public bool IsHanging => AttachedToWire;
    public bool AttachedToWire => DistanceJoint.enabled;

    public UnityAction<Checkpoint> NewCheckpoint = delegate { };

    public void ToggleRecordingDistance() => recordTravel = !recordTravel;

    public void ToggleHold()
    {
        retracting = false;

        if (!AttachedToWire && !tryingToHold)
        {
            Wire.Tighten();
            tryingToHold = true;
            return;
        }

        if (AttachedToWire)
        {
            DisableHanging();
        }

        if (tryingToHold)
        {
            tryingToHold = false;
        }
    }

    public void ToggleRetract()
    {
        retracting = !retracting;
    }


    void Start()
    {
        TryGetComponent(out DistanceJoint);
        TryGetComponent(out Controller);

        DistanceJoint.enabled = false;
    }

    void EnableShouldPlace()
    {
        ShouldPlace = true;
    }

    void EnableHanging()
    {
        originHangPoint = Wire.LastPlaced;
        retracting = false;
        Wire.LastPointUpdated += UpdateAttachedPoint;
        SyncDistanceJointWithWirePoint();
        DistanceJoint.enabled = true;
        tryingToHold = false;
    }

    void DisableHanging()
    {
        DistanceJoint.enabled = false;
        tryingToHold = false;
        retracting = false;
        Wire.LastPointUpdated -= UpdateAttachedPoint;
        if (ShouldPlace)
        {
            ShouldPlace = false;
            Invoke("EnableShouldPlace", PlaceAfterHangingCooldown);
        }
    }

    void SyncDistanceJointWithWirePoint()
    {
        var distance = math.distance(Wire.LastPlaced.Value, ((float3)transform.position).xy);
        SetJointDistance(distance);
    }

    void SetJointDistance(float distance)
    {
        DistanceJoint.distance = distance;
        DistanceJoint.maxDistanceOnly = true;
        DistanceJoint.connectedAnchor = Wire.LastPlaced.Value;
    }

    void FixedUpdate()
    {
        // Trying to hold state
        if (tryingToHold)
        {
            if (Wire.LastPlacedIsHangable(transform))
            {
                EnableHanging();
            }
        }

        // Hanging state
        UpdateWhenHanging();
        // Retracting state
        UpdateRetractWire();

        // Place wire points state
        UpdateWirePlacement();
    }

    public void UpdateWhenHanging()
    {
        if (!IsHanging) return;
        if (Wire.LastPlaced != originHangPoint)
        {
            if (PointIsInSight(Wire.LastPlaced.Value) && PointIsInSight(Wire.SecondLastPlaced.Value))
            {
                Wire.RemoveLast();
            }
        }
    }

    public bool PointIsInSight(float2 point)
    {
        var direction = math.normalize(point - ((float3)transform.position).xy);
        var distance = math.distance(point, ((float3)transform.position).xy) - 0.15f;
        return !Physics2D.Raycast(transform.position, direction, distance, Wire.GroundMask);
    }

    public void UpdateRetractWire()
    {
        if (!retracting) return;
        var subtract = RetractSpeed * Time.fixedDeltaTime;
        var distance = math.distance(Wire.LastPlaced.Value, ((float3)transform.position).xy) - subtract;
        SetJointDistance(math.max(Height - 0.2f, distance));
    }

    void UpdateWirePlacement()
    {
        if (AttachedToWire || !ShouldPlace) return;
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
