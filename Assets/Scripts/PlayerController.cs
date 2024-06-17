using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private AnimationController animationController;
    private PlayerControls playerControls;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.Player.Rotate.performed += direction => Rotate(direction.ReadValue<float>());
        playerControls.Player.Attack.performed += _ => Attack();
        playerControls.Player.Dodge.performed += ctx => Dodge(ctx);
        playerControls.Player.Dodge.canceled += ctx => Dodge(ctx);
        playerControls.Player.Throw.performed += _ => Throw();
        playerControls.Player.Fake.performed += _ => Fake();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animationController = gameObject.GetComponent<AnimationController>();
    }

    private void Attack()
    {
        animationController.AttackAnimation();
    }

    private void Dodge(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            print("start");
            animationController.DodgeAnimation();
        }
        else if (ctx.canceled)
        {
            print("end");
            animationController.DodgeToIdleAnimation();
        }
    }

    private void Throw()
    {

    }

    private void Fake()
    {

    }

    private void Rotate(float direction)
    {
        spriteRenderer.flipX = direction > 0f ? true : false;
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }
}
