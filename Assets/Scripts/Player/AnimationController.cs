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

    public void ThrowAnimation()
    {
        animator.SetTrigger("Throw");
    }

    public void HurtAnimation()
    {
        animator.SetTrigger("Hurt");
    }
}
