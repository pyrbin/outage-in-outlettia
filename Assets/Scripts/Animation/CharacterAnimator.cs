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
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (movementController.MovingDirection.x != 0)
        {
            bool shouldFlip = movementController.MovingDirection.x == 1 ? false : true;
            faceAnimator.GetComponent<SpriteRenderer>().flipX = shouldFlip;
            bodyAnimator.GetComponent<SpriteRenderer>().flipX = shouldFlip;
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
            // do power animation
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
