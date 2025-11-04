using Photon.Pun;
using RockInMyShoe.Global.DataStorage;
using RockInMyShoe.Global.Eventing;
using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviourPunCallbacks, IDamageable
{
    private PlayerController controller;

    private float health = 100;
    private float stamina = 100;

    private const float CourutineTimeStep = 0.25f;

    private const float healthHeal = 0.25f;
    private const float staminaHeal = 5f;

    private bool regenerateStamina;
    private bool staminaBurn;

    private StatsBar statsBar;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        EventBus.Subscribe<OnGameEndEvent>(CheckWin);
    }

    private void Start()
    {
        statsBar = StatsBar.instance;

        regenerateStamina = true;
        StartCoroutine(Regeneration());
    }

    //===HEALTH===
    public void TakeDamage(int amount, PhotonView damageOwner)
    {
        health -= amount;

        controller.animationController.HurtAnimation();

        if (photonView.IsMine)
        {
            statsBar.SetHealth(health);
            RoundStats.DamageReceived(amount);
        }
        if (damageOwner.IsMine && !photonView.IsMine) //Victim tracks damage for damage owner (but not damage from self)
        {
            RoundStats.DamageDealt(amount);
        }

        if (health <= 0)
        {
            if (damageOwner.IsMine && !photonView.IsMine) //Victim tracks death for killer (but not suicide)
            {
                RoundStats.PlayerKnocked();
            }
            if(photonView.IsMine)
            photonView.RPC("PunRPC_Die", RpcTarget.All);
        }
    }

    [PunRPC]
    private void PunRPC_Die()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
            StatusStorage.SetStatus(BattleStatus.Lose);
        }
        print("PunRPC_Die");
        GameplayController.PlayerDies();
    }

    //===STAMINA===
    public bool TryUseStamina(int amount)
    {
        if(stamina >= amount)
        {
            stamina = Mathf.Clamp(stamina - amount, 0, 100);

            if (photonView.IsMine)
                statsBar.SetStamina(stamina);
            
            return true;
        } else 
            return false;
    }

    //===REGENERATION===
    private IEnumerator Regeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(CourutineTimeStep);
            if (regenerateStamina)
            {
                stamina = Mathf.Clamp(stamina + staminaHeal, 0, 100);
                if (photonView.IsMine)
                    statsBar.SetStamina(stamina);
            }

            health = Mathf.Clamp(health + healthHeal, 0, 100);
            if (photonView.IsMine)
                statsBar.SetHealth(health);
        }
    }


    //===STAMINABURN===
    public void StartStaminaBurn(float amount, Action onStaminaEnd)
    {
        staminaBurn = true;
        regenerateStamina = false;
        StartCoroutine(StaminaBurn(amount, onStaminaEnd));
    }
    public void StopStaminaBurn()
    {
        staminaBurn = false;
        regenerateStamina = true;
    }

    private IEnumerator StaminaBurn(float amount, Action onStaminaEnd)
    {
        while (staminaBurn)
        {
            yield return new WaitForSeconds(CourutineTimeStep);

            stamina = Mathf.Clamp(stamina - amount, 0, 100);
            if (photonView.IsMine)
                statsBar.SetStamina(stamina);

            if (stamina <= 0)
            {
                onStaminaEnd?.Invoke();
                onStaminaEnd = null;
                StopStaminaBurn();
            }
        }
    }

    private void CheckWin(OnGameEndEvent evt)
    {   
        if(photonView.IsMine)

        if(health > 0f)
            StatusStorage.SetStatus(BattleStatus.Win);
        else
            StatusStorage.SetStatus(BattleStatus.Lose);

        print("CheckWin" + StatusStorage.GetStatus<BattleStatus>());
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnGameEndEvent>(CheckWin);
    }
}

public class OnPlayerDieEvent
{
    public bool isMine;
}
