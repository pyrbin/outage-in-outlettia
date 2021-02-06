using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(EdgeCollider2D))]
public class Wire : MonoBehaviour
{
    [Header("Settings")]
    public Transform Origin;
    public Transform Target;

    [NaughtyAttributes.Label("Ground Layer")]
    public LayerMask GroundMask;

    public float Width = 0.15f;
    public float Gravity = -4.35f;

    public bool WrapAroundCorners = true;

    public event Action<Point> LastPointUpdated;

    [Header("Debugging")]
    public bool DrawGizmos = true;

    private EdgeCollider2D EdgeCollider;
    private LineRenderer LineRenderer;

    private List<Point> Placed;
    private List<Point> InAir;

    private ContactPoint2D[] ContactPoints;

    private float Length = 0f;

    public void Start()
    {
        TryGetComponent(out LineRenderer);
        TryGetComponent(out EdgeCollider);

        Placed = new List<Point>();
        InAir = new List<Point>();

        Placed.Add(new Point { Value = ((float3)Origin.position).xy });
        lastPlacedTemp = LastPlaced;
        LastPointUpdated?.Invoke(lastPlacedTemp);
    }

    public Point Current => new Point { Value = ((float3)Target.position).xy };

    public Point LastPlaced => Placed.Last();
    public bool LastPlacedInAir => InAir.Contains(LastPlaced);
    private Point lastPlacedTemp;

    public bool LastPlacedIsHangable(Transform reference) => IsHangable(Placed.Count - 1, reference);
    public int InAirIndex(Point point) => InAir.IndexOf(point);

    public void Tighten()
    {
        var toRemove = new List<(int AirIdx, int PlacedIdx)>();
        var isViablePivot = false;
        for (var i = Placed.Count - 1; i >= 0; i--)
        {
            var point = Placed[i];
            var airIdx = InAirIndex(point);
            if (airIdx == -1)
            {
                // found grounded point
                isViablePivot = IsViableHangPivot(i, Target);
                break;
            }
            else
            {
                toRemove.Add((airIdx, i));
            }
        }

        if (isViablePivot)
        {
            foreach (var (air, placed) in toRemove)
            {
                InAir.RemoveAt(air);
                Placed.RemoveAt(placed);
            }
        }
    }

    public void Place()
    {
        var hit = Physics2D.Raycast(Current.Value, -Vector2.up, Mathf.Infinity, GroundMask);
        if (hit.collider != null)
        {
            Length += math.abs(math.distance(Current.Value, LastPlaced.Value));
            Placed.Add(new Point { Value = Current.Value });
            InAir.Add(LastPlaced);
        }
    }

    public bool IsViableHangPivot(int index, Transform reference)
    {
        const float minHangDistance = 1f;
        var point = Placed[index];
        if (InAirIndex(point) != -1
            || math.abs(point.Value.y - reference.position.y) < minHangDistance)
            return false;
        return true;
    }

    public bool IsHangable(int index, Transform reference)
    {
        var point = Placed[index];
        return point.Value.y > reference.position.y && IsViableHangPivot(index, reference);
    }

    private bool NotTooSimilarToLast(float2 point)
    {
        return math.abs(math.distance(LastPlaced.Value, point)) > 0.05f;
    }


    private void TargetToLastRaycastCheck()
    {
        var direction = math.normalize(LastPlaced.Value - ((float3)Target.position).xy);
        var distance = math.distance(LastPlaced.Value, ((float3)Target.position).xy);
        var hit = Physics2D.Raycast(Target.position, direction, distance, GroundMask);
        if (hit && NotTooSimilarToLast(hit.point))
        {
            Placed.Add(new Point { Value = hit.point });
        }
    }

    private void FixedUpdate()
    {
        // Between last point & target
        UpdatePointsInAir();

        CheckWrapForInAir();

        WrapWireAroundObstacles(LastPlaced.Value, Current.Value, Placed.Count);

        //@todo uncomment for corner wrapping
        // not working 100% atm
        if (WrapAroundCorners)
            TargetToLastRaycastCheck();

        if (lastPlacedTemp != LastPlaced)
        {
            lastPlacedTemp = LastPlaced;
            LastPointUpdated?.Invoke(lastPlacedTemp);
        }
    }

    private void CheckWrapForInAir()
    {
        for (var i = InAir.Count - 1; i >= 0; i--)
        {
            var point = InAir[i];

            // higher part
            if (point != LastPlaced)
            {
                var relativeIdx = Placed.IndexOf(point) + 1;
                var relative = Placed[relativeIdx];
                WrapWireAroundObstacles(relative.Value, point.Value, relativeIdx);
            }
            // lower part
            if (point != Placed[0])
            {
                var relativeIdx = Placed.IndexOf(point) - 1;
                var relative = Placed[relativeIdx];
                WrapWireAroundObstacles(relative.Value, point.Value, relativeIdx + 1);

            }
        }
    }


    private bool WrapWireAroundObstacles(float2 a, float2 b, int insertAt)
    {
        var added = false;
        var offset = new float2(0, 0.055f);
        var hit = Physics2D.Linecast(a + offset, b + offset, GroundMask);
        if (hit && hit.collider is PolygonCollider2D polygon)
        {
            var point = Physics2DUtility.GetClosestPointFromRaycastHit(hit, polygon);
            if (NotTooSimilarToLast(point))
            {
                Placed.Insert(insertAt, new Point { Value = point });
                if (DrawGizmos)
                    DebugDraw.Sphere(new float3(point, 0), 0.15f, Color.red, 3f);
                added = true;
            }
        }
        if (DrawGizmos)
        {
            Debug.DrawLine(new float3(a + offset, 1), new float3(b + offset, 1), Color.blue);
        }
        return added;
    }

    private void UpdatePointsInAir()
    {
        for (var i = InAir.Count - 1; i >= 0; i--)
        {
            var point = InAir[i];
            // Raycast(point.Value, -Vector2.up, Mathf.Infinity, GroundMask);
            var hit = Physics2D.OverlapCircle(point.Value, 0.1f, GroundMask);
            var removed = false;

            if (hit != null && hit.transform.position.y < point.Value.y)
            {
                InAir.RemoveAt(i);
                removed = true;
            }

            if (!removed)
            {
                point.Value.y += Gravity * Time.fixedDeltaTime;
            }

            //Placed[index] = point;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Sets all collision contact points
        ContactPoints = collision.contacts;
        foreach (var contact in ContactPoints)
        {
#if UNITY_EDITOR
            //Debug.DrawRay(contact.point, contact.normal, Color.red);
#endif
        }
    }

    void OnCollisionExit2D()
    {
        // Resets contact points to empty
        ContactPoints = null;
    }

    public void Update()
    {
        RenderWire();
#if UNITY_EDITOR
        if (DrawGizmos)
        {
            DebugDraw.Sphere(new float3(LastPlaced.Value, 0), 0.15f, Color.cyan);
            DebugDraw.Sphere(new float3(Current.Value, 0), 0.15f, Color.yellow);
            foreach (var x in InAir)
            {
                DebugDraw.Sphere(new float3(x.Value, 0), 0.15f, Color.green);
            }
        }
#endif
    }

    public void RenderWire()
    {
        var points = Placed.Select(x => new Vector3(x.Value.x, x.Value.y, 0)).ToList();
        points.Add(new float3(Current.Value, 0));

        LineRenderer.startWidth = Width;
        LineRenderer.endWidth = Width;
        LineRenderer.positionCount = points.Count;
        LineRenderer.SetPositions(points.ToArray());

        EdgeCollider.edgeRadius = Width / 2f;
        EdgeCollider.points = points.Select(x => new Vector2(x.x, x.y)).ToArray();
    }

    public class Point
    {
        public float2 Value;
    }
}
