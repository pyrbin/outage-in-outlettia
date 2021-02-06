using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    Animator animator;
    MovementController movementController;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent<Animator>(out animator);
        TryGetComponent<MovementController>(out movementController);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("IsGrounded", movementController.IsGrounded);
        animator.SetBool("IsMoving", movementController.IsMovingSideways);
    }
}
