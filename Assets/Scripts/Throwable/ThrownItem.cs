using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownItem : MonoBehaviour
{
    [SerializeField]
    private int damage;
    private int rotationSpeed;

    public int teleportations = 1;

    private void Start()
    {
        rotationSpeed = Random.Range(5, 30);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
         transform.Rotate(0, 0, rotationSpeed);
    }
}
