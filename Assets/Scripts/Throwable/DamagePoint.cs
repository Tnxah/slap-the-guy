using UnityEngine;

public class DamagePoint : MonoBehaviour
{
    public int damage;

    public bool projectile;

    public LayerMask layerMask;

    private int rotationSpeed;


    private void Start()
    {
        rotationSpeed = Random.Range(5, 30);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.gameObject.name);

        if (projectile && !collision.CompareTag("Projectile"))
        {
            TryDamage(collision);
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (!projectile)
        {
            var collider = Physics2D.OverlapCircle(transform.position, 1f, layerMask);
            if (collider != null)
                TryDamage(collider);
        }
    }

    private void TryDamage(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(damage);
            print("Damaged " + collision.gameObject.name);
        }
    }

    private void FixedUpdate()
    {
        if (projectile)
        {
            transform.Rotate(0, 0, rotationSpeed);
        }
    }

    private void OnBecameInvisible()
    {
        if(projectile)
            Destroy(gameObject);
    }
}
