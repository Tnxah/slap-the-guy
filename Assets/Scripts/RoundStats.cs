using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundStats : MonoBehaviour
{
    public static int playersKnocked { private set; get; }
    public static int damageDealt { private set; get; }
    public static int damageReceived { private set; get; }
    public static int itemsThrown { private set; get; }
    public static int hits { private set; get; }
    public static float timeInDodge { private set; get; }

    public static void ClearStats()
    {
        playersKnocked = 0;
        damageDealt = 0;
        damageReceived = 0;
        itemsThrown = 0;
        hits = 0;
        timeInDodge = 0;
    }

    public static void PlayerKnocked()
    {
        playersKnocked++;
    }

    public static void DamageDealt(int damage)
    {
        damageDealt += damage;
    }

    public static void DamageReceived(int damage)
    {
        damageReceived += damage; 
    }

    public static void ItemThrown()
    {
        itemsThrown++;
    }

    public static void Hited()
    {
        hits++;
    }

    public static void DodgedSeconds(float time)
    {
        timeInDodge += time;
    }
}
