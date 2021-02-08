using System;
using System.Collections;
using System.Collections.Generic;
using JSAM;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour
{

    public enum MovementState
    {
        Frozen,
        Free
    }

    public float JumpForce = 7f;
    public float MaxSpeed = 5f;
    public float Acceleration = 15f;
    public float Deacceleration = 20f;

    public float BoostPower = 5f;
    public float SwingForce = 1f;

    public float Direction
    {
        get => movementDirection;
        set => movementDirection = value;
    }

    private Rigidbody2D rbody;
    public ContactFilter2D ContactFilter;
    public WireHolder wireHolder;
    private float movementDirection;

    public MovementState state = MovementState.Free;

    public AudioSource swingingSound;

    public event Action UsedBoost;

    public bool HasBoost = false;

    [Header("Debugging")]
    public bool DrawGizmos = true;

    void Awake()
    {
        TryGetComponent<Rigidbody2D>(out rbody);
        TryGetComponent<WireHolder>(out wireHolder);
        wireHolder.CheckpointUsed += (Checkpoint checkpoint) => CheckpointUsed(checkpoint);
    }

    void FixedUpdate()
    {
        UpdateMovement();
    }

    public void Stop()
    {
        rbody.velocity = float2.zero;
    }

    private void UpdateMovement()
    {
        if (wireHolder.AttachedToWire)
        {
            WireMovement();
        }
        else if (state != MovementState.Frozen)
        {
            if (wireHolder.Wire && wireHolder.Wire.AtMaxLength)
                return;

            if (movementDirection != 0)
            {
                if (IsGrounded)
                {
                    if (!AudioManager.IsSoundPlaying(Sounds.Walk))
                        AudioManager.PlaySound(Sounds.Walk);
                }
                Accelerate();
            }
            Deaccalerate();
        }

        if (IsGrounded || !wireHolder.AttachedToWire)
        {
            if (swingingSound != null)
                AudioManager.StopSoundLoop(Sounds.Swinging);
        }
        else if (wireHolder.AttachedToWire && !AudioManager.IsSoundLooping(Sounds.Swinging))
        {
            swingingSound = AudioManager.PlaySoundLoop(Sounds.Swinging);
        }
        if (swingingSound != null)
            swingingSound.volume = rbody.velocity.magnitude / 15f;
    }

    private void CheckpointUsed(Checkpoint checkpoint)
    {
        Stop();
        state = MovementState.Frozen;
        if (checkpoint)
            checkpoint.OnSuccess += () => state = MovementState.Free;
    }

    private void WireMovement()
    {
        if (movementDirection != 0)
        {
            Vector2 direction = new Vector2(wireHolder.Wire.LastPlaced.Value.x - transform.position.x, wireHolder.Wire.LastPlaced.Value.y - transform.position.y);
            Vector3 perp = Vector2.Perpendicular(direction) * movementDirection * -1;
            perp.Normalize();
            rbody.AddForce(perp * SwingForce, ForceMode2D.Force);


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
        if (wireHolder.Wire && wireHolder.Wire.AtMaxLength || state == MovementState.Frozen)
            return;
        if (!wireHolder.AttachedToWire)
        {
            if (IsGrounded)//math.abs(rbody.velocity.y) < 0.001f)
            {
                JSAM.AudioManager.PlaySound(Sounds.Jump);
                rbody.AddForce(new float2(0, JumpForce), ForceMode2D.Impulse);
                OnJump.Invoke();
            }
        }
    }

    public bool CanUseBoost
        => HasBoost && (wireHolder.AttachedToWire || !IsGrounded && wireHolder.Wire.LastPlacedIsHangable(transform));

    public void Boost()
    {
        if (!CanUseBoost) return;

         wireHolder.ToggleHold();
        // wireHolder.ignoreLastHoldRequest = true;

        rbody.AddForce(math.normalize(rbody.velocity) * BoostPower, ForceMode2D.Impulse);

        HasBoost = false;
        UsedBoost?.Invoke();
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
