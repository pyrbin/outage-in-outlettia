using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(EdgeCollider2D))]
public class Wire : MonoBehaviour
{
    public Transform Origin;
    public Transform Target;

    [NaughtyAttributes.Label("Ground")]
    public LayerMask GroundMask;

    public float Width = 0.15f;

    private EdgeCollider2D EdgeCollider;
    private LineRenderer LineRenderer;

    private List<Point> Placed;
    private List<ushort> InAir;

    private ContactPoint2D[] ContactPoints;

    private float Length = 0f;
    private const float Gravity = -2.25f;

    public void Start()
    {
        TryGetComponent(out LineRenderer);
        TryGetComponent(out EdgeCollider);

        Placed = new List<Point>();
        InAir = new List<ushort>();

        Placed.Add(new Point { Value = ((float3)Origin.position).xy });
    }

    public Point Current => new Point { Value = ((float3)Target.position).xy };
    public Point LastPlaced => Placed.Last();
    public bool LastPlacedInAir => InAir.Contains((ushort)(Placed.Count - 1));

    public int InAirIndex(int index) => InAir.IndexOf((ushort)index);

    public void Place()
    {
        var hit = Physics2D.Raycast(Current.Value, -Vector2.up, Mathf.Infinity, GroundMask);
        if (hit.collider != null)
        {
            Length += math.abs(math.distance(Current.Value, LastPlaced.Value));
            Placed.Add(new Point { Value = Current.Value });
            InAir.Add((ushort)(Placed.Count - 1));
        }
    }

    private void FixedUpdate()
    {
        // Between last point & target
        UpdatePointsInAir();

        CheckWrapForInAir();

        WrapWireAroundObstacles(LastPlaced.Value, Current.Value, Placed.Count);

        if (!LastPlacedInAir)
        {
            var hit = Physics2D.Raycast(LastPlaced.Value, -Vector2.up, Mathf.Infinity, GroundMask);
            Debug.Log(hit);
            if (hit.collider != null)
            {
                if (math.abs(math.distance(hit.point, LastPlaced.Value)) > 0.15f)
                {
                    InAir.Add((ushort)(Placed.Count - 1));
                }
            }
        }

    }

    private void CheckWrapForInAir()
    {
        for (var i = InAir.Count - 1; i >= 0; i--)
        {
            var index = InAir[i];
            var point = Placed[index];
            // higher part
            if (index < (Placed.Count - 1))
            {
                var relative = Placed[index + 1];
                var inAirIdx = InAirIndex(index + 1);
                if (WrapWireAroundObstacles(relative.Value, point.Value, index + 1) && inAirIdx != -1)
                {
                    InAir[inAirIdx] = (ushort)Placed.IndexOf(relative);
                    InAir[i] = (ushort)Placed.IndexOf(point);
                }
            }
            // lower part
            else if (index > 0)
            {
                var relative = Placed[index - 1];
                if (WrapWireAroundObstacles(relative.Value, point.Value, index))
                {
                    InAir[i] = (ushort)Placed.IndexOf(point);
                }
            }
        }
    }


    private bool WrapWireAroundObstacles(float2 a, float2 b, int insertAt)
    {
        var added = false;
        var offset = new float2(0, 0.085f);
        var hit = Physics2D.Linecast(a + offset, b + offset, GroundMask);
        if (hit && hit.collider is PolygonCollider2D polygon)
        {
            var point = Physics2DUtility.GetClosestPointFromRaycastHit(hit, polygon);
            Placed.Insert(insertAt, new Point { Value = point });
            DebugDraw.Sphere(new float3(point, 0), 0.75f, Color.red, 5f);
            added = true;
        }
        Debug.DrawLine(new float3(LastPlaced.Value, 1), new float3(Current.Value, 1), Color.white);
        return added;
    }

    private void UpdatePointsInAir()
    {
        for (var i = InAir.Count - 1; i >= 0; i--)
        {
            var index = InAir[i];
            var point = Placed[index];

            var hit = Physics2D.Raycast(point.Value, -Vector2.up, Mathf.Infinity, GroundMask);
            var removed = false;

            if (hit.collider != null)
            {
                if (math.abs(math.distance(hit.point, point.Value)) < 0.15f)
                {
                    InAir.RemoveAt(i);
                    point.Value = hit.point;
                    removed = true;
                }
            }

            if (!removed)
            {
                point.Value.y += Gravity * Time.fixedDeltaTime;
            }

            Placed[index] = point;
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
        DebugDraw.Sphere(new float3(LastPlaced.Value, 0), 0.35f, Color.cyan, .35f);
        DebugDraw.Sphere(new float3(Current.Value, 0), 0.35f, Color.yellow, .35f);
        foreach (var x in Placed)
        {
            // DebugDraw.Circle(new float3(x.Value, 0), Vector2.right, .25f, Color.green);
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

    public struct Point
    {
        public float2 Value;
    }
}
