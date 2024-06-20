using UnityEngine;

public class DamagePoint : MonoBehaviour
{
    public int damage;

    public bool projectile;

    private int rotationSpeed;

    public int teleports = 1;

    private void Start()
    {
        print($"Start with {teleports}");
        rotationSpeed = Random.Range(5, 30);
    }

    private void OnEnable()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var collider in colliders) 
        {
            if (collider.gameObject.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage);
                return;
            }
        }
    }
}
