using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour
{
    public float JumpForce = 7f;
    public float MaxSpeed = 5f;
    public float Acceleration = 15f;
    public float Deacceleration = 20f;
    public float MaxSpeedPoint = 2f;

    public float Direction
    {
        get => movementDirection;
        set => movementDirection = value;
    }

    private Rigidbody2D rbody;
    private ContactFilter2D ContactFilter;
    private WireHolder wireHolder;
    private float movementDirection;

    [Header("Debugging")]
    public bool DrawGizmos = true;

    void Start()
    {
        TryGetComponent<Rigidbody2D>(out rbody);
        TryGetComponent<WireHolder>(out wireHolder);

        // Should only check ground contact
        ContactFilter.useNormalAngle = true;
        ContactFilter.minNormalAngle = 90f;
        ContactFilter.maxNormalAngle = 90f;
    }

    void FixedUpdate()
    {
        if (wireHolder.AttachedToWire)
        {
            WireMovement();
        }
        else
        {
            Deaccalerate();
            Accelerate();
        }
    }

    private void WireMovement()
    {
        if (movementDirection != 0)
        {
            Vector2 direction = new Vector2(wireHolder.Wire.LastPlaced.Value.x - transform.position.x, wireHolder.Wire.LastPlaced.Value.y - transform.position.y);
            Vector3 perp = Vector2.Perpendicular(direction) * movementDirection * -1;
            perp.Normalize();
            rbody.AddForce(perp, ForceMode2D.Force);

#if UNITY_EDITOR
            if (DrawGizmos)
            {
                DebugDraw.Arrow(transform.position, perp, Color.red, 1f);
            }
#endif

        }

    }

    // Returns true if the rigidbody has any velocity on the x-axis

    public bool IsMovingSideways => math.abs(rbody.velocity.x) > 0;
    // Returns true while the player is inputting left or right
    public bool IsAcceleratingSideways => movementDirection != 0;

    public bool IsGrounded => rbody.IsTouching(ContactFilter);

    public bool IsFalling => rbody.velocity.y < 0 && !IsGrounded;

    public float2 Velocity => rbody.velocity;

    public Vector2 MovingDirection => new Vector2(movementDirection, 0);
    public UnityAction OnJump = delegate { };

    public void Jump()
    {

        if (wireHolder.AttachedToWire)
        {
            WireMovement();
            Vector2 direction = new Vector2(wireHolder.Wire.LastPlaced.Value.x - transform.position.x, wireHolder.Wire.LastPlaced.Value.y - transform.position.y);
            rbody.AddForce(direction, ForceMode2D.Impulse);

        }
        else
        {
            if (Mathf.Abs(rbody.velocity.y) < 0.001f)
            {
                rbody.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
                OnJump.Invoke();
            }
        }
    }

    private void Accelerate()
    {
        Vector2 movementDir = new Vector2(movementDirection, 0);
        rbody.velocity += movementDir * Acceleration * Time.deltaTime;

        if (math.abs(rbody.velocity.x) > MaxSpeed)
        {
            rbody.velocity = new Vector2(rbody.velocity.x * (MaxSpeed / rbody.velocity.magnitude), rbody.velocity.y);
        }
    }

    private void Deaccalerate()
    {
        Vector2 movementDir = new Vector2(movementDirection, 0);
        float decrease = Time.deltaTime * Deacceleration;

        float xVelocity = rbody.velocity.x;
        if (xVelocity > 0 && movementDir.x <= 0)
        {
            xVelocity -= decrease;
            xVelocity = Mathf.Clamp(xVelocity, 0f, MaxSpeed);
        }
        if (xVelocity < 0 && movementDir.x >= 0)
        {
            xVelocity += decrease;
            xVelocity = Mathf.Clamp(xVelocity, -MaxSpeed, 0);
        }

        rbody.velocity = new Vector2(xVelocity, rbody.velocity.y);
    }


}
