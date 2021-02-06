using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Interactor : MonoBehaviour
{

    private HashSet<Interactable> InRange;
    private HashSet<Interactable> Tracked;

    [SerializeField]
    private CollisionEmitter2D collisionEmitter;

    private Interactable nearestCache = null;

    public Interactable Nearest => nearestCache;

    public Interactable InteractNearest()
    {
        Nearest?.Interact(this);
        return Nearest;
    }

    private void Start()
    {
        Tracked = new HashSet<Interactable>();
        InRange = new HashSet<Interactable>();

        collisionEmitter.OnTriggerEnter += (data) =>
        {
            if (!GetInteractable(data.transform, out var inter)) return;
            Tracked.Add(inter);
        };

        collisionEmitter.OnTriggerExit += (data) =>
        {
            if (!GetInteractable(data.transform, out var inter)) return;
            RemoveFromTracked(inter);
        };

        StartCoroutine(
     UpdateInRangeLoop(0.1f)
 );

    }

    private bool GetInteractable(Transform target, out Interactable inter)
    {
        if (!target.TryGetComponent<Interactable>(out inter))
            inter = target.GetComponentInParent<Interactable>();
        return inter;
    }

    private void RemoveFromTracked(Interactable interactable)
    {
        Tracked.Remove(interactable);
        if (Tracked.Count == 0)
            InRange.Clear();
        if (interactable == nearestCache)
            nearestCache = null;
    }

    private IEnumerator UpdateInRangeLoop(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            UpdateInRange();
        }
    }

    private void UpdateInRange()
    {
        if (Tracked.Count <= 0) return;
        InRange.Clear();
        var min = float.MaxValue;
        Interactable current = null;
        foreach (var inter in Tracked)
        {
            var dist = math.distance(inter.transform.position, transform.position);
            //if (dist > MaxDistance) continue;
            if (dist < min)
            {
                current = inter;
                min = dist;
            }
            InRange.Add(inter);
        };

        if (current == nearestCache) return;
        nearestCache = current;

    }

}
