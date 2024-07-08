using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundStats : MonoBehaviour
{
    private static int playersKnocked;
    private static int damageDealt;
    private static int damageReceived;
    private static int itemsThrowned;
    private static int hits;
    private static float timeInDodge;

    public static void ClearStats()
    {
        playersKnocked = 0;
        damageDealt = 0;
        damageReceived = 0;
        itemsThrowned = 0;
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
        itemsThrowned++;
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
