using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.Events;

[ExecuteInEditMode]
[RequireComponent(typeof(Collider2D))]
public class CollisionEmitter2D : MonoBehaviour
{
    public event Action<Collision2D> OnCollisionEnter;
    public event Action<Collision2D> OnCollisionExit;

    public event Action<Collider2D> OnTriggerEnter;
    public event Action<Collider2D> OnTriggerExit;

    public Collider2D Collider { get; private set; }

    private void Awake()
    {
        Collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnTriggerExit?.Invoke(other);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        OnCollisionEnter?.Invoke(other);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        OnCollisionExit?.Invoke(other);
    }
}