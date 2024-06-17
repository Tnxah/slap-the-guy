using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void AttackAnimation()
    {
        animator.SetTrigger("Attack");
    }

    public void DodgeAnimation()
    {
        animator.SetBool("Dodge", true);
    }

    public void DodgeToIdleAnimation()
    {
        animator.SetBool("Dodge", false);
    }
}
