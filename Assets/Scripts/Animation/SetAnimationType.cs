using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimationType : MonoBehaviour
{
    [SerializeField] private AnimatorOverrideController[] overrideControllers;
    [SerializeField] private InputReader inputReader;
    private int index = 0;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public void Start()
    {
        TryGetComponent<Animator>(out animator);
        inputReader.InteractEvent += Set;
        SetAnimations(overrideControllers[index]);
    }

    public void Set()
    {
        index++;
        index %= overrideControllers.Length;
        SetAnimations(overrideControllers[index]);
    }

    public void SetAnimations(AnimatorOverrideController overrideController)
    {
        animator.runtimeAnimatorController = overrideController;
    }

}
