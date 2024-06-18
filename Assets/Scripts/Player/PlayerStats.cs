using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviourPunCallbacks, IDamageable
{
    private int health = 100;
    private int stamina = 100;

    private int staminaHeal = 5;
    private int healthHeal = 1;

    private bool staminaBurn;

    private StatsBar statsBar;

    private void Start()
    {
        statsBar = StatsBar.instance;
        StartCoroutine(Regeneration());
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if(photonView.IsMine)
            statsBar.SetHealth(health);

        if (health <= 0)
            Die();
    }
    private void UseStamina(int amount)
    {
        stamina = Mathf.Clamp(stamina - amount, 0, 100);

        if (photonView.IsMine)
            statsBar.SetStamina(stamina);
    }

    private void Die()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private IEnumerator Regeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            stamina = Mathf.Clamp(stamina + staminaHeal, 0, 100);
            if (photonView.IsMine)
                statsBar.SetStamina(stamina);
            health = Mathf.Clamp(health + healthHeal, 0, 100);
            if (photonView.IsMine)
                statsBar.SetHealth(health);
        }
    }

    public bool TryUseStamina(int amount)
    {   if (stamina >= amount) 
        {
            UseStamina(amount);
            return true;
        } 
        else
        {
            return false;
        }
        
    }

    public bool StartStaminaBurn(int amount, Action onStaminaEnd)
    {
        if (TryUseStamina(amount))
        {
            StartCoroutine(StaminaBurn(amount, onStaminaEnd));
            return true;
        } else 
            return false;
    }
    public void StopStaminaBurn()
    {
        staminaBurn = false;
    }

    private IEnumerator StaminaBurn(int amount, Action onStaminaEnd)
    {
        staminaBurn = true;
        while (staminaBurn && stamina > 0)
        {
            stamina = Mathf.Clamp(stamina - amount, 0, 100);
            if (photonView.IsMine)
                statsBar.SetStamina(stamina);

            yield return new WaitForSeconds(0.25f);
        }

        if (stamina <= 0)
        {
            print("STAMINA END");
            onStaminaEnd?.Invoke();
            onStaminaEnd = null;
        }
        staminaBurn = false;
    }
}
