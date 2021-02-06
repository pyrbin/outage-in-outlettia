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

    [SerializeField] private InputReader inputReader;
    private Rigidbody2D rbody;
    private float movementDirection;
    private ContactFilter2D ContactFilter;

    void Start()
    {
        TryGetComponent<Rigidbody2D>(out rbody);

        // Should only check ground contact
        ContactFilter.useNormalAngle = true;
        ContactFilter.minNormalAngle = 90f;
        ContactFilter.maxNormalAngle = 90f;

        inputReader.MoveEvent += (float dir) => movementDirection = dir;
        inputReader.JumpEvent += Jump;
    }


    void FixedUpdate()
    {
        Deaccalerate();
        Accelerate();
    }

    // Returns true if the rigidbody has any velocity on the x-axis

    public bool IsMovingSideways => math.abs(rbody.velocity.x) > 0;
    // Returns true while the player is inputting left or right
    public bool IsAcceleratingSideways => movementDirection != 0;

    public bool IsGrounded => rbody.IsTouching(ContactFilter);

    public bool IsFalling => rbody.velocity.y <= 0 && !IsGrounded;

    public float2 Velocity => rbody.velocity;

    public Vector2 MovingDirection => new Vector2(movementDirection, 0);
    public UnityAction OnJump = delegate { };

    void Jump()
    {
        if (Mathf.Abs(rbody.velocity.y) < 0.001f)
        {
            rbody.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
            OnJump.Invoke();
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
