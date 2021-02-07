using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

public static class Physics2DUtility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float2 GetClosestPointFromPoint(float2 point, PolygonCollider2D polyCollider)
    {
        // Transform polygoncolliderpoints to world space (default is local)
        var distanceDictionary = polyCollider.points.ToDictionary<Vector2, float, Vector2>(
            position => Vector2.Distance(point, polyCollider.transform.TransformPoint(position)),
            position => polyCollider.transform.TransformPoint(position));

        var orderedDictionary = distanceDictionary.OrderBy(e => e.Key);
        return orderedDictionary.Any() ? orderedDictionary.First().Value : Vector2.zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float2 GetClosestPointFromPoint(float2 point, CompositeCollider2D composite)
    {
        var points = new List<Vector2>();
        for (int i = 0; i < composite.pathCount; i++)
        {
            Vector2[] currentPoints = new Vector2[composite.GetPathPointCount(i)];
            composite.GetPath(i, currentPoints);
            points.AddRange(currentPoints);
        }

        var distanceDictionary = points.ToDictionary<Vector2, float, Vector2>(
            position => Vector2.Distance(point, composite.transform.TransformPoint(position)),
            position => composite.transform.TransformPoint(position));

        var orderedDictionary = distanceDictionary.OrderBy(e => e.Key);
        return orderedDictionary.Any() ? orderedDictionary.First().Value : Vector2.zero;
    }

}
