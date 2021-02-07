using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{

    public Animator faceAnimator;
    public Animator bodyAnimator;
    public Animator handsAnimator;

    public Animator powerAnimator;
    public Animator boostAnimator;

    MovementController movementController;
    private bool powerCache = false;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent<MovementController>(out movementController);
        movementController.wireHolder.CheckpointUsed += _ =>
        {
            handsAnimator.SetTrigger("Checkpoint");
            bodyAnimator.SetTrigger("Checkpoint");
        };
        movementController.UsedBoost += () =>
        {
            boostAnimator.SetTrigger("Run");
            JSAM.AudioManager.PlaySound(JSAM.Sounds.Use_Battery);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (math.length(movementController.Direction) != 0 || movementController.wireHolder.AttachedToWire)
        {
            var flip = movementController.Velocity.x < 0;
            if (movementController.GetComponent<SpriteRenderer>().flipX != flip)
            {
                movementController.GetComponent<SpriteRenderer>().flipX = movementController.Velocity.x < 0;
                faceAnimator.GetComponent<SpriteRenderer>().flipX = movementController.Velocity.x < 0;
                bodyAnimator.GetComponent<SpriteRenderer>().flipX = movementController.Velocity.x < 0;
            }
        }

        var sprite = boostAnimator.GetComponent<SpriteRenderer>();
        if (movementController.GetComponent<SpriteRenderer>().flipX)
        {
            sprite.flipX = true;
            sprite.transform.localPosition = new float3(0.5f, sprite.transform.localPosition.y, 0);
        }
        else
        {
            sprite.flipX = false;
            sprite.transform.localPosition = new float3(-0.5f, sprite.transform.localPosition.y, 0);
        }

        if (movementController.HasBoost && !powerCache)
        {
            powerCache = true;
            powerAnimator.SetTrigger("Run");
            JSAM.AudioManager.PlaySound(JSAM.Sounds.Eat_Battery);
        }
        if (!movementController.HasBoost && powerCache)
        {
            powerCache = false;
        }

        bodyAnimator.SetBool("IsGrounded", movementController.IsGrounded);
        handsAnimator.SetBool("IsGrounded", movementController.IsGrounded);
        faceAnimator.SetBool("IsGrounded", movementController.IsGrounded);
        bodyAnimator.SetBool("IsMoving", movementController.IsMovingSideways);
        handsAnimator.SetBool("IsMoving", movementController.IsMovingSideways);
        faceAnimator.SetBool("IsMoving", movementController.IsAcceleratingSideways);

    }
}
