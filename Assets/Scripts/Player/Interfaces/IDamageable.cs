using Photon.Pun;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int amount, PhotonView damageOwner);
}
