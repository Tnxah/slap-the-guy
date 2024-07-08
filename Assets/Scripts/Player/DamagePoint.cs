using Photon.Pun;
using UnityEngine;

public class DamagePoint : MonoBehaviour
{
    public int damage;

    private void OnEnable()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var collider in colliders) 
        {
            if (collider.gameObject.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage, PhotonView.Get(transform.parent));
                if (PhotonView.Get(transform.parent).IsMine)
                {
                    RoundStats.Hited();
                }
                return;
            }
        }
    }
}
