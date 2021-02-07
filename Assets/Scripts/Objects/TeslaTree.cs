using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaTree : MonoBehaviour
{
    public Animator Animator;

    [NaughtyAttributes.MinMaxSlider(3, 15)]
    public Vector2 TimeInterval;

    void Start()
    {
        Invoke("PlayAnim", TimeInterval.x);
    }

    void PlayAnim()
    {
        float randomTime = Random.Range(TimeInterval.x, TimeInterval.y);
        Animator.SetTrigger("Play");
        Invoke("PlayAnim", randomTime);
    }
}
