using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{

    public Animator faceAnimator;
    public Animator bodyAnimator;
    MovementController movementController;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent<MovementController>(out movementController);
    }

    // Update is called once per frame
    void Update()
    {
        bodyAnimator.SetBool("IsGrounded", movementController.IsGrounded);
        bodyAnimator.SetBool("IsMoving", movementController.IsMovingSideways);
        faceAnimator.SetBool("IsMoving", movementController.IsAcceleratingSideways);
    }
}
